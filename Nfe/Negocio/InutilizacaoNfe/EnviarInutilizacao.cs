using Nfe.Negocio.InterfacesAbstratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfe.Entidade;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using Nfe.Negocio.Geral;
using Nfe.Model;

namespace Nfe.Negocio.InutilizacaoNfe
{
    public class EnviarInutilizacao : abMetodos, IMetodosBase<Entidade_Inutilizacao>
    {
        Entidade_Inutilizacao eObjInut;
        Model_InutilizacaoNfe mInutlizacaonfe;
        InutNFe.TInutNFe InutilNfe;
        public void Enviar(Entidade_Inutilizacao ObjEnt, out Entidade_Inutilizacao objDados)
        {
            docTran = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            Settings = new XmlWriterSettings();
            xmlStatus = new XmlSerializer(typeof(InutNFe.TInutNFe));
            
            eObjInut = ObjEnt;

            InutilNfe = new InutNFe.TInutNFe(ObjEnt);
            eObjInut.ChaveAcessoNfe = InutilNfe.infInut.Id.Replace("ID", "");
            // E DEFINIDO O TIPO DE LEITURA DO XML
            Settings.Encoding = UTF8Encoding.UTF8;
            Settings.NewLineHandling = NewLineHandling.None;
            Settings.Indent = true;
            Settings.IndentChars = "";

            ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

            Sw = new UTF8StringWriter();
            Wx = XmlWriter.Create(Sw, Settings);
            xmlStatus.Serialize(Sw, InutilNfe, ns);
            string xmlGer = Sw.ToString();

            docTran.LoadXml(xmlGer);
            docTran.PreserveWhitespace = false;


            CertEmpresa = AssinaturaDigital.FindCertOnStore(ObjEnt.Loja);

            EnviarXml(AssinaturaDigital.SignXml(docTran, CertEmpresa, "infInut"), CertEmpresa, ref ObjEnt);

            objDados = ObjEnt;
        }
        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_Inutilizacao obj)
        {
            try
            {
                object retObj = new object();
                UrlEstados = new UrlNfesEstados();
                Inutilizacao.NfeInutilizacao2 wInutilizacao = new Inutilizacao.NfeInutilizacao2();
                Inutilizacao.nfeCabecMsg wCabMsg = new Inutilizacao.nfeCabecMsg();

                wCabMsg.cUF = eObjInut.cUf.ToString();
                wCabMsg.versaoDados = "3.10";

                wInutilizacao.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                wInutilizacao.PreAuthenticate = true;
                wInutilizacao.ClientCertificates.Add(cert);
                wInutilizacao.nfeCabecMsgValue = wCabMsg;

                nodeList = doc.GetElementsByTagName("inutNFe");
                nodeStatus = nodeList.Item(0);
                wInutilizacao.Url = UrlEstados.SetarUrlEstado(UrlEstados.Uf(int.Parse(eObjInut.cUf.ToString())), eObjInut.TpAmbiente == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.Inutilizacao);
                retObj = wInutilizacao.nfeInutilizacaoNF2(nodeStatus);
                DeserilizarEvento(retObj, ref obj);
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Inutilizacao, "Inutilizacao", Ex.Message.ToString());
            }

        }
        public void DeserilizarEvento(object obj, ref Entidade_Inutilizacao objDes)
        {
            mInutlizacaonfe = new Model_InutilizacaoNfe();

            object objRet = new object();

            retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
            xmlDesSerializar = new XmlSerializer(typeof(RetInutNFe.TRetInutNFe));

            objRet = xmlDesSerializar.Deserialize(retXmlNodeReader);

            var Ret = (RetInutNFe.TRetInutNFe)objRet;

            objDes = null;

            eObjInut.CdRetorno = Ret.infInut.cStat;

            mInutlizacaonfe.Salvar(eObjInut);
        }

    }
}
