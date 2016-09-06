using Nfe.Entidade;
using Nfe.Negocio.InterfacesAbstratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Nfe.consReciNFe;
using NFe.retConsReciNFe;
using Nfe.Negocio.Geral;
using Nfe.Model;
using System.IO;

namespace Nfe.Negocio.ConsultaRecibo
{

    public class ConRecibo : abMetodos, IMetodosBase<Entidade_Recibo>
    {
        TConsReciNFe ReciNFe;
        TRetConsReciNFe RetReciNFe;
        public void Enviar(Entidade_Recibo ObjEnt, out Entidade_Recibo objDados)
        {
            ReciNFe = new TConsReciNFe();
            docTran = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            Settings = new XmlWriterSettings();
            xmlStatus = new XmlSerializer(typeof(TConsReciNFe));

            ReciNFe = new TConsReciNFe(ObjEnt.Recibo, ObjEnt.TpAmb);

            Settings.Encoding = UTF8Encoding.UTF8;
            Settings.NewLineHandling = NewLineHandling.None;
            Settings.Indent = true;
            Settings.IndentChars = "";

            ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

            Sw = new UTF8StringWriter();
            Wx = XmlWriter.Create(Sw, Settings);
            xmlStatus.Serialize(Sw, ReciNFe, ns);
            string xmlGer = Sw.ToString();

            docTran.LoadXml(xmlGer);
            docTran.PreserveWhitespace = false;

            CertEmpresa = AssinaturaDigital.FindCertOnStore(ObjEnt.Loja);
            try
            {
                EnviarXml(docTran, CertEmpresa, ref ObjEnt);

                objDados = ObjEnt;
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.RetAutoriz, "Saida", Ex.Message.ToString());
                objDados = null;
            }
        }
        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_Recibo obj)
        {
            try
            {
                object retObj = new object();
                UrlEstados = new UrlNfesEstados();
                RetAutorizacao.NfeRetAutorizacao wRetAut = new RetAutorizacao.NfeRetAutorizacao();
                RetAutorizacao.nfeCabecMsg wCabMsg = new RetAutorizacao.nfeCabecMsg();

                wCabMsg.cUF = obj.cUf.ToString();
                wCabMsg.versaoDados = "3.10";

                wRetAut.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                wRetAut.PreAuthenticate = true;
                wRetAut.ClientCertificates.Add(cert);
                wRetAut.nfeCabecMsgValue = wCabMsg;

                nodeList = doc.GetElementsByTagName("consReciNFe");
                nodeStatus = nodeList.Item(0);
                wRetAut.Url = UrlEstados.SetarUrlEstado(UrlEstados.Uf(int.Parse(obj.cUf.ToString())), obj.TpAmb == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.RetAutorizacao);
                retObj = wRetAut.nfeRetAutorizacaoLote(nodeStatus);
                DeserilizarEvento(retObj, ref obj);
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.RetAutoriz, "Saida", Ex.Message.ToString());
            }
        }
        public void DeserilizarEvento(object obj, ref Entidade_Recibo objDes)
        {
            Entidade_Recibo retRecibo = new Entidade_Recibo();
            Model_NotaFiscal mNf = new Model_NotaFiscal();
            Model_Lote mLote = new Model_Lote();
            XmlDocument docRet = new XmlDocument();
            RetReciNFe = new TRetConsReciNFe();
            StreamWriter SW;
            string xProtNFe = string.Empty;

            object objRet = new object();

            retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
            xmlDesSerializar = new XmlSerializer(typeof(TRetConsReciNFe));

            objRet = xmlDesSerializar.Deserialize(retXmlNodeReader);

            var Retorno = (TRetConsReciNFe)objRet;


            XmlNodeList xNodList = ((XmlNode)obj).ChildNodes;

            try
            {
                if (Retorno.protNFe != null)
                {
                    foreach (var Prot in Retorno.protNFe)
                    {
                        if (objDes.TpNf.Trim() == "S")
                            mNf.AtualizaRetornoNfeSaida(Prot.infProt.cStat, Prot.infProt.nProt, objDes.Loja, Prot.infProt.chNFe, Model_NotaFiscal.NotaFiscal.Saida);
                        else
                            mNf.AtualizaRetornoNfeSaida(Prot.infProt.cStat, Prot.infProt.nProt, objDes.Loja, Prot.infProt.chNFe, Model_NotaFiscal.NotaFiscal.Entrada);

                        for (int i = 0; i < xNodList.Count; i++)
                        {
                            if(xNodList.Item(i).Name.Equals("protNFe"))
                            {
                                XmlNode xNodListProc = xNodList.Item(i).FirstChild;
                                XmlNodeList xNodListProc2 = xNodListProc.ChildNodes;

                                for (int j = 0; j < xNodListProc2.Count; j++)
                                {
                                    if (xNodListProc2.Item(j).Name.Equals("chNFe"))
                                    {
                                        xProtNFe = xNodList.Item(i).OuterXml;
                                        break;
                                    }
                                }
                            }
                        }
                        mLote.ItemLoteNfe(Prot.infProt.cStat, Prot.infProt.chNFe, xProtNFe);
                        mLote.LoteProcessado(Retorno.cStat, Convert.ToDateTime(Prot.infProt.dhRecbto), Retorno.nRec);
                    }
                }
                else
                {
                    mLote.LoteProcessado(Retorno.cStat, Convert.ToDateTime(Retorno.dhRecbto), Retorno.nRec);
                }

                if (!Directory.Exists(@"C:\NFe\Retorno_Proc_NFe\"))
                {
                    Directory.CreateDirectory(@"C:\NFe\Retorno_Proc_NFe\");
                }
                FileStream FS = new FileStream(@"C:\NFe\Retorno_Proc_NFe\" + string.Format("{0:MMddyyyy}", DateTime.Now.Date) + "_" + Retorno.nRec.Trim() + ".xml", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                SW = new StreamWriter(FS);

                xmlDesSerializar.Serialize(SW, Retorno);
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.RetAutoriz, "Saida", Ex.Message.ToString());

                if (!Directory.Exists(@"C:\NFe\Retorno_Proc_NFe\"))
                {
                    Directory.CreateDirectory(@"C:\NFe\Retorno_Proc_NFe\");
                }
                FileStream FS = new FileStream(@"C:\NFe\Retorno_Proc_NFe\" + string.Format("{0:MMddyyyy}", DateTime.Now.Date) + "_" + Retorno.nRec.Trim() + ".xml", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite);
                SW = new StreamWriter(FS);

                xmlDesSerializar.Serialize(SW, Retorno);
            }
        }
    }
}
