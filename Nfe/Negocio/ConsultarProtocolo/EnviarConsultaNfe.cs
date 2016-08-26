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
using Nfe.Model;
using Nfe.Negocio.Geral;

namespace Nfe.Negocio.ConsultarProtocolo
{
    public class EnviarConsultaNfe : abMetodos, IMetodosBase<Entidade_NotaFiscal>
    {
        Entidade_NotaFiscal eNotaFiscal;
        Model_NotaFiscal mNotaFiscal;
        Model_CCe mCartaEletronica;
        FuncoesGerais fGeral;
        ConsReciNfe.TConsSitNFe SitNfe;
        public void Enviar(Entidade_NotaFiscal ObjEnt, out Entidade_NotaFiscal objDados)
        {
            docTran = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            Settings = new XmlWriterSettings();
            xmlStatus = new XmlSerializer(typeof(ConsReciNfe.TConsSitNFe));

            eNotaFiscal = ObjEnt;
            SitNfe = new ConsReciNfe.TConsSitNFe(ObjEnt);

            Settings.Encoding = UTF8Encoding.UTF8;
            Settings.NewLineHandling = NewLineHandling.None;
            Settings.Indent = true;
            Settings.IndentChars = "";

            ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

            Sw = new UTF8StringWriter();
            Wx = XmlWriter.Create(Sw, Settings);
            xmlStatus.Serialize(Sw, SitNfe, ns);
            string xmlGer = Sw.ToString();

            docTran.LoadXml(xmlGer);
            docTran.PreserveWhitespace = false;

            CertEmpresa = AssinaturaDigital.FindCertOnStore(ObjEnt.Loja);

            EnviarXml(docTran, CertEmpresa, ref eNotaFiscal);

            objDados = null;
        }
        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_NotaFiscal obj)
        {
            object retObj = new object();
            UrlEstados = new UrlNfesEstados();


            ConsultaProtocolo.NfeConsulta2 wConsulta2 = new ConsultaProtocolo.NfeConsulta2();
            ConsultaProtocolo.nfeCabecMsg wCabMsg = new ConsultaProtocolo.nfeCabecMsg();

            wCabMsg.cUF = obj.cUf.ToString();
            wCabMsg.versaoDados = "3.10";

            wConsulta2.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
            wConsulta2.PreAuthenticate = true;
            wConsulta2.ClientCertificates.Add(cert);
            wConsulta2.nfeCabecMsgValue = wCabMsg;

            nodeList = doc.GetElementsByTagName("consSitNFe");
            nodeStatus = nodeList.Item(0);
            wConsulta2.Url = UrlEstados.SetarUrlEstado(UrlEstados.Uf(int.Parse(obj.cUf.ToString())), obj.TpAmbiente == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.ConsultaSitNfe);
            retObj = wConsulta2.nfeConsultaNF2(nodeStatus);
            DeserilizarEvento(retObj, ref obj);
        }
        public void DeserilizarEvento(object obj, ref Entidade_NotaFiscal objDes)
        {
            fGeral = new FuncoesGerais();
            mNotaFiscal = new Model_NotaFiscal();

            mCartaEletronica = new Model_CCe();

            object objRet = new object();

            retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
            xmlDesSerializar = new XmlSerializer(typeof(RetConsSitNFe.TRetConsSitNFe));

            objRet = xmlDesSerializar.Deserialize(retXmlNodeReader);

            var Ret = (RetConsSitNFe.TRetConsSitNFe)objRet;

            if (Ret.protNFe != null)
            {
                if (Ret.protNFe.infProt.cStat == "100")
                {
                    if (objDes.TpNfe == "E")
                        mNotaFiscal.UpdateNfRetornoAutorizado(Convert.ToInt32(Ret.protNFe.infProt.cStat), Ret.protNFe.infProt.nProt, objDes.Loja, objDes.ChaveAcessoNfe, Model_NotaFiscal.NotaFiscal.Entrada);
                    else
                        mNotaFiscal.UpdateNfRetornoAutorizado(Convert.ToInt32(Ret.protNFe.infProt.cStat), Ret.protNFe.infProt.nProt, objDes.Loja, objDes.ChaveAcessoNfe, Model_NotaFiscal.NotaFiscal.Saida);

                    fGeral.TramitacaoNfe(FuncoesGerais.TipoTramitacao.AutorizacaoNFe, eNotaFiscal.Loja, eNotaFiscal.NotaFiscal, eNotaFiscal.sSerieNf, eNotaFiscal.CdFornecedor);
                }
                else if (Ret.protNFe.infProt.cStat == "101" || Ret.protNFe.infProt.cStat == "102")
                {
                    if (objDes.TpNfe == "E")
                        mNotaFiscal.UpdateNfInutilizacaoCancelamento(Convert.ToInt32(Ret.protNFe.infProt.cStat), Ret.protNFe.infProt.nProt, objDes.Loja, objDes.ChaveAcessoNfe, Model_NotaFiscal.NotaFiscal.Entrada);
                    else
                        mNotaFiscal.UpdateNfInutilizacaoCancelamento(Convert.ToInt32(Ret.protNFe.infProt.cStat), Ret.protNFe.infProt.nProt, objDes.Loja, objDes.ChaveAcessoNfe, Model_NotaFiscal.NotaFiscal.Saida);

                    fGeral.TramitacaoNfe(FuncoesGerais.TipoTramitacao.Inutilizacao, eNotaFiscal.Loja, eNotaFiscal.NotaFiscal, eNotaFiscal.sSerieNf, eNotaFiscal.CdFornecedor);
                }
                else
                {
                    if (objDes.TpNfe == "E")
                        mNotaFiscal.UpdateNfRetorno(Convert.ToInt32(Ret.protNFe.infProt.cStat), Ret.protNFe.infProt.nProt, objDes.Loja, objDes.ChaveAcessoNfe, Model_NotaFiscal.NotaFiscal.Entrada);
                    else
                        mNotaFiscal.UpdateNfRetorno(Convert.ToInt32(Ret.protNFe.infProt.cStat), Ret.protNFe.infProt.nProt, objDes.Loja, objDes.ChaveAcessoNfe, Model_NotaFiscal.NotaFiscal.Saida);

                    fGeral.TramitacaoNfe(FuncoesGerais.TipoTramitacao.Indefinido, eNotaFiscal.Loja, eNotaFiscal.NotaFiscal, eNotaFiscal.sSerieNf, eNotaFiscal.CdFornecedor);
                }

                int ddd = Ret.procEventoNFe.Count();
                if(Ret.procEventoNFe.Count() > 0)
                {
                    foreach (var EventoNfe in Ret.procEventoNFe)
                    {
                        mCartaEletronica.UpdateRetornoCCeConsSit(Convert.ToInt32(EventoNfe.evento.infEvento.nSeqEvento), Convert.ToInt32(EventoNfe.retEvento.infEvento.cStat), EventoNfe.retEvento.infEvento.chNFe);
                    }
                }
            }
            
            string cdret = Ret.cStat;
        }
    }
}
