using Nfe.Negocio.InterfacesAbstratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Nfe.Entidade;
using Nfe.envConfRecebto;
using System.Xml.Serialization;
using Nfe.Model;
using Nfe.Negocio.Geral;

namespace Nfe.Negocio.Manifestacao
{
    public class EnviarManifestacao : abMetodos, IMetodosBase<Entidade_Manifestacao>
    {
        TEnvEvento envManifestacao;
        Entidade_Manifestacao EntManifestacao;
        Model_Manisfestacao ObjModelManifestacao;
        public void Enviar(Entidade_Manifestacao ObjEnt, out Entidade_Manifestacao objDados)
        {

            docTran = new XmlDocument();
            docEnviNfe = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            Settings = new XmlWriterSettings();
            xmlStatus = new XmlSerializer(typeof(TEnvEvento));
            EntManifestacao = new Entidade_Manifestacao();

            //EntManifestacao = PesquisaDados();
            envManifestacao = new TEnvEvento(ObjEnt);
            objDados = null;

            Settings.Encoding = UTF8Encoding.UTF8;
            Settings.NewLineHandling = NewLineHandling.None;
            Settings.Indent = true;
            Settings.IndentChars = "";

            ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

            try
            {
                Sw = new UTF8StringWriter();
                Wx = XmlWriter.Create(Sw, Settings);
                xmlStatus.Serialize(Sw, envManifestacao, ns);
                string xmlGer = Sw.ToString();

                docTran.LoadXml(xmlGer);
                docTran.PreserveWhitespace = false;

                CertEmpresa = AssinaturaDigital.FindCertOnStore(1);

                EnviarXml(AssinaturaDigital.SignXml(docTran, CertEmpresa, "infEvento"), CertEmpresa, ref ObjEnt);

                objDados = ObjEnt;
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Cancelamento, "Envia Manifestação", Ex.Message.ToString());
                objDados = null;
            }
        }

        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_Manifestacao obj)
        {
            try
            {
                object retObj = new object();
                UrlEstados = new UrlNfesEstados();

                RecepcaoEvento.RecepcaoEvento RecepEvCanc = new RecepcaoEvento.RecepcaoEvento();
                RecepcaoEvento.nfeCabecMsg wCabMsg = new RecepcaoEvento.nfeCabecMsg();

                wCabMsg.cUF = "91";//FuncoesGerais.UfIbgeEmpresa(obj.id_loja);
                wCabMsg.versaoDados = "1.00";

                RecepEvCanc.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                RecepEvCanc.PreAuthenticate = true;
                RecepEvCanc.ClientCertificates.Add(cert);
                RecepEvCanc.nfeCabecMsgValue = wCabMsg;

                nodeList = doc.GetElementsByTagName("envEvento");
                nodeStatus = nodeList.Item(0);
                RecepEvCanc.Url = UrlEstados.SetarUrlEstado(UrlNfesEstados.Estado.AN, FuncoesGerais.TipoAmbiente() == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.RecepcaoEvento);
                retObj = RecepEvCanc.nfeRecepcaoEvento(nodeStatus);
                DeserilizarEvento(retObj, ref obj);
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Manifestacao, "Saida", Ex.Message.ToString());
            }
        }

        public void DeserilizarEvento(object obj, ref Entidade_Manifestacao objDes)
        {
            object objRet = new object();
            XmlDocument docRet = new XmlDocument();
            ObjModelManifestacao = new Model_Manisfestacao();
            retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
            xmlDesSerializar = new XmlSerializer(typeof(retEnvConfRecebto.TRetEnvEvento));
            int i = 0;

            objRet = xmlDesSerializar.Deserialize(retXmlNodeReader);

            var Retorno = (retEnvConfRecebto.TRetEnvEvento)objRet;

            if (Retorno.retEvento != null)
            {
                foreach (var ret in Retorno.retEvento)
                {
                    if (ret.infEvento.chNFe != null)
                        ObjModelManifestacao.UpdateManifestacao(objDes.ListNfeManifestacao[i].id, Convert.ToInt32(ret.infEvento.cStat), ret.infEvento.nProt, Convert.ToDateTime(ret.infEvento.dhRegEvento));
                    else
                        ObjModelManifestacao.UpdateManifestacao(objDes.ListNfeManifestacao[i].id, Convert.ToInt32(ret.infEvento.cStat), string.Empty, DateTime.Now);

                    i += 1;
                }
            }
            else
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Manifestacao, "ManifestacaoNFe", "Retorno do Evento retornou null - Possivel causa, NrSeqNf da carta de correção." + Retorno.xMotivo);
            }
        }

        public Entidade_Manifestacao PesquisaDados()
        {
            ObjModelManifestacao = new Model_Manisfestacao();

            Entidade_Manifestacao ObjEntMan = new Entidade_Manifestacao();
            Entidade_ItemManifestacao ObjItemManifestacao;

            List<Entidade_ItemManifestacao> ListItemManifestacao = new List<Entidade_ItemManifestacao>(); ;

            var DtManifestacao = ObjModelManifestacao.ConsultaManifestacao();


            if (DtManifestacao.Rows.Count > 0)
            {
                ObjEntMan = new Entidade_Manifestacao();
                ObjEntMan.id = Convert.ToInt32(DtManifestacao.Rows[0]["id"]);
                ObjEntMan.dtManifestacao = Convert.ToDateTime(DtManifestacao.Rows[0]["dtmanifestacao"]);

                for (int i = 0; i < DtManifestacao.Rows.Count; i++)
                {
                    ObjItemManifestacao = new Entidade_ItemManifestacao();
                    ObjItemManifestacao.idseq = Convert.ToInt32(DtManifestacao.Rows[i]["idseq"]);
                    ObjItemManifestacao.chaveacesso = DtManifestacao.Rows[i]["txchacessonfe"].ToString();
                    ObjItemManifestacao.codmanifestacao = Convert.ToInt32(DtManifestacao.Rows[i]["codmanifestacao"]);
                    ListItemManifestacao.Add(ObjItemManifestacao);
                }

                ObjEntMan.ListNfeManifestacao = ListItemManifestacao;
            }

            return ObjEntMan;
        }

        string CabecalhoEvento(int nrlote)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?><envEvento xmlns=\"http://www.portalfiscal.inf.br/nfe\" versao=\"1.00\"><idLote>" + nrlote + "</idLote></envEvento>";
        }
    }
}
