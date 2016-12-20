using Nfe.Entidade;
using Nfe.Negocio.InterfacesAbstratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;
using System.Xml;
using Nfe.Negocio.Geral;
using Nfe.Model;
using System.Xml.Serialization;
using Nfe.downloadNFe;
using Nfe.retDownloadNFe;

namespace Nfe.Negocio.DownloadNFe
{
    public class EnviarDownloadNFe : abMetodos, IMetodosBase<Entidade_DownloadNFe>
    {
        NegocioFuncoesGerais nFG = new NegocioFuncoesGerais();
        Entidade_DownloadNFe eDownloadNFe;
        Model_LogNfe mLog;
        NegocioFuncoesGerais NFuncoes;

        TDownloadNFe ObjDownloadNFeEnv;

        public void Enviar(Entidade_DownloadNFe ObjEnt, out Entidade_DownloadNFe objDados)
        {
            Entidade_Status eRetStatus = new Entidade_Status();
            eDownloadNFe = new Entidade_DownloadNFe();
            mLog = new Model_LogNfe();
            NFuncoes = new NegocioFuncoesGerais();

            docTran = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            Settings = new XmlWriterSettings();
            xmlStatus = new XmlSerializer(typeof(TDownloadNFe));

            try
            {
                //Passando os dados para a Class que vai ser serelizada
                ObjDownloadNFeEnv = new TDownloadNFe(ObjEnt);

                // E DEFINIDO O TIPO DE LEITURA DO XML
                Settings.Encoding = UTF8Encoding.UTF8;
                Settings.NewLineHandling = NewLineHandling.None;
                Settings.Indent = true;
                Settings.IndentChars = "";

                ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

                Sw = new UTF8StringWriter();
                Wx = XmlWriter.Create(Sw, Settings);
                xmlStatus.Serialize(Sw, ObjDownloadNFeEnv, ns);
                string xmlGer = Sw.ToString();

                docTran.LoadXml(xmlGer);
                docTran.PreserveWhitespace = false;

                CertEmpresa = AssinaturaDigital.FindCertOnStore(ObjEnt.id_loja);
                EnviarXml(docTran, CertEmpresa, ref ObjEnt);
                objDados = null;
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Status, "DownloaNFe", Ex.Message.ToString());
                mLog.InsertErroLog(NFuncoes.TiraCampos(Ex.Message.ToString()));
                objDados = null;
            }
        }

        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_DownloadNFe obj)
        {
            object retObj = new object();
            UrlEstados = new UrlNfesEstados();
            DownloadNFeService.NfeDownloadNF wDownloadEnviar = new DownloadNFeService.NfeDownloadNF();
            DownloadNFeService.nfeCabecMsg wCabMsg = new DownloadNFeService.nfeCabecMsg();


            wCabMsg.cUF = "91";
            wCabMsg.versaoDados = "1.00";

            wDownloadEnviar.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            wDownloadEnviar.PreAuthenticate = true;
            wDownloadEnviar.ClientCertificates.Add(cert);
            wDownloadEnviar.nfeCabecMsgValue = wCabMsg;

            nodeList = doc.GetElementsByTagName("downloadNFe");
            nodeStatus = nodeList.Item(0);
            //wStatusServ.Url = UrlEstados.SetarUrlEstado(UrlEstados.Uf(, eStatus.tpAmbiente == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.StatusServico);
            retObj = wDownloadEnviar.nfeDownloadNF(nodeStatus);

            DeserilizarEvento(retObj, ref obj);
        }
        public void DeserilizarEvento(object obj, ref Entidade_DownloadNFe objDes)
        {

            Model_DownloadNFe ObjDownloadNFe = new Model_DownloadNFe();
            object objRet = new object();

            retDownloadNFe.TRetDownloadNFe Ret = new retDownloadNFe.TRetDownloadNFe();

            retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
            xmlDesSerializar = new XmlSerializer(typeof(retDownloadNFe.TRetDownloadNFe));

            objRet = xmlDesSerializar.Deserialize(retXmlNodeReader);

            Ret = (retDownloadNFe.TRetDownloadNFe)objRet;

            XmlDocument docxmlret = new XmlDocument();

            foreach (var itemProc in Ret.retNFe)
            {
                var RetProc = (TRetDownloadNFeRetNFeProcNFe)itemProc.Item ;
                docxmlret.LoadXml(RetProc.Any.OuterXml);
                objDes.xmlNfe = docxmlret;

                ObjDownloadNFe.IncluirDownloadNFe(objDes.id_loja, itemProc.chNFe, docxmlret.OuterXml);
            }
        }
    }
}
