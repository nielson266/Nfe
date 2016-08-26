using Nfe.Entidade;
using Nfe.Model;
using Nfe.Negocio.Geral;
using Nfe.Negocio.InterfacesAbstratos;
using NFe.retEnviNFe;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Nfe.Negocio.Nfe
{
    public class EnviarEntrada : abMetodos, IMetodosBase<Entidade_NotaFiscal>
    {
        Entidade_ItemNotaFiscal ObjItemNotaFiscal;
        Entidade_Duplicatas ObjDuplicatas;
        Entidade_LocalEntrega ObjLocalEntrega;
        Entidade_Totais ObjTotais;
        Entidade_Emitente ObjEntEmit;
        Entidade_Destinatario ObjEntDest;
        Entidade_NotaFiscal ObjNotaFiscal;
        Entidade_NotaFiscalReferida ObjNotaFiscalReferida;
        List<Entidade_NotaFiscalReferida> ListObjNotaFiscalReferida;
        //Models
        Model_NotaFiscal ObjModNfe;
        Model_LogNfe mLog;
        Model_Lote ObjLote;
        FuncoesGerais ObjFuncGerais;
        nfe.TNFe Nfe;

        NegocioFuncoesGerais NFuncoes;
        public void Enviar(Entidade_NotaFiscal ObjEnt, out Entidade_NotaFiscal objDados)
        {
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

            var Lojas = FuncoesGerais.LojasEmitentes();

            for (int i = 0; i < Lojas.Rows.Count; i++)
            {
                int Loja = 0;
                int Nota = 0;
                int Fornecedor = 0;
                string Serie = "";
                int TpEmis = 0;
                var Lote = GerarLoteNfe(Convert.ToInt32(Lojas.Rows[i]["id_loja"].ToString()));
                docEnviNfe = new XmlDocument();
                if (Lote != 0)
                {
                    docEnviNfe.PreserveWhitespace = false;
                    docEnviNfe.LoadXml(CabecalhoEnvieNfe(Lote));

                    nodeListNfe = docEnviNfe.GetElementsByTagName("enviNFe");

                    DtLoteNfe = PesquisaNotasFiscaisLoteNfe(Convert.ToInt32(Lojas.Rows[i]["id_loja"].ToString()), Lote);

                    foreach (DataRow RowNFDt in DtLoteNfe.Rows)
                    {
                        try
                        {
                            docTran = new XmlDocument();

                            var RetEnt = CarregarDados(Convert.ToInt32(RowNFDt["id_loja"]), Convert.ToInt32(RowNFDt["NrNf"]), RowNFDt["serienf"].ToString(), Convert.ToInt32(RowNFDt["CdFornec"]), RowNFDt["TpNFe"].ToString());
                            if (RetEnt != null)
                            {
                                ObjNotaFiscal.cUf = RetEnt.cUf; // Uf do Emitente
                                ObjNotaFiscal.Loja = RetEnt.Loja;
                                Loja = RetEnt.Loja;
                                Nota = RetEnt.NotaFiscal;
                                Fornecedor = RetEnt.CdFornecedor;
                                Serie = RetEnt.sSerieNf;
                                TpEmis = RetEnt.TpEmis;
                                ObjNotaFiscal.Lote = Lote;

                                Nfe = new nfe.TNFe(RetEnt);
                            }
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
                                Mensagem.MensagemErro(Mensagem.TipoMensagem.XmlLoteGerados, "Entrada", Lote.ToString() + "|" + "Nota :" + RetEnt.sSerieNf + "-" + RetEnt.NotaFiscal.ToString() + "|" + docTran.OuterXml);
                                nodeListCarregarNfe = docTran.GetElementsByTagName("NFe", "http://www.portalfiscal.inf.br/nfe");
                                nodeListNfe.Item(0).AppendChild(docEnviNfe.ImportNode(nodeListCarregarNfe.Item(0), true));
                                ObjLote.UpdateXmlTpSaida(RetEnt.Loja, RetEnt.NotaFiscal, RetEnt.sSerieNf, Lote, RetEnt.TpEmis.ToString(), docTran.OuterXml, RetEnt.CdFornecedor);
                                ObjModNfe.UpdateNfTpEmisVersaoNFe(RetEnt.TpEmis.ToString(), null, "3.10", Lote.ToString(), RetEnt.NotaFiscal.ToString(), RetEnt.Loja.ToString(), RetEnt.sSerieNf, RetEnt.CdFornecedor, Model_NotaFiscal.NotaFiscal.Entrada);
                            }
                        }
                        catch (Exception Ex)
                        {
                            Mensagem.MensagemErro(Mensagem.TipoMensagem.Nfe, "Entrada", Ex.Message.ToString());
                            ObjLote.UpdateXmlTpSaida(Loja, Nota, Serie, Lote, TpEmis.ToString(), docTran.OuterXml,Fornecedor);
                            ObjModNfe.UpdateNfErro(225, Loja, Serie, Nota, Fornecedor, Model_NotaFiscal.NotaFiscal.Entrada);
                            ObjLote.UpdateLoteErro(225, Loja, Serie, Nota, Fornecedor);
                            mLog.InsertErroLog("Erro Lote:" + Lote + ", Nota Fiscal Entrada:" + Serie + "-" + Nota + ". " + NFuncoes.TiraCampos(Ex.Message.ToString()));
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
        public Entidade_NotaFiscal CarregarDados(int Loja, int NotaFiscal, string SerieNf, int Fornec, string tipoNf)
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

            string TipoAmbiente = FuncoesGerais.TipoAmbiente();
            string TipoEmissao = FuncoesGerais.TipoEmissao();
            var DtNotaFiscal = ObjModNfe.ConsultarNotasFiscais(Loja, NotaFiscal, SerieNf, Fornec, Model_NotaFiscal.NotaFiscal.Entrada);
            var DtNotaFiscalItens = ObjModNfe.ConsultarNotasFiscalItens(Loja, NotaFiscal, SerieNf, Fornec, Model_NotaFiscal.NotaFiscal.Entrada);
            var DtNotaFiscalReferida = ObjModNfe.ConsultarNotasFiscalReferidaEntrada(Loja, NotaFiscal, SerieNf, Fornec);

            if (DtNotaFiscal.Rows.Count > 0)
            {
                #region Dados da NotaFiscal
                //Dados da Nota Fiscal
                ObjNotaFiscal.cUf = int.Parse(DtNotaFiscal.Rows[0]["CdUfCidadeIbge_Emitente"].ToString().Substring(0, 2));
                ObjNotaFiscal.CdFornecedor = int.Parse(DtNotaFiscal.Rows[0]["CdFornec"].ToString());
                ObjNotaFiscal.Loja = int.Parse(DtNotaFiscal.Rows[0]["id_loja"].ToString());
                ObjNotaFiscal.NaturezaOperacao = DtNotaFiscal.Rows[0]["NmCfo"].ToString().Trim();
                ObjNotaFiscal.ModNfe = int.Parse(DtNotaFiscal.Rows[0]["ModNfe"].ToString());
                ObjNotaFiscal.sSerieNf = DtNotaFiscal.Rows[0]["serienf"].ToString().Trim();
                ObjNotaFiscal.NotaFiscal = int.Parse(DtNotaFiscal.Rows[0]["NrNf"].ToString());
                ObjNotaFiscal.DtEmissao = Convert.ToDateTime(DtNotaFiscal.Rows[0]["DtEmissao"].ToString());
                ObjNotaFiscal.DtEntrada = Convert.ToDateTime(DtNotaFiscal.Rows[0]["DtEntrada"].ToString());
                ObjNotaFiscal.Observacao = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["Observacao"].ToString().Trim());
                ObjNotaFiscal.ChaveAcessoNfe = DtNotaFiscal.Rows[0]["TxChAcessoNFe"].ToString().Trim();
                ObjNotaFiscal.CdNfe = string.IsNullOrEmpty(DtNotaFiscal.Rows[0]["cdNFe"].ToString()) ? 0 : int.Parse(DtNotaFiscal.Rows[0]["cdNFe"].ToString());
                if (tipoNf == "E")
                    ObjNotaFiscal.TpNfe = "E";
                else
                    ObjNotaFiscal.TpNfe = "S";
                if (DtNotaFiscal.Rows[0]["CdUf_Remetente"].ToString() == DtNotaFiscal.Rows[0]["CdUf_Emitente"].ToString())
                    ObjNotaFiscal.TipoOperacao = 1;
                else
                    ObjNotaFiscal.TipoOperacao = 2;

                if (TipoEmissao.Trim() == "1")
                    ObjNotaFiscal.TpEmis = 1;
                else
                    ObjNotaFiscal.TpEmis = 2;

                ObjNotaFiscal.TpAmbiente = TipoAmbiente;

                if (Convert.ToBoolean(DtNotaFiscal.Rows[0]["FlNfComplementar"].ToString()) == true)
                    ObjNotaFiscal.TipoFInalidadeNfe = 2;
                else if (Convert.ToBoolean(DtNotaFiscal.Rows[0]["FlNfAjuste"].ToString()) == true)
                    ObjNotaFiscal.TipoFInalidadeNfe = 3;
                else if (DtNotaFiscal.Rows[0]["CdTipoNf"].ToString() == "D")
                    ObjNotaFiscal.TipoFInalidadeNfe = 4;
                else
                    ObjNotaFiscal.TipoFInalidadeNfe = 1;
                #endregion

                #region Dados do Emitente
                //Emitente
                ObjEntEmit.Nome = DtNotaFiscal.Rows[0]["NmRazaoSocial_Emitente"].ToString().Trim().Trim();
                ObjEntEmit.RazaoSocial = DtNotaFiscal.Rows[0]["NmRazaoSocial_Emitente"].ToString().Trim();
                ObjEntEmit.CpfCnpj = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCpfCgc_Emitente"].ToString().Trim());
                ObjEntEmit.IE = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["NrCgf_Emitente"].ToString().Trim());
                ObjEntEmit.Lagradouro = DtNotaFiscal.Rows[0]["NmEnder_Emitente"].ToString().Trim();
                ObjEntEmit.Numero = DtNotaFiscal.Rows[0]["NrEnder_Emitente"].ToString().Trim();
                ObjEntEmit.Bairro = DtNotaFiscal.Rows[0]["NmBairro_Emitente"].ToString().Trim().Trim();
                ObjEntEmit.sCep = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCep_Emitente"].ToString());
                ObjEntEmit.sUf = DtNotaFiscal.Rows[0]["CdUf_Emitente"].ToString();
                ObjEntEmit.cMunicipio = int.Parse(DtNotaFiscal.Rows[0]["CdUfCidadeIbge_Emitente"].ToString());
                ObjEntEmit.sMunicipio = DtNotaFiscal.Rows[0]["NmCidadeIbge_Emitente"].ToString();
                #endregion

                #region Dados do Destinatario
                //Destinatario
                ObjEntDest.Nome = DtNotaFiscal.Rows[0]["NmFornec_Remetente"].ToString().Trim();
                ObjEntDest.CpfCnpj = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCpfCgc_Remetente"].ToString()).Trim();
                ObjEntDest.IE = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["NrCgf_Remetente"].ToString()).Trim();
                ObjEntDest.Lagradouro = DtNotaFiscal.Rows[0]["NmEnder_Remetente"].ToString().Trim();
                ObjEntDest.Numero = DtNotaFiscal.Rows[0]["NrEnder_Remetente"].ToString().Trim();
                ObjEntDest.FlIsento = Convert.ToBoolean(DtNotaFiscal.Rows[0]["FlIsento_Remetente"]);
                ObjEntDest.Bairro = DtNotaFiscal.Rows[0]["NmBairro_Remetente"].ToString().Trim();
                ObjEntDest.sCep = NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCep_Remetente"].ToString());
                ObjEntDest.sUf = DtNotaFiscal.Rows[0]["CdUf_Remetente"].ToString().Trim();
                ObjEntDest.cMunicipio = int.Parse(DtNotaFiscal.Rows[0]["CdUfCidadeIbge_Remetente"].ToString());
                ObjEntDest.sMunicipio = DtNotaFiscal.Rows[0]["NmCidade_Remetente"].ToString().Trim();
                #endregion

                #region Totais
                //Totais
                ObjTotais.ValorBaseIcms = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlBaseIcms"]);
                ObjTotais.ValorIcms = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlIcms"]);
                ObjTotais.ValorIcmsBaseSub = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlBaseIcmsSub"]);
                ObjTotais.ValorIcmsSub = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlIcmsSub"]);
                ObjTotais.ValorFrete = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlFrete"]);
                ObjTotais.ValorSeguro = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlSeguro"]);
                ObjTotais.ValorOutrasDesp = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlOutrasDesp"]);
                ObjTotais.ValorIpi = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlIpi"]);
                ObjTotais.ValorTotalProdutos = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlProdutos"]);
                ObjTotais.ValorTotal = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlNf"]);
                ObjTotais.ValorPis = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlPis"]);
                ObjTotais.ValorCofins = Convert.ToDecimal(DtNotaFiscal.Rows[0]["VlCofins"]);
                #endregion

                #region Local de Entrega
                //if (Convert.ToBoolean(DtNotaFiscal.Rows[0]["NrSeq"]) && DtNotaFiscal.Rows[0]["LocalEntrega_NmEnder"].ToString() != string.Empty && DtNotaFiscal.Rows[0]["LocalEntrega_CdUfCidadeIbge"].ToString() != string.Empty && NFuncoes.TiraCampos(DtNotaFiscal.Rows[0]["CdCpfCgc_Destinatario"].ToString()).Trim().Length == 14)
                //{
                //    ObjLocalEntrega = new Entidade_LocalEntrega();
                //    ObjLocalEntrega.sMunicipio = DtNotaFiscal.Rows[0]["LocalEntrega_NmCidade"].ToString();
                //    ObjLocalEntrega.Bairro = DtNotaFiscal.Rows[0]["LocalEntrega_NmBairro"].ToString();
                //    ObjLocalEntrega.sUf = DtNotaFiscal.Rows[0]["LocalEntrega_CdUf"].ToString();
                //    ObjLocalEntrega.cMunicipio = DtNotaFiscal.Rows[0]["LocalEntrega_cdUfCidadeIbge"].ToString() == string.Empty ? 0 : int.Parse(DtNotaFiscal.Rows[0]["LocalEntrega_cdUfCidadeIbge"].ToString());
                //    ObjLocalEntrega.Lagradouro = DtNotaFiscal.Rows[0]["LocalEntrega_NmEnder"].ToString();
                //    ObjLocalEntrega.Numero = DtNotaFiscal.Rows[0]["LocalEntrega_NrEnder"].ToString();
                //    ObjLocalEntrega.Complemento = DtNotaFiscal.Rows[0]["LocalEntrega_NmComplEnder"].ToString();
                //}
                //else
                ObjLocalEntrega = null;

                #endregion

                #region Item das Notas Fiscais
                for (int i = 0; i < DtNotaFiscalItens.Rows.Count; i++)
                {
                    ObjItemNotaFiscal = new Entidade_ItemNotaFiscal();
                    ObjItemNotaFiscal.CodigoProduto = DtNotaFiscalItens.Rows[i]["CodigoDoProduto"].ToString().Trim();
                    ObjItemNotaFiscal.NrSeqItem = int.Parse(DtNotaFiscalItens.Rows[i]["NrSeqItem"].ToString());
                    ObjItemNotaFiscal.CFOP = DtNotaFiscalItens.Rows[i]["CFOP"].ToString().Trim();
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
                    ObjItemNotaFiscal.ValorBaseIpi = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlTotal"].ToString()); // Base do IPI
                    ObjItemNotaFiscal.ValorPis = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlPis"].ToString());
                    ObjItemNotaFiscal.ValorCofins = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlCofins"].ToString());
                    ObjItemNotaFiscal.AliqPis = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlAliqPis"].ToString());
                    ObjItemNotaFiscal.AliqCofins = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlAliqCofins"].ToString());
                    ObjItemNotaFiscal.NCM = DtNotaFiscalItens.Rows[i]["NCM"].ToString().Trim().Trim();
                    ObjItemNotaFiscal.CSTICMS = DtNotaFiscalItens.Rows[i]["CstICMS"].ToString().Trim();
                    ObjItemNotaFiscal.CSTPI = DtNotaFiscalItens.Rows[i]["CstIPI"].ToString().Trim();
                    ObjItemNotaFiscal.CSTPIS = DtNotaFiscalItens.Rows[i]["CstPIS"].ToString().Trim();
                    ObjItemNotaFiscal.CSTCOFINS = DtNotaFiscalItens.Rows[i]["CstCOFINS"].ToString().Trim();
                    ObjItemNotaFiscal.Desconto = Convert.ToDecimal(DtNotaFiscalItens.Rows[i]["VlDescItem"].ToString());
                    ObjItemNotaFiscal.Origem = DtNotaFiscalItens.Rows[i]["CdOrigemProduto"].ToString().Trim();

                    ListItemNotaFiscal.Add(ObjItemNotaFiscal);
                }
                #endregion


                if(DtNotaFiscalReferida.Rows.Count > 0)
                {
                    for (int i = 0; i < DtNotaFiscalReferida.Rows.Count; i++)
                    {
                        ObjNotaFiscalReferida = new Entidade_NotaFiscalReferida();
                        ObjNotaFiscalReferida.TxChAcessoNfeReferida = DtNotaFiscalReferida.Rows[0]["TxChAcessoNfe"].ToString();
                        ObjNotaFiscalReferida.COO = 0;
                        ObjNotaFiscalReferida.SerieNfRef = DtNotaFiscalReferida.Rows[0]["serienfReferida"].ToString();
                    }
                    ListObjNotaFiscalReferida.Add(ObjNotaFiscalReferida);
                    ObjNotaFiscal.NotaFiscalReferida = ListObjNotaFiscalReferida;
                }
                else
                {
                    ObjNotaFiscal.NotaFiscalReferida = null;
                }
                ObjNotaFiscal.EntEmit = ObjEntEmit;
                ObjNotaFiscal.EntDest = ObjEntDest;
                
                ObjNotaFiscal.EntDuplicata = null;
                ObjNotaFiscal.EntLocalEntrega = ObjLocalEntrega;
                ObjNotaFiscal.EntTotais = ObjTotais;
                ObjNotaFiscal.EntItemNotaFiscal = ListItemNotaFiscal;
            }
            return ObjNotaFiscal;
        }
        public int GerarLoteNfe(int Loja)
        {
            ObjModNfe = new Model_NotaFiscal();

            return ObjModNfe.IncluirLoteNfeSaida(Loja, Model_NotaFiscal.NotaFiscal.Entrada);
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
    }
}
