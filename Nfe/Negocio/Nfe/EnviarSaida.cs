using Nfe.Negocio.InterfacesAbstratos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfe.Entidade;
using Nfe.Model;
using System.Xml;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using System.Data;
using Nfe.Negocio.Geral;
using Nfe.retEnviNFe;
using System.IO;

namespace Nfe.Negocio.Nfe
{
    public class EnviarSaida : abMetodos, IMetodosBase<Entidade_NotaFiscal>
    {
        Entidade_ItemNotaFiscal ObjItemNotaFiscal;
        Entidade_Duplicatas ObjDuplicatas;
        Entidade_LocalEntrega ObjLocalEntrega;
        Entidade_Transportador ObjTrans;
        Entidade_Totais ObjTotais;
        Entidade_Emitente ObjEntEmit;
        Entidade_NotaFiscalReferida ObjNotaFiscalReferida;
        List<Entidade_NotaFiscalReferida> ListObjNotaFiscalReferida;
        Entidade_Destinatario ObjEntDest;
        Entidade_NotaFiscal ObjNotaFiscal;
        //Models
        Model_NotaFiscal ObjModNfe;
        Model_LogNfe mLog;
        Model_Lote ObjLote;
        FuncoesGerais ObjFuncGerais;
        nfe.TNFe Nfe;

        NegocioFuncoesGerais NFuncoes;

        string[] CfopDev = new string[] { "1201", "1202", "1203", "1204", "1208", "1209", "1410", "1411", "1503", "1504", "1505", "1506", "1553", "1660", "1661", "1662", "1918", "1919", "2201", "2202", "2203", "2204", "2208", "2209", "2410", "2411", "2503", "2504", "2505", "2506", "2553", "2660", "2661", "2662", "2918", "2919", "3201", "3202", "3211", "3503", "3553", "5201", "5202", "5208", "5209", "5210", "5410", "5411", "5412", "5413", "5503", "5553", "5555", "5556", "5660", "5661", "5662", "5918", "5919", "6201", "6202", "6208", "6209", "6210", "6410", "6411", "6412", "6413", "6503", "6553", "6555", "6556", "6660", "6661", "6662", "6918", "6919", "7201", "7202", "7210", "7211", "7553", "7556" };

        Dictionary<string, decimal> ListAliqUfInterestadual = new Dictionary<string, decimal>();

        public void Enviar(Entidade_NotaFiscal ObjEnt, out Entidade_NotaFiscal objDados)
        {
            //var d = CfopDev.Contains("5556");

            docTran = new XmlDocument();
            ns = new XmlSerializerNamespaces();
            xmlStatus = new XmlSerializer(typeof(nfe.TNFe));
            Settings = new XmlWriterSettings();
            DataTable DtLoteNfe = new DataTable();
            mLog = new Model_LogNfe();
            ObjLote = new Model_Lote();
            ObjModNfe = new Model_NotaFiscal();
            ObjNotaFiscal = ObjEnt;
            NFuncoes = new NegocioFuncoesGerais();

            ObjNotaFiscal = new Entidade_NotaFiscal();

            CarregaAliqInterestadual();

            var Lojas = FuncoesGerais.LojasEmitentes();

            for (int i = 0; i < Lojas.Rows.Count; i++)
            {
                int Loja = 0;
                int Nota = 0;
                string Serie = "";
                int TpEmis = 0;
                var Lote = GerarLoteNfe(Convert.ToInt32(Lojas.Rows[i]["idloja"].ToString()));
                docEnviNfe = new XmlDocument();
                if (Lote != 0)
                {
                    docEnviNfe.PreserveWhitespace = false;
                    docEnviNfe.LoadXml(CabecalhoEnvieNfe(Lote));

                    nodeListNfe = docEnviNfe.GetElementsByTagName("enviNFe");

                    DtLoteNfe = PesquisaNotasFiscaisLoteNfe(Convert.ToInt32(Lojas.Rows[i]["idloja"].ToString()), Lote);

                    foreach (DataRow RowNFDt in DtLoteNfe.Rows)
                    {
                        try
                        {
                            docTran = new XmlDocument();

                            var RetEnt = CarregarDados(Convert.ToInt32(RowNFDt["id_loja"]), Convert.ToInt32(RowNFDt["NrNf"]), RowNFDt["serienf"].ToString(), RowNFDt["TpNFe"].ToString());
                            if (RetEnt != null)
                            {
                                ObjNotaFiscal.cUf = RetEnt.cUf; // Uf do Emitente
                                ObjNotaFiscal.Loja = RetEnt.Loja;
                                Loja = RetEnt.Loja;
                                Nota = RetEnt.NotaFiscal;
                                Serie = RetEnt.sSerieNf;
                                TpEmis = RetEnt.TpEmis;
                                ObjNotaFiscal.Lote = Lote;

                                Nfe = new nfe.TNFe(RetEnt);

                                // E DEFINIDO O TIPO DE LEITURA DO XML
                                Settings.Encoding = UTF8Encoding.UTF8;
                                Settings.NewLineHandling = NewLineHandling.None;
                                Settings.Indent = true;
                                Settings.IndentChars = "";
                                ns.Add(string.Empty, "http://www.portalfiscal.inf.br/nfe");

                                Sw = new UTF8StringWriter();
                                Wx = XmlWriter.Create(Sw, Settings);
                                xmlStatus.Serialize(Sw, Nfe, ns);
                                string xmlGer = Sw.ToString();

                                docTran.LoadXml(xmlGer);
                                docTran.PreserveWhitespace = false;

                                CertEmpresa = AssinaturaDigital.FindCertOnStore(Convert.ToInt32(RowNFDt["id_loja"]));

                                docTran = AssinaturaDigital.SignXml(docTran, CertEmpresa, "infNFe");

                                if (NFuncoes.ValidarEstruturaXml(docTran.OuterXml, "nfe_v3.10"))
                                {
                                    Mensagem.MensagemErro(Mensagem.TipoMensagem.XmlLoteGerados, "Saida", Lote.ToString() + "|" + "Nota :" + RetEnt.sSerieNf == "9" ? "D1" : RetEnt.sSerieNf + "-" + RetEnt.NotaFiscal.ToString() + "|" + docTran.OuterXml);
                                    nodeListCarregarNfe = docTran.GetElementsByTagName("NFe", "http://www.portalfiscal.inf.br/nfe");
                                    nodeListNfe.Item(0).AppendChild(docEnviNfe.ImportNode(nodeListCarregarNfe.Item(0), true));
                                    ObjLote.UpdateXmlTpSaida(RetEnt.Loja, RetEnt.NotaFiscal, RetEnt.sSerieNf == "9" ? "D1" : RetEnt.sSerieNf, Lote, RetEnt.TpEmis.ToString(), docTran.OuterXml);
                                    ObjModNfe.UpdateNfTpEmisVersaoNFe(RetEnt.TpEmis.ToString(), null, "3.10", Lote.ToString(), RetEnt.NotaFiscal.ToString(), RetEnt.Loja.ToString(), RetEnt.sSerieNf == "9" ? "D1" : RetEnt.sSerieNf, 0, Model_NotaFiscal.NotaFiscal.Saida);
                                }
                            }
                        }
                        catch (Exception Ex)
                        {
                            try
                            {
                                Mensagem.MensagemErro(Mensagem.TipoMensagem.Nfe, "Saida", Ex.Message.ToString());
                                ObjLote.UpdateXmlTpSaida(Loja, Nota, Serie == "9" ? "D1" : Serie, Lote, TpEmis.ToString(), docTran.OuterXml);
                                ObjModNfe.UpdateNfErro(225, Loja, Serie == "9" ? "D1" : Serie, Nota, 0, Model_NotaFiscal.NotaFiscal.Saida);
                                ObjLote.UpdateLoteErro(225, Loja, Serie == "9" ? "D1" : Serie, Nota);
                                mLog.InsertErroLog("Erro Lote:" + Lote + ", Nota Fiscal:" + Serie == "9" ? "D1" : Serie + "-" + Nota + ". " + NFuncoes.TiraCampos(Ex.Message.ToString()));
                            }
                            catch
                            {
                                mLog.InsertErroLog("Erro Lote:" + Lote + ", Nota Fiscal:" + Serie == "9" ? "D1" : Serie + "-" + Nota + ". " + NFuncoes.TiraCampos(Ex.Message.ToString()));
                            }
                        }
                    }

                    if (CertEmpresa != null)
                    {
                        ObjNotaFiscal.TpAmbiente = FuncoesGerais.TipoAmbiente();
                        EnviarXml(docEnviNfe, CertEmpresa, ref ObjNotaFiscal);
                    }
                    objDados = null;
                }
                else
                    objDados = null;
            }

            objDados = null;
        }
        public void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref Entidade_NotaFiscal obj)
        {
            object retObj = new object();
            UrlEstados = new UrlNfesEstados();
            Autorizacao.NfeAutorizacao wNfeAut = new Autorizacao.NfeAutorizacao();
            Autorizacao.nfeCabecMsg wCabMsg = new Autorizacao.nfeCabecMsg();
            mLog = new Model_LogNfe();

            try
            {
                wCabMsg.cUF = obj.cUf.ToString();
                wCabMsg.versaoDados = "3.10";

                wNfeAut.SoapVersion = System.Web.Services.Protocols.SoapProtocolVersion.Soap12;
                wNfeAut.PreAuthenticate = true;
                wNfeAut.ClientCertificates.Add(cert);
                wNfeAut.nfeCabecMsgValue = wCabMsg;

                nodeList = doc.GetElementsByTagName("enviNFe");
                nodeStatus = nodeList.Item(0);

                wNfeAut.Url = UrlEstados.SetarUrlEstado(UrlEstados.Uf(int.Parse(obj.cUf.ToString())), obj.TpAmbiente == "HOM" ? UrlNfesEstados.tbAmbiente.HOM : UrlNfesEstados.tbAmbiente.PROD, UrlNfesEstados.TipoUrlEnvio.Autorizacao);
                wNfeAut.Timeout = 200000;
                retObj = wNfeAut.nfeAutorizacaoLote(nodeStatus);
                DeserilizarEvento(retObj, ref obj);
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Nfe, "Saida", Ex.Message.ToString());
                mLog.InsertErroLog("Erro Lote:" + obj.Lote + " Loja : " + obj.Loja + ". " + NFuncoes.TiraCampos(Ex.Message.ToString()));
            }
        }
        public void DeserilizarEvento(object obj, ref Entidade_NotaFiscal objDes)
        {

            //mStatus = new Model_StatusNfe();
            //Entidade_Status eStatusRet = new Entidade_Status();
            mLog = new Model_LogNfe();
            ObjLote = new Model_Lote();

            object objRet = new object();
            try
            {
                retXmlNodeReader = new XmlNodeReader((XmlNode)obj);
                xmlDesSerializar = new XmlSerializer(typeof(TRetEnviNFe));

                objRet = xmlDesSerializar.Deserialize(retXmlNodeReader);

                var Ret = (TRetEnviNFe)objRet;

                var InfRet = (TRetEnviNFeInfRec)Ret.Item;

                if (InfRet != null)
                    ObjLote.UpdateLoteRecebidos("R", Convert.ToDateTime(Ret.dhRecbto), InfRet.tMed, InfRet.nRec, Ret.cStat, objDes.Lote.ToString(), objDes.Loja);
                else
                    ObjLote.UpdateLoteRecebidos("R", Convert.ToDateTime(Ret.dhRecbto), "0", "0", Ret.cStat, objDes.Lote.ToString(), objDes.Loja);
            }
            catch (Exception Ex)
            {
                Mensagem.MensagemErro(Mensagem.TipoMensagem.Nfe, "Saida", Ex.Message.ToString());
                mLog.InsertErroLog(Ex.Message.ToString());
            }
        }
        public Entidade_NotaFiscal CarregarDados(int Loja, int NotaFiscal, string SerieNf, string tipoNf)
        {
            List<Entidade_Duplicatas> ListDuplicatas = new List<Entidade_Duplicatas>();
            List<Entidade_ItemNotaFiscal> ListItemNotaFiscal = new List<Entidade_ItemNotaFiscal>();
            ListObjNotaFiscalReferida = new List<Entidade_NotaFiscalReferida>();
            ObjTotais = new Entidade_Totais();
            ObjEntEmit = new Entidade_Emitente();
            ObjEntDest = new Entidade_Destinatario();
            ObjNotaFiscal = new Entidade_NotaFiscal();
            ObjModNfe = new Model_NotaFiscal();
            ObjItemNotaFiscal = new Entidade_ItemNotaFiscal();
            NFuncoes = new NegocioFuncoesGerais();
            ObjNotaFiscalReferida = new Entidade_NotaFiscalReferida();
            mLog = new Model_LogNfe();
            string LocalErro = string.Empty;

            string TipoAmbiente = FuncoesGerais.TipoAmbiente();
            string TipoEmissao = FuncoesGerais.TipoEmissao();
            var DtNotaFiscal = ObjModNfe.ConsultarNotasFiscais(Loja, NotaFiscal, SerieNf, 0, Model_NotaFiscal.NotaFiscal.Saida);
            var DtNotaFiscalItens = ObjModNfe.ConsultarNotasFiscalItens(Loja, NotaFiscal, SerieNf, 0, Model_NotaFiscal.NotaFiscal.Saida);
            //var DtDuplicatas = ObjModNfe.ConsultarNotasFiscalSaidaDuplicatas(Loja, NotaFiscal, SerieNf);
            //var DtNotaFiscalReferida = ObjModNfe.ConsultarNotasFiscalReferidaSaida(Loja, NotaFiscal, SerieNf);


            try
            {
                if (DtNotaFiscal.Rows.Count > 0)
                {
                    #region Dados da NotaFiscal
                    LocalErro = "Nota Fiscal";
                    //Dados da Nota Fiscal
                    ObjNotaFiscal.cUf = int.Parse(DtNotaFiscal.Rows[0]["CdUfCidadeIbge_Emitente"].ToString().Substring(0, 2));
                    ObjNotaFiscal.CdFornecedor = 0;
                    ObjNotaFiscal.Loja = int.Parse(DtNotaFiscal.Rows[0]["cdloja"].ToString());
                    ObjNotaFiscal.NaturezaOperacao = DtNotaFiscal.Rows[0]["naturezaoperacao"].ToString().Trim();
                    ObjNotaFiscal.ModNfe = int.Parse(DtNotaFiscal.Rows[0]["ModNfe"].ToString());
                    ObjNotaFiscal.sSerieNf = DtNotaFiscal.Rows[0]["nmserienf"].ToString().Trim();
                    ObjNotaFiscal.NotaFiscal = int.Parse(DtNotaFiscal.Rows[0]["NrNf"].ToString());
                    ObjNotaFiscal.DtEmissao = Convert.ToDateTime(DtNotaFiscal.Rows[0]["DtSaida"].ToString());
                    //ObjNotaFiscal.Observacao = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["Observacao"].ToString().Trim() + ":" + DtNotaFiscal.Rows[0]["MensagemOrcamento"].ToString().Trim() + ":" + DtNotaFiscal.Rows[0]["MensagemComprovanteDeposito"].ToString().Trim() + " | " + DtNotaFiscal.Rows[0]["MensagemLocalEntrega"].ToString().Trim() + ":" + DtNotaFiscal.Rows[0]["MensagemParamVenda"].ToString().Trim() + "|" + DtNotaFiscal.Rows[0]["CdVendedor"].ToString().Trim() + " " + DtNotaFiscal.Rows[0]["NmVendedor"].ToString().Trim() + ". Prazo : " + DtNotaFiscal.Rows[0]["NmPrazo"].ToString().Trim() + ". Rota:" + DtNotaFiscal.Rows[0]["nrRotaGP"].ToString().Trim());
                    ObjNotaFiscal.Observacao = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["Observacao"].ToString().Trim());
                    ObjNotaFiscal.ChaveAcessoNfe = DtNotaFiscal.Rows[0]["TxChAcessoNFe"].ToString().Trim();
                    ObjNotaFiscal.CdNfe = string.IsNullOrEmpty(DtNotaFiscal.Rows[0]["cdNFe"].ToString()) ? 0 : int.Parse(DtNotaFiscal.Rows[0]["cdNFe"].ToString());
                    if (tipoNf == "E")
                        ObjNotaFiscal.TpNfe = "E";
                    else
                        ObjNotaFiscal.TpNfe = "S";
                    if (DtNotaFiscal.Rows[0]["CdUf_Destinatario"].ToString() == DtNotaFiscal.Rows[0]["CdUf_Emitente"].ToString())
                        ObjNotaFiscal.TipoOperacao = 1;
                    else
                        ObjNotaFiscal.TipoOperacao = 2;

                    if (TipoEmissao.Trim() == "1")
                        ObjNotaFiscal.TpEmis = 1;
                    else
                        ObjNotaFiscal.TpEmis = 2;

                    ObjNotaFiscal.TpAmbiente = TipoAmbiente;

                    if (Convert.ToBoolean(DtNotaFiscal.Rows[0]["FlCartao"].ToString()) == true)
                        ObjNotaFiscal.TipoPagamento = 1;
                    else if (Convert.ToBoolean(DtNotaFiscal.Rows[0]["FlAvista"]) == true)
                        ObjNotaFiscal.TipoPagamento = 0;
                    else
                        ObjNotaFiscal.TipoPagamento = 0;

                    if (Convert.ToInt32(DtNotaFiscal.Rows[0]["FlFinalidade"]) == 2)
                        ObjNotaFiscal.TipoFInalidadeNfe = 2;
                    else if (Convert.ToInt32(DtNotaFiscal.Rows[0]["FlFinalidade"]) == 3)
                        ObjNotaFiscal.TipoFInalidadeNfe = 3;
                    else if (Convert.ToInt32(DtNotaFiscal.Rows[0]["FlFinalidade"]) == 4)
                        ObjNotaFiscal.TipoFInalidadeNfe = 4;
                    else
                        ObjNotaFiscal.TipoFInalidadeNfe = 1;
                    #endregion

                    #region Dados do Emitente

                    LocalErro = "Dados do Emitente";
                    //Emitente
                    ObjEntEmit.Nome = DtNotaFiscal.Rows[0]["NmRazaoSocial_Emitente"].ToString().Trim();
                    ObjEntEmit.RazaoSocial = DtNotaFiscal.Rows[0]["NmRazaoSocial_Emitente"].ToString().Trim();
                    ObjEntEmit.CpfCnpj = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCpfCgc_Emitente"].ToString().Trim());
                    ObjEntEmit.IE = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["NrCgf_Emitente"].ToString().Trim());
                    ObjEntEmit.Lagradouro = DtNotaFiscal.Rows[0]["NmEnder_Emitente"].ToString().Trim();
                    ObjEntEmit.Numero = DtNotaFiscal.Rows[0]["NrEnder_Emitente"].ToString().Trim();
                    ObjEntEmit.Bairro = DtNotaFiscal.Rows[0]["NmBairro_Emitente"].ToString().Trim().Trim();
                    ObjEntEmit.sCep = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCep_Emitente"].ToString());
                    ObjEntEmit.sUf = DtNotaFiscal.Rows[0]["CdUf_Emitente"].ToString();
                    ObjEntEmit.cMunicipio = DtNotaFiscal.Rows[0]["CdUfCidadeIbge_Emitente"].ToString().Trim() == string.Empty ? 0 : int.Parse(DtNotaFiscal.Rows[0]["CdUfCidadeIbge_Emitente"].ToString());
                    ObjEntEmit.sMunicipio = DtNotaFiscal.Rows[0]["NmCidadeIbge_Emitente"].ToString();
                    #endregion

                    #region Dados do Destinatario

                    LocalErro = "Dados do Destinatario";
                    //Destinatario
                    ObjEntDest.Nome = DtNotaFiscal.Rows[0]["NmCliente_Destinatario"].ToString().Trim();
                    ObjEntDest.CpfCnpj = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCpfCgc_Destinatario"].ToString()).Trim();
                    ObjEntDest.IE = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["NrCgf_Destinatario"].ToString()).Trim();
                    ObjEntDest.Lagradouro = DtNotaFiscal.Rows[0]["NmEnder_Destinatario"].ToString().Trim();
                    ObjEntDest.Numero = DtNotaFiscal.Rows[0]["NrEnder_Destinatario"].ToString().Trim();
                    ObjEntDest.FlIsento = Convert.ToBoolean(DtNotaFiscal.Rows[0]["Isento_Destinatario"]);
                    ObjEntDest.Bairro = DtNotaFiscal.Rows[0]["NmBairro_Destinatario"].ToString().Trim();
                    ObjEntDest.sCep = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCep_Destinatario"].ToString());
                    ObjEntDest.sUf = DtNotaFiscal.Rows[0]["CdUf_Destinatario"].ToString().Trim();
                    ObjEntDest.cMunicipio = DtNotaFiscal.Rows[0]["CdUfCidadeIbge_Destinatario"].ToString().Trim() == string.Empty ? 0 : int.Parse(DtNotaFiscal.Rows[0]["CdUfCidadeIbge_Destinatario"].ToString());
                    ObjEntDest.sMunicipio = DtNotaFiscal.Rows[0]["NmCidade_Destinatario"].ToString().Trim();
                    #endregion

                    #region Totais

                    LocalErro = "Totais";
                    //Totais
                    ObjTotais.ValorBaseIcms = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlBaseIcms"]);
                    ObjTotais.ValorIcms = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlIcms"]);
                    ObjTotais.ValorIcmsBaseSub = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlBaseIcmsSub"]);
                    ObjTotais.ValorIcmsSub = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlIcmsSub"]);
                    ObjTotais.ValorFrete = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlFrete"]);
                    ObjTotais.ValorSeguro = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlSeguro"]);
                    ObjTotais.ValorDesconto = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlDesconto"]);
                    ObjTotais.ValorOutrasDesp = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlOutrasDesp"]);
                    ObjTotais.ValorIpi = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlIpi"]);
                    ObjTotais.ValorTotalProdutos = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlProdutos"]);
                    ObjTotais.ValorTotal = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlTotal"]);
                    ObjTotais.ValorPis = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlPis"]);
                    ObjTotais.ValorCofins = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlCofins"]);
                    #endregion

                    #region Local de Entrega
                    if (DtNotaFiscal.Rows[0]["LocalEntrega_NmEnder"].ToString() != string.Empty && DtNotaFiscal.Rows[0]["LocalEntrega_CdUfCidadeIbge"].ToString() != string.Empty && NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCpfCgc_Destinatario"].ToString()).Trim().Length == 14)
                    {
                        LocalErro = "Dados do Local de Entrega";
                        ObjLocalEntrega = new Entidade_LocalEntrega();
                        ObjLocalEntrega.sMunicipio = DtNotaFiscal.Rows[0]["LocalEntrega_NmCidade"].ToString();
                        ObjLocalEntrega.Bairro = DtNotaFiscal.Rows[0]["LocalEntrega_NmBairro"].ToString();
                        ObjLocalEntrega.sUf = DtNotaFiscal.Rows[0]["LocalEntrega_CdUf"].ToString();
                        ObjLocalEntrega.cMunicipio = DtNotaFiscal.Rows[0]["LocalEntrega_cdUfCidadeIbge"].ToString() == string.Empty ? 0 : int.Parse(DtNotaFiscal.Rows[0]["LocalEntrega_cdUfCidadeIbge"].ToString());
                        ObjLocalEntrega.Lagradouro = DtNotaFiscal.Rows[0]["LocalEntrega_NmEnder"].ToString();
                        ObjLocalEntrega.Numero = DtNotaFiscal.Rows[0]["LocalEntrega_NrEnder"].ToString();
                        ObjLocalEntrega.Complemento = DtNotaFiscal.Rows[0]["LocalEntrega_NmComplEnder"].ToString();
                    }
                    else
                        ObjLocalEntrega = null;


                    if (DtNotaFiscal.Rows[0]["FretePorConta"].ToString().Trim() == "1")
                    {
                        ObjTrans = new Entidade_Transportador();
                        if (DtNotaFiscal.Rows[0]["CdCpfCgcTrans"].ToString().Trim() != string.Empty)
                        {
                            ObjTrans.CpfCnpj = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCpfCgcTrans"].ToString().Trim());
                            ObjTrans.RazaoSocial = DtNotaFiscal.Rows[0]["NmTransportadora"].ToString().Trim();
                            ObjTrans.Lagradouro = DtNotaFiscal.Rows[0]["NmEnderTrans"].ToString().Trim();
                            ObjTrans.sMunicipio = DtNotaFiscal.Rows[0]["NmCidadeTrans"].ToString().Trim();
                            ObjTrans.IE = DtNotaFiscal.Rows[0]["NrCgfTrans"].ToString().Trim();
                            ObjTrans.sUf = DtNotaFiscal.Rows[0]["CdUfTrans"].ToString().Trim();
                        }
                        else
                            ObjTrans = null;

                    }

                    #endregion

                    #region Duplicatas
                    //Duplicatas
                    //if (DtDuplicatas.Rows.Count > 0)
                    //{
                    //    LocalErro = "Duplicatas";
                    //    for (int i = 0; i < DtDuplicatas.Rows.Count; i++)
                    //    {
                    //        ObjDuplicatas = new Entidade_Duplicatas();
                    //        if (SerieNf != "D1")
                    //        {
                    //            ObjDuplicatas.NumeroDup = DtDuplicatas.Rows[i]["nrDup"].ToString() + DtDuplicatas.Rows[i]["NmParcDup"].ToString().Trim();
                    //            ObjDuplicatas.DataVencimento = Convert.ToDateTime(DtDuplicatas.Rows[i]["dtVencto"].ToString());
                    //            ObjDuplicatas.ValorDup = decimal.Parse(DtDuplicatas.Rows[i]["VlDup"].ToString());
                    //        }
                    //        else
                    //        {
                    //            ObjDuplicatas.TipoPagamento = Convert.ToInt32(DtDuplicatas.Rows[i]["TipoPagto"]);
                    //            ObjDuplicatas.ValorDup = decimal.Parse(DtDuplicatas.Rows[i]["Total"].ToString());
                    //        }
                    //        ListDuplicatas.Add(ObjDuplicatas);
                    //    }
                    //}
                    //else
                    ListDuplicatas = null;

                    #endregion

                    #region Item das Notas Fiscais
                    for (int i = 0; i < DtNotaFiscalItens.Rows.Count; i++)
                    {
                        LocalErro = "Dados dos Itens da Nota";
                        ObjItemNotaFiscal = new Entidade_ItemNotaFiscal();
                        ObjItemNotaFiscal.CodigoProduto = DtNotaFiscalItens.Rows[i]["CodigoDoProduto"].ToString().Trim();
                        //ObjItemNotaFiscal.NrSeqItem = int.Parse(DtNotaFiscalItens.Rows[i]["NrSeqItem"].ToString());
                        ObjItemNotaFiscal.NrSeqItem = i + 1;
                        ObjItemNotaFiscal.CFOP = DtNotaFiscalItens.Rows[i]["CFOP"].ToString().Trim();
                        if (CfopDev.Contains(DtNotaFiscalItens.Rows[i]["CFOP"].ToString().Trim()) == true)
                        {
                            ObjNotaFiscal.TipoFInalidadeNfe = 4;
                        }
                        ObjItemNotaFiscal.DescricaoProdutos = DtNotaFiscalItens.Rows[i]["DescricaoDoProduto"].ToString().Trim();
                        ObjItemNotaFiscal.Unidade = DtNotaFiscalItens.Rows[i]["UND"].ToString().Trim();
                        ObjItemNotaFiscal.Qtd = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["Quantidade"].ToString());
                        ObjItemNotaFiscal.ValorUnitario = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorUnitario"].ToString());
                        ObjItemNotaFiscal.ValorTotal = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlTotal"].ToString());
                        ObjItemNotaFiscal.ValorTotalIcms = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorTotalIcms"].ToString());
                        ObjItemNotaFiscal.ValorIcms = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorIcms"].ToString());
                        ObjItemNotaFiscal.AliqIcms = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ICMS"].ToString());
                        ObjItemNotaFiscal.AliqIpi = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["IPI"].ToString());
                        ObjItemNotaFiscal.ValorIpi = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorIPI"].ToString());
                        ObjItemNotaFiscal.ValorBaseIpi = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorBaseIPI"].ToString());
                        ObjItemNotaFiscal.ValorPis = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorPis"].ToString());
                        ObjItemNotaFiscal.ValorCofins = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorCofins"].ToString());
                        ObjItemNotaFiscal.AliqPis = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlAliqPis"].ToString());
                        ObjItemNotaFiscal.AliqCofins = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlAliqCofins"].ToString());
                        ObjItemNotaFiscal.NCM = DtNotaFiscalItens.Rows[i]["NCM"].ToString().Trim().Trim();
                        ObjItemNotaFiscal.PercMVA = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["PercMVA"]);
                        ObjItemNotaFiscal.PercReducao = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["PercReducao"]);
                        ObjItemNotaFiscal.ValorIcmsBaseSub = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorBaseIcmsSub"]);
                        ObjItemNotaFiscal.ValorIcmsSub = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorBaseIcmsSub"]) * (Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ICMS"].ToString()) / 100) - Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["ValorIcms"].ToString());
                        ObjItemNotaFiscal.CSTICMS = DtNotaFiscalItens.Rows[i]["CstICMS"].ToString().Trim();
                        ObjItemNotaFiscal.CSTPI = DtNotaFiscalItens.Rows[i]["CstIPI"].ToString().Trim();
                        ObjItemNotaFiscal.CSTPIS = DtNotaFiscalItens.Rows[i]["CstPIS"].ToString().Trim();
                        ObjItemNotaFiscal.CSTCOFINS = DtNotaFiscalItens.Rows[i]["CstCOFINS"].ToString().Trim();
                        ObjItemNotaFiscal.Desconto = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlDescItem"].ToString());
                        ObjItemNotaFiscal.Origem = DtNotaFiscalItens.Rows[i]["CdOrigemProduto"].ToString().Trim();

                        ListItemNotaFiscal.Add(ObjItemNotaFiscal);
                    }
                    #endregion

                    #region Nota Fiscal Referida

                    //if (DtNotaFiscalReferida.Rows.Count > 0)
                    //{
                    //    LocalErro = "Dados do Nota Fiscal Referida";
                    //    for (int i = 0; i < DtNotaFiscalReferida.Rows.Count; i++)
                    //    {
                    //        ObjNotaFiscalReferida = new Entidade_NotaFiscalReferida();
                    //        ObjNotaFiscalReferida.TxChAcessoNfeReferida = DtNotaFiscalReferida.Rows[0]["TxChAcessoNfe"].ToString();
                    //        ObjNotaFiscalReferida.COO = 0;
                    //        ObjNotaFiscalReferida.SerieNfRef = DtNotaFiscalReferida.Rows[0]["serienfReferida"].ToString();
                    //    }
                    //    ListObjNotaFiscalReferida.Add(ObjNotaFiscalReferida);
                    //    ObjNotaFiscal.NotaFiscalReferida = ListObjNotaFiscalReferida;
                    //}
                    //else
                    ObjNotaFiscal.NotaFiscalReferida = null;

                    #endregion

                    ObjNotaFiscal.EntEmit = ObjEntEmit;
                    ObjNotaFiscal.EntDest = ObjEntDest;
                    ObjNotaFiscal.EntDuplicata = ListDuplicatas;
                    ObjNotaFiscal.EntLocalEntrega = ObjLocalEntrega;
                    ObjNotaFiscal.EntTotais = ObjTotais;
                    ObjNotaFiscal.EntItemNotaFiscal = ListItemNotaFiscal;
                    ObjNotaFiscal.EntTransportador = ObjTrans;
                }
            }
            catch (InvalidCastException Ice)
            {
                if (Ice.Source != null)
                    mLog.InsertErroLog("Erro em consulta de nota fiscal. \nErro encontrado:" + Ice.Source + "." + Ice.Message.ToString() + "." + LocalErro);
                else
                    mLog.InsertErroLog("Erro em consulta de nota fiscal. \nErro encontrado:" + Ice.Message.ToString() + "." + LocalErro);
                return null;
            }
            catch (IOException e)
            {
                if (e.Source != null)
                    mLog.InsertErroLog("Erro em consulta de nota fiscal. \nErro encontrado:" + e.Source + "." + e.Message.ToString() + "." + LocalErro);
                else
                    mLog.InsertErroLog("Erro em consulta de nota fiscal. \nErro encontrado:" + e.Message.ToString() + "." + LocalErro);
                return null;
            }
            catch (Exception ex)
            {
                if (ex.Source != null)
                    mLog.InsertErroLog("Erro em consulta de nota fiscal. \nErro encontrado:" + ex.Source + "." + ex.Message.ToString() + "." + LocalErro);
                else
                    mLog.InsertErroLog("Erro em consulta de nota fiscal. \nErro encontrado:" + ex.Message.ToString() + "." + LocalErro);
                return null;
            }
            return ObjNotaFiscal;
        }
        public int GerarLoteNfe(int Loja)
        {
            ObjModNfe = new Model_NotaFiscal();

            return ObjModNfe.IncluirLoteNfeSaida(Loja, Model_NotaFiscal.NotaFiscal.Saida);
        }
        public DataTable PesquisaNotasFiscaisLoteNfe(int Loja, int Lote)
        {
            ObjModNfe = new Model_NotaFiscal();

            return ObjModNfe.PesquisarLoteNotaFiscais(Loja, Lote);
        }

        string CabecalhoEnvieNfe(int nrlote)
        {
            return "<?xml version=\"1.0\" encoding=\"UTF-8\"?> <enviNFe xmlns=\"http://www.portalfiscal.inf.br/nfe\" versao=\"3.10\"> <idLote>" + nrlote + "</idLote><indSinc>0</indSinc></enviNFe>";
        }
        void CarregaAliqInterestadual()
        {
            ListAliqUfInterestadual.Add("AC",Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("AL", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("AM", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("AP", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("BA", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("CE", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("DF", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("ES", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("GO", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("MA", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("MG", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("MS", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("MT", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("PA", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("PB", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("PE", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("PI", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("PR", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("RJ", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("RN", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("RO", Convert.ToDecimal(17.50));
            ListAliqUfInterestadual.Add("RR", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("RS", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("SC", Convert.ToDecimal(17.00));
            ListAliqUfInterestadual.Add("SE", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("SP", Convert.ToDecimal(18.00));
            ListAliqUfInterestadual.Add("TO", Convert.ToDecimal(18.00));
        }
    }
}

