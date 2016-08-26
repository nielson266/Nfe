using Nfe.Negocio.InterfacesAbstratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Nfe.Entidade;
using Nfe.Model;
using System.Xml;
using System.Xml.Serialization;
using Nfe.Negocio.Geral;
using System.Security.Cryptography.X509Certificates;
using System.IO;

namespace Nfe.Negocio.CartaEletronica
{
    public class EnviarCartaEletronica : abMetodos, IMetodosBase<Entidade_CCe>
    {
        CCe.TEvento nEventoCarta;
        Model_CCe mCartaEletronica;
        Entidade_CCe EntCarta;
        int CodUfEmpresa = 0;
        public void Enviar(Entidade_CCe ObjEnt, out Entidade_CCe objDados)
        {

            docTran = new XmlDocument();
            docEnviNfe = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            Settings = new XmlWriterSettings();
            xmlStatus = new XmlSerializer(typeof(CCe.TEvento));
            EntCarta = new Entidade_CCe();

            EntCarta = PesquisarCartaEletronica();

            docEnviNfe.PreserveWhitespace = false;
            docEnviNfe.LoadXml(CabecalhoEvento(EntCarta.Id_CCe_Lote));

            nodeListNfe = docEnviNfe.GetElementsByTagName("envEvento");

            if (EntCarta.ItemCCe != null)
            {
                foreach (var item in EntCarta.ItemCCe)
                {
                    nEventoCarta = new CCe.TEvento(item);

                    Settings.Encoding = UTF8Encoding.UTF8;
                    Settings.NewLineHandling = NewLineHandling.None;
                    Settings.Indent = true;
                    Settings.IndentChars = "";

                    ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

                    Sw = new UTF8StringWriter();
                    Wx = XmlWriter.Create(Sw, Settings);
                    xmlStatus.Serialize(Sw, nEventoCarta, ns);
                    string xmlGer = Sw.ToString();

                    docTran.LoadXml(xmlGer);
                    docTran.PreserveWhitespace = false;

                    CertEmpresa = AssinaturaDigital.FindCertOnStore(item.id_loja);

                    docTran = AssinaturaDigital.SignXml(docTran, CertEmpresa, "infEvento");

                    nodeListCarregarNfe = docTran.GetElementsByTagName("evento", "http://www.portalfiscal.inf.br/nfe");
                    nodeListNfe.Item(0).AppendChild(docEnviNfe.ImportNode(nodeListCarregarNfe.Item(0), true));
                }
                try
                {
                    EnviarXml(docEnviNfe, CertEmpresa, ref EntCarta);
                    objDados = ObjEnt;
                }
                catch (Exception Ex)
                {
                    Mensagem.MensagemErro(Mensagem.TipoMensagem.CartaEletronica, "Saida", Ex.Message.ToString());
                    objDados = null;
                }
            }
            else
                objDados = null;
        }

        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_CCe obj)
        {
            try
            {
                object retObj = new object();
                UrlEstados = new UrlNfesEstados();
                RecepcaoEvento.RecepcaoEvento wRetEvento = new RecepcaoEvento.RecepcaoEvento();
                RecepcaoEvento.nfeCabecMsg wCabMsg = new RecepcaoEvento.nfeCabecMsg();

                wCabMsg.cUF = CodUfEmpresa.ToString();
                wCabMsg.versaoDados = "1.00";

                wRetEvento.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                wRetEvento.PreAuthenticate = true;
                wRetEvento.ClientCertificates.Add(cert);
                wRetEvento.nfeCabecMsgValue = wCabMsg;

                nodeList = doc.GetElementsByTagName("envEvento");
                nodeStatus = nodeList.Item(0);
                wRetEvento.Url = UrlEstados.SetarUrlEstado(UrlEstados.Uf(int.Parse(CodUfEmpresa.ToString())), obj.TipoAmbiente == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.RecepcaoEvento);
                retObj = wRetEvento.nfeRecepcaoEvento(nodeStatus);
                DeserilizarEvento(retObj, ref obj);
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.CartaEletronica, "Saida", Ex.Message.ToString());
            }
        }

        public void DeserilizarEvento(object obj, ref Entidade_CCe objDes)
        {
            object objRet = new object();
            XmlDocument docRet = new XmlDocument();
            mCartaEletronica = new Model_CCe();
            retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
            xmlDesSerializar = new XmlSerializer(typeof(retEnvCCe.TRetEnvEvento));
            int i = 0;

            objRet = xmlDesSerializar.Deserialize(retXmlNodeReader);

            var Retorno = (retEnvCCe.TRetEnvEvento)objRet;

            if (Retorno.retEvento != null)
            {
                foreach (var ret in Retorno.retEvento)
                {
                    if (ret.infEvento.chNFe != null)
                        mCartaEletronica.UpdateRetornoCCe(objDes.Id_CCe_Lote, ret.infEvento.chNFe, Convert.ToInt32(ret.infEvento.cStat), Convert.ToDateTime(ret.infEvento.dhRegEvento));
                    else
                        mCartaEletronica.UpdateRetornoCCe(objDes.Id_CCe_Lote, Convert.ToInt32(ret.infEvento.cStat), EntCarta.ItemCCe[i].TxChAcessoNfe);

                    i += 1;
                }
            }
            else
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.CartaEletronica, "Saida", "Retorno do Evento retornou null - Possivel calsa, NrSeqNf da carta de correção." + Retorno.xMotivo);
            }
        }

        public Entidade_CCe PesquisarCartaEletronica()
        {
            Entidade_CCe ObjEntCarta = new Entidade_CCe();
            Entidade_ItemCCe ObjEntItemCCe = new Entidade_ItemCCe();
            List<Entidade_ItemCCe> ObjListEntItemCCe = new List<Entidade_ItemCCe>();
            mCartaEletronica = new Model_CCe();
            int seqItem = 0;
            var DtCarta = mCartaEletronica.ConsultaCartas();

            for (int i = 0; i < DtCarta.Rows.Count; i++)
            {
                ObjEntCarta = new Entidade_CCe();

                ObjEntCarta.id_loja = Convert.ToInt32(DtCarta.Rows[i]["id_loja"]);
                ObjEntCarta.Id_CCe_Lote = Convert.ToInt32(DtCarta.Rows[i]["Id_cce_lote"]);
                ObjEntCarta.TipoAmbiente = FuncoesGerais.TipoAmbiente();
                ObjEntCarta.cUF = Convert.ToInt32(DtCarta.Rows[i]["cdUfCidadeIbge_Empresa"].ToString().Substring(0,2));
                CodUfEmpresa = Convert.ToInt32(DtCarta.Rows[i]["cdUfCidadeIbge_Empresa"].ToString().Substring(0, 2));

                seqItem = 0;

                for (int j = 0; j < DtCarta.Rows.Count; j++)
                {
                    ObjEntItemCCe = new Entidade_ItemCCe();

                    if (ObjEntCarta.Id_CCe_Lote == Convert.ToInt32(DtCarta.Rows[j]["Id_CCe_Lote"]))
                    {
                        ObjEntItemCCe.id_loja = Convert.ToInt32(DtCarta.Rows[j]["id_loja"]);
                        ObjEntItemCCe.Dt_Prot_Nfe = Convert.ToDateTime(DtCarta.Rows[j]["Dt_Prot_Nfe"]);
                        ObjEntItemCCe.CdUfCidadeIbge_Empresa = Convert.ToInt32(DtCarta.Rows[i]["cdUfCidadeIbge_Empresa"].ToString().Substring(0, 2));
                        ObjEntItemCCe.TxChAcessoNfe = DtCarta.Rows[j]["TxChAcessoNfe"].ToString();
                        ObjEntItemCCe.NrNf = Convert.ToInt32(DtCarta.Rows[j]["NrNf"]);
                        ObjEntItemCCe.serienf = DtCarta.Rows[j]["serienf"].ToString();
                        ObjEntItemCCe.CdCpfCgc = DtCarta.Rows[j]["CdCpfCgc"].ToString();
                        ObjEntItemCCe.Ambiente = DtCarta.Rows[j]["TipoEmissao"].ToString();
                        ObjEntItemCCe.Desc_Correcao = DtCarta.Rows[j]["Desc_Correcao"].ToString();
                        //ObjEntItemCCe.NrSeqEnvio = Convert.ToInt32(DtCarta.Rows[j]["NrSeqEnvio"]);
                        ObjEntItemCCe.NrSeqEnvio = seqItem += 1;
                        ObjListEntItemCCe.Add(ObjEntItemCCe);
                    }
                    else
                        break;
                }

                ObjEntCarta.ItemCCe = ObjListEntItemCCe;
                break;
            }

            return ObjEntCarta;
        }

        string CabecalhoEvento(int nrlote)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><envEvento xmlns=\"http://www.portalfiscal.inf.br/nfe\" versao=\"1.00\"><idLote>" + nrlote + "</idLote></envEvento>";
        }
    }
}
