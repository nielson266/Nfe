using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

using Nfe.envEventoCancNFe;
using Nfe.retEnvEventoCancNFe;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Data;

// CLASSES
using Nfe.Model;
using Nfe.Negocio.Geral;
using System.Xml.Serialization;
using Nfe.Negocio.InterfacesAbstratos;
using Nfe.Entidade;
using Nfe.RecepcaoEvento;

namespace Nfe.Negocio.Cancelamento
{
    public class EnviarCancelamentoSefaz : abMetodos, IMetodosBase<Entidade_Cancelamento>
    {
        TRetEnvEvento RecebEnvento;
        Model_Cancelamento mCancelamento;
        public void Enviar(Entidade_Cancelamento ObjEnt, out Entidade_Cancelamento objDados)
        {
            docTran = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            Settings = new XmlWriterSettings();

            xmlStatus = new XmlSerializer(typeof(TEnvEvento));


            TEnvEvento cancelamentoEvento = new TEnvEvento(ObjEnt.id, Model.FuncoesGerais.TipoAmbiente(), ObjEnt);            

            Settings.Encoding = UTF8Encoding.UTF8;
            Settings.NewLineHandling = NewLineHandling.None;
            Settings.Indent = true;
            Settings.IndentChars = "";

            ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

            Sw = new UTF8StringWriter();
            Wx = XmlWriter.Create(Sw, Settings);
            xmlStatus.Serialize(Sw, cancelamentoEvento, ns);
            string xmlGer = Sw.ToString();

            docTran.LoadXml(xmlGer);
            docTran.PreserveWhitespace = false;

            CertEmpresa = AssinaturaDigital.FindCertOnStore(ObjEnt.Loja);
            try
            {

                EnviarXml(AssinaturaDigital.SignXml(docTran, CertEmpresa, "infEvento"), CertEmpresa, ref ObjEnt);

                objDados = ObjEnt;
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Cancelamento, "Cancelamento", Ex.Message.ToString());
                objDados = null;
            }
        }
        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_Cancelamento obj)
        {
            try
            {
                object retObj = new object();
                UrlEstados = new UrlNfesEstados();

                RecepcaoEvento.RecepcaoEvento RecepEvCanc = new RecepcaoEvento.RecepcaoEvento();
                RecepcaoEvento.nfeCabecMsg wCabMsg = new nfeCabecMsg();

                wCabMsg.cUF = obj.cUf.ToString();
                wCabMsg.versaoDados = "1.00";

                RecepEvCanc.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                RecepEvCanc.PreAuthenticate = true;
                RecepEvCanc.ClientCertificates.Add(cert);
                RecepEvCanc.nfeCabecMsgValue = wCabMsg;

                nodeList = doc.GetElementsByTagName("envEvento");
                nodeStatus = nodeList.Item(0);
                RecepEvCanc.Url = UrlEstados.SetarUrlEstado(UrlEstados.Uf(int.Parse(obj.cUf.ToString())), obj.TpAmb == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.RecepcaoEvento);
                retObj = RecepEvCanc.nfeRecepcaoEvento(nodeStatus);
                DeserilizarEvento(retObj, ref obj);
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.RetAutoriz, "Saida", Ex.Message.ToString());
            }
        }
        public void DeserilizarEvento(object obj, ref Entidade_Cancelamento objDes)
        {
            Entidade_Cancelamento EntCa = new Entidade_Cancelamento();
            Model_NotaFiscal mNf = new Model_NotaFiscal();
            Model_Lote mLote = new Model_Lote();
            XmlDocument docRet = new XmlDocument();
            RecebEnvento = new TRetEnvEvento();
            StreamWriter SW;
            string xProtNFe = string.Empty;

            EntCa = objDes;

            
            object retCancelamentoSefaz = new object();

            mCancelamento = new Model_Cancelamento();

            TRetEnvEvento EventoRetCancalentoSefaz = new TRetEnvEvento();
            
            try
            {
                XmlNodeReader retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
                XmlSerializer xmlDesSerializar = new XmlSerializer(typeof(retEnvEventoCancNFe.TRetEnvEvento));

                retCancelamentoSefaz = xmlDesSerializar.Deserialize(retXmlNodeReader);

                EventoRetCancalentoSefaz = (TRetEnvEvento)retCancelamentoSefaz;

                if (EventoRetCancalentoSefaz.cStat != "410")
                {
                    foreach (var item in EventoRetCancalentoSefaz.retEvento)
                    {
                        if (item.infEvento.cStat.Trim() == "135" || item.infEvento.cStat.Trim() == "136" || item.infEvento.cStat.Trim() == "155")
                        {
                            mCancelamento.UpdateCancelamentoNfe(101, Convert.ToDateTime(item.infEvento.dhRegEvento), EntCa.Loja, EntCa.NmSerie, EntCa.NrNf.ToString(), 0);

                            if (EntCa.TpNf == "S")
                                mCancelamento.UpdateNfSaidaCancelamento(101, item.infEvento.nProt, EntCa.Loja, EntCa.NmSerie.Trim(), EntCa.NrNf.ToString());
                            else
                                mCancelamento.UpdateNfEntradaCancelamento(101, item.infEvento.nProt, EntCa.Loja, EntCa.NmSerie.Trim(), EntCa.NrNf.ToString(), EntCa.CdFornec);

                            mCancelamento.TramitacaoNfe(EntCa.Loja, EntCa.NmSerie.Trim(), EntCa.NrNf.ToString(), EntCa.CdFornec, 0, "Cancelamento autorizado");
                        }
                        else
                        {
                            mCancelamento.UpdateCancelamentoNfe(int.Parse(item.infEvento.cStat), Convert.ToDateTime(item.infEvento.dhRegEvento), EntCa.Loja, EntCa.NmSerie.Trim() == "9" ? "D1" : EntCa.NmSerie.Trim(), EntCa.NrNf.ToString(), EntCa.CdFornec);
                            mCancelamento.TramitacaoNfe(EntCa.Loja, EntCa.NmSerie.Trim() == "9" ? "D1" : EntCa.NmSerie.Trim(), EntCa.NrNf.ToString(), EntCa.CdFornec, 0, "Cancelamento não autorizado! Erro:" + int.Parse(item.infEvento.cStat));
                        }
                    }
                }
                else
                {
                    vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoNotaFiscal.txt", true);
                    vWriter.WriteLine("=====================CANCELAMENTO - " + string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now) + "================================");
                    vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + EventoRetCancalentoSefaz.xMotivo);
                    vWriter.WriteLine("=====================================================");
                    vWriter.Flush();
                    vWriter.Close();
                }
            }
            catch (Exception ex)
            {
                vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoNotaFiscal.txt", true);
                vWriter.WriteLine("=====================CANCELAMENTO - "+ string.Format("{0:dd/MM/yyyy HH:mm:ss}", DateTime.Now) +"================================");
                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
                vWriter.WriteLine("=====================================================");
                vWriter.Flush();
                vWriter.Close();
            }
        }

        #region MONTANDO O XML PARA CANCELAMENTO
//        public void CancelamentoNfeSolicitadadas(DataRow rowNotasSolicitadas)
//        {
//            try
//            {
//                docTran = new XmlDocument();
//                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();

//                XmlWriterSettings Settings = new XmlWriterSettings();

//                XmlSerializer xmlCancelamento = new XmlSerializer(typeof(envEventoCancNFe.TEnvEvento));

//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine("ENTRANDO NO TEnvEvento");
//                vWriter.Flush();
//                vWriter.Close();

//                TEnvEvento cancelamentoEvento = new TEnvEvento(1, Model.FuncoesGerais.TipoAmbiente(), rowNotasSolicitadas);

//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine("SAINDO NO TEnvEvento");
//                vWriter.Flush();
//                vWriter.Close();

//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine(" iniciando Serialize - " + DateTime.Now);
//                vWriter.Flush();
//                vWriter.Close();


//                // E DEFINIDO O TIPO DE LEITURA DO XML
//                Settings.Encoding = UTF8Encoding.UTF8;
//                Settings.NewLineHandling = NewLineHandling.None;
//                Settings.Indent = true;
//                Settings.IndentChars = "";

//                // E DEFINIDO NAMESPACE
//                ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

//                UTF8StringWriter Sw = new UTF8StringWriter();
//                XmlWriter wx = XmlWriter.Create(Sw, Settings);
//                xmlCancelamento.Serialize(Sw, cancelamentoEvento, ns);
//                string xmlGer = Sw.ToString();

//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine("finalizando Serialize - " + DateTime.Now);
//                vWriter.Flush();
//                vWriter.Close();

//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine("iniciando o load");
//                vWriter.Flush();
//                vWriter.Close();

//                docTran.LoadXml(xmlGer);

//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine("finalizando o load");
//                vWriter.Flush();
//                vWriter.Close();
////                docTran.LoadXml(xmlGer);

//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine("LOAD DO DOCUMENT");
//                vWriter.Flush();
//                vWriter.Close();

//                docTran.Save(@"C:\Nota_Fiscal_Eletronica\1\Cancelamento-NotaFiscal-" + rowNotasSolicitadas["serienf"].ToString().Trim() + "-" + rowNotasSolicitadas["NrNf"].ToString() + ".xml");

//                docTran.PreserveWhitespace = false;

//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine("PEGANDO O CERTIFICADO");
//                vWriter.Flush();
//                vWriter.Close();

//                CertEmpresa = AssinaturaDigital.FindCertOnStore();

//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine("CERTIFICADO OK");
//                vWriter.Flush();
//                vWriter.Close();

//                if (CertEmpresa != null)
//                    EnviaCancelamento(AssinaturaDigital.SignXml(docTran, CertEmpresa, "infEvento"), rowNotasSolicitadas);
//                else
//                {
//                    vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                    vWriter.WriteLine("Existe um problema com certificado digital.");
//                    vWriter.Flush();
//                    vWriter.Close();
//                }
//            }
//            catch (Exception ex)
//            {
//                vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
//                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString() + ex.Source + ex.StackTrace + ex.InnerException.Message == null ? "Nada" : ex.InnerException.Message);
//                vWriter.Flush();
//                vWriter.Close();
//            }
       // }
        #endregion

        #region Enviando Cancelamento
        //void EnviaCancelamento(XmlDocument envCancelemento, DataRow rowNotasFiscais)
        //{
        //    object retCancelamento;
        //    XmlNodeList nodeListenvCancelamento;
        //    XmlNode nodeenvCancelamento;

        //    try
        //    {

        //        vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
        //        vWriter.WriteLine("ENVIADO CANCELAMENTO");
        //        vWriter.Flush();
        //        vWriter.Close();

        //        docTran.Save(@"C:\Nota_Fiscal_Eletronica\1\Pedido Cancelamento\Cancelamento-NotaFiscal-" + rowNotasFiscais["serienf"].ToString().Trim() + "-" + rowNotasFiscais["NrNf"].ToString() + "-Assinada.xml");

        //        Nfe.nfeh.RecepcaoEvento.sefaz.ce.gov.br.RecepcaoEvento Ret = new Nfe.nfeh.RecepcaoEvento.sefaz.ce.gov.br.RecepcaoEvento();
        //        Nfe.nfeh.RecepcaoEvento.sefaz.ce.gov.br.nfeCabecMsg nfCabMsgCancelamento = new Nfe.nfeh.RecepcaoEvento.sefaz.ce.gov.br.nfeCabecMsg();

        //        // CABEÇALHO DO EVENTO

        //        nfCabMsgCancelamento.cUF = Model.FuncoesGerais.UfIbgeEmpresa();
        //        nfCabMsgCancelamento.versaoDados = "1.00";

        //        Ret.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
        //        Ret.PreAuthenticate = true;
        //        Ret.ClientCertificates.Add(CertEmpresa);
        //        Ret.nfeCabecMsgValue = nfCabMsgCancelamento;

        //        nodeListenvCancelamento = envCancelemento.GetElementsByTagName("envEvento");

        //        vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
        //        vWriter.WriteLine("COLOCADO O NO");
        //        vWriter.Flush();
        //        vWriter.Close();
        //        nodeenvCancelamento = nodeListenvCancelamento.Item(0);

        //        vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
        //        vWriter.WriteLine("ENVIADO PARA URL");
        //        vWriter.Flush();
        //        vWriter.Close();
        //        retCancelamento = Ret.nfeRecepcaoEvento(nodeenvCancelamento);

        //        vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
        //        vWriter.WriteLine("RESPOSTA OK");
        //        vWriter.Flush();
        //        vWriter.Close();

        //        ReturnsProcessamentoSefaz(retCancelamento, rowNotasFiscais);
        //    }
        //    catch (Exception ex)
        //    {
        //        vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
        //        vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
        //        vWriter.Flush();
        //        vWriter.Close();
        //    }
        //}
        #endregion

        #region RETORNO DE PROCESSAMENTO DE CANCELAMENTO
        //public void ReturnsProcessamentoSefaz(object Returns, DataRow rowRetornoNotaFiscal)
        //{
        //    object retCancelamentoSefaz = new object();

        //    mCancelamento = new Model_Cancelamento();

        //    TRetEnvEvento EventoRetCancalentoSefaz = new TRetEnvEvento();

        //    try
        //    {
        //        XmlNodeReader retXmlNodeReader = new XmlNodeReader((XmlNode)Returns);
        //        XmlSerializer xmlDesSerializar = new XmlSerializer(typeof(retEventoCancNFe.TRetEnvEvento));

        //        retCancelamentoSefaz = xmlDesSerializar.Deserialize(retXmlNodeReader);

        //        EventoRetCancalentoSefaz = (TRetEnvEvento)retCancelamentoSefaz;

        //        if (EventoRetCancalentoSefaz.cStat != "410")
        //        {
        //            foreach (var item in EventoRetCancalentoSefaz.retEvento)
        //            {
        //                if (item.infEvento.cStat.Trim() == "135" || item.infEvento.cStat.Trim() == "136" || item.infEvento.cStat.Trim() == "155")
        //                {
        //                    mCancelamento.UpdateCancelamentoNfe(101, Convert.ToDateTime(item.infEvento.dhRegEvento), int.Parse(rowRetornoNotaFiscal["id_loja"].ToString()), rowRetornoNotaFiscal["serienf"].ToString(), rowRetornoNotaFiscal["NrNf"].ToString(), 0);

        //                    if (rowRetornoNotaFiscal["TpNfe"].ToString() == "S")
        //                        mCancelamento.UpdateNfSaidaCancelamento(101, item.infEvento.nProt, int.Parse(rowRetornoNotaFiscal["id_loja"].ToString()), rowRetornoNotaFiscal["serienf"].ToString().Trim(), rowRetornoNotaFiscal["NrNf"].ToString());
        //                    else
        //                        mCancelamento.UpdateNfEntradaCancelamento(101, item.infEvento.nProt, int.Parse(rowRetornoNotaFiscal["id_loja"].ToString()), rowRetornoNotaFiscal["serienf"].ToString().Trim(), rowRetornoNotaFiscal["NrNf"].ToString(), int.Parse(rowRetornoNotaFiscal["CdFornec"].ToString()));

        //                    mCancelamento.TramitacaoNfe(int.Parse(rowRetornoNotaFiscal["id_loja"].ToString().Trim()), rowRetornoNotaFiscal["serienf"].ToString().Trim(), rowRetornoNotaFiscal["NrNf"].ToString().Trim(), int.Parse(rowRetornoNotaFiscal["CdFornec"].ToString()), 0, "Cancelamento autorizado");
        //                }
        //                else
        //                {
        //                    mCancelamento.UpdateCancelamentoNfe(int.Parse(item.infEvento.cStat), Convert.ToDateTime(item.infEvento.dhRegEvento), int.Parse(rowRetornoNotaFiscal["id_loja"].ToString()), rowRetornoNotaFiscal["serienf"].ToString(), rowRetornoNotaFiscal["NrNf"].ToString(), 0);
        //                    mCancelamento.TramitacaoNfe(int.Parse(rowRetornoNotaFiscal["id_loja"].ToString().Trim()), rowRetornoNotaFiscal["serienf"].ToString().Trim(), rowRetornoNotaFiscal["NrNf"].ToString(), int.Parse(rowRetornoNotaFiscal["CdFornec"].ToString()), 0, "Cancelamento não autorizado! Erro:" + int.Parse(item.infEvento.cStat));
        //                }
        //            }
        //        }
        //        else
        //        {
        //            vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
        //            vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + EventoRetCancalentoSefaz.xMotivo);
        //            vWriter.Flush();
        //            vWriter.Close();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
        //        vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
        //        vWriter.Flush();
        //        vWriter.Close();
        //    }
        //}
        #endregion

        //public void ConsultaNotasFiscaisParaCancelar()
        //{
        //    Model_Cancelamento objCancelameto = new Model_Cancelamento();

        //    var DtResult = objCancelameto.ConsultarCancelementosSolicitados();

        //    foreach (DataRow row in DtResult.Rows)
        //    {
        //        CancelamentoNfeSolicitadadas(row);
        //    }

        //}
    }
}
