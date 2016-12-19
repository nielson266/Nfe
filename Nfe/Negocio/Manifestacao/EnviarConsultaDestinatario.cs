using Nfe.Entidade;
using Nfe.Negocio.InterfacesAbstratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Nfe.consNFeDest;
using System.Xml.Serialization;
using Nfe.Negocio.Geral;
using Nfe.Model;

namespace Nfe.Negocio.Manifestacao
{
    public class EnviarConsultaDestinatario : abMetodos, IMetodosBase<Entidade_ConsNFDest>
    {
        NegocioFuncoesGerais NFuncoes;
        TConsNFeDest ObjConDest;
        Model_LogNfe mLog;
        Model_Manisfestacao ObjManifestacaoDest;

        public void Enviar(Entidade_ConsNFDest ObjEnt, out Entidade_ConsNFDest objDados)
        {
            Entidade_ConsNFDest ObjRetManifestacao = new Entidade_ConsNFDest();
            ObjRetManifestacao.id_loja = 1;
            NFuncoes = new NegocioFuncoesGerais();

            docTran = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            Settings = new XmlWriterSettings();
            mLog = new Model_LogNfe();

            xmlStatus = new XmlSerializer(typeof(TConsNFeDest));


            try
            {
                ObjConDest = new TConsNFeDest(1);

                Settings.Encoding = UTF8Encoding.UTF8;
                Settings.NewLineHandling = NewLineHandling.None;
                Settings.Indent = true;
                Settings.IndentChars = "";

                ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

                Sw = new UTF8StringWriter();
                Wx = XmlWriter.Create(Sw, Settings);
                xmlStatus.Serialize(Sw, ObjConDest, ns);
                string xmlGer = Sw.ToString();

                docTran.LoadXml(xmlGer);
                docTran.PreserveWhitespace = false;

                CertEmpresa = AssinaturaDigital.FindCertOnStore(1);
                EnviarXml(docTran, CertEmpresa, ref ObjRetManifestacao);
                objDados = ObjRetManifestacao;
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Manifestacao, "Manifestação", Ex.Message.ToString());
                mLog.InsertErroLog(NFuncoes.TiraCampos(Ex.Message.ToString()));
                objDados = null;
            }
        }
        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_ConsNFDest obj)
        {
            object retObj = new object();
            UrlEstados = new UrlNfesEstados();
            NFeConsultaDest.NFeConsultaDest wNfeConsDest = new NFeConsultaDest.NFeConsultaDest();
            NFeConsultaDest.nfeCabecMsg wCabMsg = new NFeConsultaDest.nfeCabecMsg();

            wCabMsg.cUF = FuncoesGerais.UfIbgeEmpresa(obj.id_loja);
            wCabMsg.versaoDados = "1.01";

            wNfeConsDest.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            wNfeConsDest.PreAuthenticate = true;
            wNfeConsDest.ClientCertificates.Add(cert);
            wNfeConsDest.nfeCabecMsgValue = wCabMsg;

            nodeList = doc.GetElementsByTagName("consNFeDest");
            nodeStatus = nodeList.Item(0);
            //wNfeConsDest.Url = UrlEstados.SetarUrlEstado(UrlEstados.Uf(Convert.ToInt32(FuncoesGerais.UfIbgeEmpresa(obj.id_loja))), FuncoesGerais.TipoAmbiente() == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.RecepcaoEvento);
            retObj = wNfeConsDest.nfeConsultaNFDest(nodeStatus);

            DeserilizarEvento(retObj, ref obj);

        }
        public void DeserilizarEvento(object obj, ref Entidade_ConsNFDest objDes)
        {
            object objRet = new object();

            mLog = new Model_LogNfe();

            Entidade_ConsNFDest ObjDestNf = new Entidade_ConsNFDest();
            Entidade_ItemConsNFDest ObjItemDestNf = new Entidade_ItemConsNFDest();
            List<Entidade_ItemConsNFDest> ObjListItemDestNf = new List<Entidade_ItemConsNFDest>();

            ObjManifestacaoDest = new Model_Manisfestacao();

            retconsNFeDest.TRetConsNFeDest Ret = new retconsNFeDest.TRetConsNFeDest();

            retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
            xmlDesSerializar = new XmlSerializer(typeof(retconsNFeDest.TRetConsNFeDest));

            objRet = xmlDesSerializar.Deserialize(retXmlNodeReader);

            Ret = (retconsNFeDest.TRetConsNFeDest)objRet;

            ObjDestNf.codstatus = Convert.ToInt32(Ret.cStat);
            ObjDestNf.dhresp = Convert.ToDateTime(Ret.dhResp);
            ObjDestNf.indcont = Ret.indCont == retconsNFeDest.TRetConsNFeDestIndCont.Item0 ? "0" : "1";

            if (Convert.ToInt32(Ret.cStat) != 137)
            {
                foreach (var objRetCons in Ret.ret)
                {
                    if (objRetCons.Item.ToString().Contains("TRetConsNFeDestRetResNFe"))
                    {
                        var ObjRetNFe = (retconsNFeDest.TRetConsNFeDestRetResNFe)objRetCons.Item;

                        if (ObjManifestacaoDest.ConsultaChaveNfeExiteConsultaDest(ObjRetNFe.chNFe))
                        {
                            ObjItemDestNf = new Entidade_ItemConsNFDest();
                            ObjItemDestNf.nsu = ObjRetNFe.NSU;
                            ObjItemDestNf.chacessonfe = ObjRetNFe.chNFe;
                            ObjItemDestNf.cnpjcpf = ObjRetNFe.Item;
                            ObjItemDestNf.nomeemitente = ObjRetNFe.xNome;
                            ObjItemDestNf.dtemissao = Convert.ToDateTime(ObjRetNFe.dEmi);
                            ObjItemDestNf.dhautorizacao = Convert.ToDateTime(ObjRetNFe.dhRecbto);
                            ObjItemDestNf.vlnfe = Convert.ToDecimal(ObjRetNFe.vNF.Replace(".", ","));
                            ObjItemDestNf.tpnfe = ObjRetNFe.tpNF == retconsNFeDest.TRetConsNFeDestRetResNFeTpNF.Item0 ? "0" : "1";
                            if (ObjRetNFe.cSitNFe == retconsNFeDest.TRetConsNFeDestRetResNFeCSitNFe.Item1)
                                ObjItemDestNf.sitnfe = "1";
                            else if (ObjRetNFe.cSitNFe == retconsNFeDest.TRetConsNFeDestRetResNFeCSitNFe.Item2)
                                ObjItemDestNf.sitnfe = "2";
                            else
                                ObjItemDestNf.sitnfe = "3";

                            if (ObjRetNFe.cSitConf == retconsNFeDest.TRetConsNFeDestRetResNFeCSitConf.Item0)
                                ObjItemDestNf.sit_manifestacao_dest = "0";
                            else if (ObjRetNFe.cSitConf == retconsNFeDest.TRetConsNFeDestRetResNFeCSitConf.Item1)
                                ObjItemDestNf.sit_manifestacao_dest = "1";
                            else if (ObjRetNFe.cSitConf == retconsNFeDest.TRetConsNFeDestRetResNFeCSitConf.Item2)
                                ObjItemDestNf.sit_manifestacao_dest = "2";
                            else if (ObjRetNFe.cSitConf == retconsNFeDest.TRetConsNFeDestRetResNFeCSitConf.Item3)
                                ObjItemDestNf.sit_manifestacao_dest = "3";
                            else if (ObjRetNFe.cSitConf == retconsNFeDest.TRetConsNFeDestRetResNFeCSitConf.Item4)
                                ObjItemDestNf.sit_manifestacao_dest = "4";

                            ObjListItemDestNf.Add(ObjItemDestNf);
                        }
                    }
                }
            }

            ObjDestNf.ListItemConsDest = ObjListItemDestNf;

            try
            {
                if(Convert.ToInt32(Ret.cStat) != 137)
                {
                    if (ObjDestNf.ListItemConsDest.Count > 0)
                    {
                        ObjManifestacaoDest.InsertConsNFDest(ObjDestNf);
                    }
                }
            }
            catch (Exception Ex)
            {
                mLog.InsertErroLog(NFuncoes.TiraCampos(Ex.Message.ToString()));
            }
        }
    }
}
