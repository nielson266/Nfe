using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfe.Entidade;
using Nfe.Model;
using System.Xml.Serialization;
using System.Xml;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using Nfe.Negocio.Geral;
using Nfe.Negocio.InterfacesAbstratos;

namespace Nfe.Negocio.StatusServico
{
    public class EnviarStatus : abMetodos, IMetodosBase<Entidade_Status>
    {
        Model_StatusNfe mStatus;
        NegocioFuncoesGerais nFG = new NegocioFuncoesGerais();
        Entidade_Status eStatus;
        Model_LogNfe mLog;
        NegocioFuncoesGerais NFuncoes;
        ConsStatServ.TConsStatServ cStatus;

        public void Enviar(Entidade_Status ObjEnt, out Entidade_Status objDados)
        {
            Entidade_Status eRetStatus = new Entidade_Status();
            eStatus = new Entidade_Status();
            mLog = new Model_LogNfe();
            NFuncoes = new NegocioFuncoesGerais();

            eStatus = ObjEnt;
            
            docTran = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            Settings = new XmlWriterSettings();
            xmlStatus = new XmlSerializer(typeof(ConsStatServ.TConsStatServ));

            try
            {
                //Passando os dados para a Class que vai ser serelizada
                cStatus = new ConsStatServ.TConsStatServ(eStatus);

                // E DEFINIDO O TIPO DE LEITURA DO XML
                Settings.Encoding = UTF8Encoding.UTF8;
                Settings.NewLineHandling = NewLineHandling.None;
                Settings.Indent = true;
                Settings.IndentChars = "";

                ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

                Sw = new UTF8StringWriter();
                Wx = XmlWriter.Create(Sw, Settings);
                xmlStatus.Serialize(Sw, cStatus, ns);
                string xmlGer = Sw.ToString();

                docTran.LoadXml(xmlGer);
                docTran.PreserveWhitespace = false;

                if (nFG.ValidarEstruturaXml(docTran.OuterXml, "consStatServ_v3.10"))
                {
                    CertEmpresa = AssinaturaDigital.FindCertOnStore(eStatus.Loja);
                    EnviarXml(docTran, CertEmpresa, ref eRetStatus);
                    objDados = eRetStatus;
                }
                else
                    objDados = null;
            }
            catch(Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Nfe, "Saida", Ex.Message.ToString());
                mLog.InsertErroLog(NFuncoes.TiraCampos(Ex.Message.ToString()));
                objDados = null;
            }
        }
        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_Status obj)
        {
            object retObj = new object();
            UrlEstados = new UrlNfesEstados();
            StatuServico.NfeStatusServico2 wStatusServ = new StatuServico.NfeStatusServico2();
            StatuServico.nfeCabecMsg wCabMsg = new StatuServico.nfeCabecMsg();

            wCabMsg.cUF = eStatus.cUf;
            wCabMsg.versaoDados = "3.10";

            wStatusServ.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            wStatusServ.PreAuthenticate = true;
            wStatusServ.ClientCertificates.Add(cert);
            wStatusServ.nfeCabecMsgValue = wCabMsg;

            nodeList = doc.GetElementsByTagName("consStatServ");
            nodeStatus = nodeList.Item(0);
            wStatusServ.Url = UrlEstados.SetarUrlEstado(UrlEstados.Uf(int.Parse(eStatus.cUf)), eStatus.tpAmbiente == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.StatusServico);
            retObj = wStatusServ.nfeStatusServicoNF2(nodeStatus);

            DeserilizarEvento(retObj, ref obj);
        }
        public void DeserilizarEvento(object obj, ref Entidade_Status objDes)
        {
            mStatus = new Model_StatusNfe();
            Entidade_Status eStatusRet = new Entidade_Status();

            object objRet = new object();

            RetConsStatServ.TRetConsStatServ Ret = new RetConsStatServ.TRetConsStatServ();

            retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
            xmlDesSerializar = new XmlSerializer(typeof(RetConsStatServ.TRetConsStatServ));

            objRet = xmlDesSerializar.Deserialize(retXmlNodeReader);

            Ret = (RetConsStatServ.TRetConsStatServ)objRet;

            eStatusRet.cStatus = Ret.cStat;
            eStatusRet.dhRet = Ret.dhRecbto;
            eStatusRet.versao = Ret.versao;
            eStatusRet.sMotivo = Ret.xMotivo;
            eStatusRet.Uf = eStatus.Uf;
            eStatusRet.tpAmbiente = Ret.tpAmb == RetConsStatServ.TAmb.Item1 ? "1" : "2";

            mStatus.Salvar(eStatusRet);

            objDes = eStatusRet;

        }
    }
}
