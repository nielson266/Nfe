using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Nfe.Negocio.StatusServico;
using Nfe.Negocio.InutilizacaoNfe;
using Nfe.Negocio.ConsultarProtocolo;
using Nfe.Negocio.Cancelamento;
using Nfe.Entidade;
using Nfe.Negocio.Geral;
using Nfe.Model;
using System.Xml;
using Nfe.Negocio.Nfe;
using Nfe.Negocio.ConsultaRecibo;
using System.IO;
using Nfe.Negocio.CartaEletronica;
using Nfe.Negocio.EnviarEmail;
using Nfe.Model;
using Nfe.Negocio;

namespace Nfe
{
    public partial class FConfiguracao : Form
    {
        public FConfiguracao()
        {
            InitializeComponent();

            /// Iniciar pausado para manuntenção 16/08/2016

            TmStatus.Enabled = true;
            TmInutilizacao.Enabled = true;
            TmCancelamento.Enabled = true;
            TmNotaFiscal.Enabled = true;
            TmConsultaLote.Enabled = true;
            //TmEntrada.Enabled = true;
            TmSemRetorno.Enabled = true;
            //sBtnSemRetorno.Image = Inicializar();

        }


        Entidade_Status eRet;
        Entidade_Inutilizacao eInut;
        Entidade_NotaFiscal eNf;
        Entidade_Recibo eRec;
        Entidade_Cancelamento eCancelamento;
        Model_LogNfe mLog;
        Model_Cancelamento mCancelamento;
        Model_CCe mCartaEletronica;
        Model_XmlCliente mXmlCliente;
        NegocioFuncoesGerais NFuncoes;


        private void TmInutilizacao_Tick(object sender, EventArgs e)
        {
            Model_InutilizacaoNfe mInut = new Model_InutilizacaoNfe();
            EnviarInutilizacao nEnvInut = new EnviarInutilizacao();

            TmInutilizacao.Enabled = false;

            var dtInut = mInut.Pesquisar();

            for (int i = 0; i < dtInut.Rows.Count; i++)
            {
                eInut = new Entidade_Inutilizacao();

                eInut.Loja = int.Parse(dtInut.Rows[i]["id_loja"].ToString());
                eInut.cUf = Convert.ToInt32(dtInut.Rows[i]["cdUfCidadeIbge_Empresa"].ToString().Substring(0, dtInut.Rows[i]["cdUfCidadeIbge_Empresa"].ToString().Length - 5));
                eInut.Cnpj = dtInut.Rows[i]["cnpj"].ToString();
                eInut.sSerieNf = dtInut.Rows[i]["serienf"].ToString().Trim();
                eInut.NrIni = int.Parse(dtInut.Rows[i]["numero_ini"].ToString());
                eInut.NrFim = int.Parse(dtInut.Rows[i]["numero_fim"].ToString());
                eInut.ModNfe = int.Parse(dtInut.Rows[i]["ModNfe"].ToString());
                eInut.TpAmbiente = FuncoesGerais.TipoAmbiente();
                nEnvInut.Enviar(eInut, out eInut);
            }

            TmInutilizacao.Enabled = true;
        }

        private void TmCancelamento_Tick(object sender, EventArgs e)
        {
            EnviarCancelamentoSefaz envCancelamento;
            mCancelamento = new Model_Cancelamento();
            eCancelamento = new Entidade_Cancelamento();


            TmCancelamento.Enabled = false;

            var dtCancelamento = mCancelamento.ConsultarCancelementosSolicitados();

            for (int i = 0; i < dtCancelamento.Rows.Count; i++)
            {
                envCancelamento = new EnviarCancelamentoSefaz();

                eCancelamento.id = Convert.ToInt32(dtCancelamento.Rows[i]["idoperacao"]);
                eCancelamento.Loja = Convert.ToInt32(dtCancelamento.Rows[i]["id_loja"]);
                //eCancelamento.CdFornec = dtCancelamento.Rows[i]["CdFornec"] != null ? Convert.ToInt32(dtCancelamento.Rows[i]["CdFornec"]) : 0;
                eCancelamento.NmSerie = dtCancelamento.Rows[i]["serienf"].ToString();
                eCancelamento.NrNf = Convert.ToInt32(dtCancelamento.Rows[i]["NrNF"].ToString());
                eCancelamento.CnpjCpf = dtCancelamento.Rows[i]["cnpj"].ToString();
                eCancelamento.ChaveAcessoNfe = dtCancelamento.Rows[i]["TxChAcessoNfe"].ToString();
                eCancelamento.TpNf = dtCancelamento.Rows[i]["TpNFe"].ToString();
                eCancelamento.ProtocoloAutoriz = dtCancelamento.Rows[i]["NrProtocoloAutorizNfe"].ToString().Trim();
                eCancelamento.cUf = Convert.ToInt32(dtCancelamento.Rows[i]["cdUfCidadeIbge_Empresa"].ToString().Substring(0, dtCancelamento.Rows[i]["cdUfCidadeIbge_Empresa"].ToString().Length - 5));
                eCancelamento.CodigoIbgeEmpresa = dtCancelamento.Rows[i]["cdUfCidadeIbge_Empresa"].ToString();
                eCancelamento.DataHora = dtCancelamento.Rows[i]["DtOperacao"].ToString();
                eCancelamento.TpAmb = FuncoesGerais.TipoAmbiente(); ;
                eCancelamento.Justificatica = "CANCELAMENTO DE NOTA FISCAL POR PROBLEMAS LANCAMENTO";
                envCancelamento.Enviar(eCancelamento, out eCancelamento);
            }

            TmCancelamento.Enabled = true;
        }
        private void sNotaFiscal_Click(object sender, EventArgs e)
        {
            if (TmNotaFiscal.Enabled == true)
            {
                TmNotaFiscal.Enabled = false;
            }
            else
            {
                TmNotaFiscal.Enabled = true;
            }
        }

        public Image Inicializar()
        {
            string Path = Directory.GetCurrentDirectory();

            Image img = Image.FromFile(Path + @"\image\backuground.jpg");

            return img;
        }

        public Task NotaFiscalSaida()
        {
            return null;
        }

        private void sBtnConsSitNfe_Click(object sender, EventArgs e)
        {

        }
        private void sBtnStatus_Click(object sender, EventArgs e)
        {
            if (TmStatus.Enabled == true)
            {
                TmStatus.Enabled = false;
            }
            else
            {
                TmStatus.Enabled = true;
            }
        }
        private void sBtnLote_Click(object sender, EventArgs e)
        {
            if (TmConsultaLote.Enabled == true)
            {
                TmConsultaLote.Enabled = false;
            }
            else
            {
                TmConsultaLote.Enabled = true;
            }
        }
        private void sBtnEntrada_Click(object sender, EventArgs e)
        {
            if (TmEntrada.Enabled == true)
            {
                TmEntrada.Enabled = false;
            }
            else
            {
                TmEntrada.Enabled = true;
            }
        }
        private void sBtnSemRetorno_Click(object sender, EventArgs e)
        {
            if (TmSemRetorno.Enabled == true)
            {
                TmSemRetorno.Enabled = false;
                
            }
            else
            {
                TmSemRetorno.Enabled = true;
                
            }
        }

        private void sBtnEnviarEmail_Click(object sender, EventArgs e)
        {
            if (TmEnviarEmailCliente.Enabled == true)
            {
                TmEnviarEmailCliente.Enabled = false;
                
            }
            else
            {
                TmEnviarEmailCliente.Enabled = true;
            }
        }

        #region STATUS SERVIÇO
        private void TmStatus_Tick(object sender, EventArgs e)
        {
            EnviarStatus status = new EnviarStatus();

            TmStatus.Enabled = false;
            try
            {

                var dtRet = FuncoesGerais.LojasEmitentes();

                for (int i = 0; i < dtRet.Rows.Count; i++)
                {
                    eRet = new Entidade_Status();
                    eRet.Loja = int.Parse(dtRet.Rows[i]["idloja"].ToString());
                    eRet.tpAmbiente = FuncoesGerais.TipoAmbiente();
                    eRet.cUf = FuncoesGerais.UfIbgeEmpresa(int.Parse(dtRet.Rows[i]["idloja"].ToString()));
                    eRet.Uf = dtRet.Rows[i]["uf"].ToString();
                    eRet.versao = dtRet.Rows[i]["VersaoNfe"].ToString();
                    status.Enviar(eRet, out eRet);

                    if (eRet != null)
                    {
                        txtStatus.Text = eRet.cStatus;
                        txtAmb.Text = eRet.AmbienteFormatado;
                        txtMsg.Text = eRet.sMotivo;
                        TxtVersao.Text = eRet.versao;
                        txtUltConsulta.Text = eRet.dhRet;
                    }
                }
                TmStatus.Enabled = true;
            }
            catch (XmlException exm)
            {
                TmStatus.Enabled = true;
                throw new Exception(exm.Message.ToString());
            }
            catch (Exception ex)
            {
                TmStatus.Enabled = true;
                throw new Exception(ex.Message.ToString());
            }
        }
        #endregion

        #region INUTILIZAÇÃO NFe
        private void sBtnInutilizaco_Click(object sender, EventArgs e)
        {
            if (TmInutilizacao.Enabled == true)
            {
                TmInutilizacao.Enabled = false;
                
            }
            else
            {
                TmInutilizacao.Enabled = true;
            }
        }
        #endregion

        #region CONSULTA LOTES NÃO PROCESSADOS
        private void TmConsultaLote_Tick(object sender, EventArgs e)
        {
            TmConsultaLote.Enabled = false;

            ConRecibo nRec = new ConRecibo();
            Model_Lote mLote = new Model_Lote();
            eRec = new Entidade_Recibo();

            var retDt = mLote.LotesNaoProcessados();

            for (int i = 0; i < retDt.Rows.Count; i++)
            {
                eRec.TpNf = retDt.Rows[i]["TpNFe"].ToString();
                eRec.Recibo = retDt.Rows[i]["NrRecibo"].ToString();
                eRec.Loja = int.Parse(retDt.Rows[i]["id"].ToString());
                eRec.cUf = int.Parse(FuncoesGerais.UfIbgeEmpresa(int.Parse(retDt.Rows[i]["id"].ToString())));
                eRec.TpAmb = FuncoesGerais.TipoAmbiente();
                nRec.Enviar(eRec, out eRec);
            }

            TmConsultaLote.Enabled = true;
        }
        #endregion

        #region NOTA FISCAL DE SAIDA
        private void TmNotaFiscal_Tick(object sender, EventArgs e)
        {
            mLog = new Model_LogNfe();
            TmNotaFiscal.Enabled = false;
            EnviarSaida objNfe = new EnviarSaida();
            eNf = new Entidade_NotaFiscal();
            objNfe.Enviar(null, out eNf);
            TmNotaFiscal.Enabled = true;

        }
        #endregion

        #region NOTA FISCAL DE ENTRANDA
        private void TmEntrada_Tick(object sender, EventArgs e)
        {
            mLog = new Model_LogNfe();
            try
            {
                TmEntrada.Enabled = false;
                EnviarEntrada objNfe = new EnviarEntrada();
                eNf = new Entidade_NotaFiscal();
                objNfe.Enviar(null, out eNf);
                TmEntrada.Enabled = true;
            }
            catch (Exception Ex)
            {
                TmEntrada.Enabled = true;
                mLog.InsertErroLog("Erro em consulta de nota fiscal. \nErro encontrado:" + Ex.Message.ToString());
            }
        }
        #endregion

        #region Nota Fiscal Sem Retorno
        private void TmSemRetorno_Tick(object sender, EventArgs e)
        {
            EnviarConsultaNfe ConsultaNfe = new EnviarConsultaNfe();
            Model_NotaFiscal mNotaFisca = new Model_NotaFiscal();
            NFuncoes = new NegocioFuncoesGerais();

            mLog = new Model_LogNfe();

            try
            {
                TmSemRetorno.Enabled = false;
                var DtRet = mNotaFisca.ConsultarNotasFiscalSemRetornoOuDuplicadas();

                for (int i = 0; i < DtRet.Rows.Count; i++)
                {
                    eNf = new Entidade_NotaFiscal();
                    eNf.Loja = int.Parse(DtRet.Rows[i]["id_loja"].ToString());
                    eNf.TpNfe = DtRet.Rows[i]["TpNfe"].ToString();
                    eNf.NotaFiscal = int.Parse(DtRet.Rows[i]["NrNf"].ToString());
                    eNf.sSerieNf = DtRet.Rows[i]["serienf"].ToString();
                    eNf.CdFornecedor = Convert.ToInt32(DtRet.Rows[i]["CdFornec"].ToString());
                    eNf.ChaveAcessoNfe = DtRet.Rows[i]["TxChAcessoNfe"].ToString();
                    eNf.TpNfe = DtRet.Rows[i]["TpNFe"].ToString();
                    eNf.cUf = int.Parse(FuncoesGerais.UfIbgeEmpresa(int.Parse(DtRet.Rows[i]["id_loja"].ToString())));
                    eNf.TpAmbiente = FuncoesGerais.TipoAmbiente();
                    ConsultaNfe.Enviar(eNf, out eNf);
                }
                TmSemRetorno.Enabled = true;
            }
            catch (Exception Ex)
            {
                TmSemRetorno.Enabled = true;
                mLog.InsertErroLog("Erro em consulta de nota fiscal. \nErro encontrado:" + NFuncoes.TiraCampos(Ex.Message.ToString()));
            }
        }
        #endregion

        /// <summary>
        /// O Xml de Cliente e Gerado a cada seis minutos, e a busca ocorre buscanda as notas emitidas nos ultimos 60 dias.
        /// Para gerar os xml do cliente é preciso da chave de acesso no item
        /// , as vezes fica null, e preciso dar um update pegando da nfsaida ou entrada, e colocando CdRetorno=103 no LoteNFe.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void TmXmlCliente_Tick(object sender, EventArgs e)
        {
            Model_XmlCliente mXmlCliente = new Model_XmlCliente();
            NegocioFuncoesGerais ng = new NegocioFuncoesGerais();

            var DtXml = mXmlCliente.NotasFiscaisSemEnvio();

            for (int i = 0; i < DtXml.Rows.Count; i++)
            {
                try
                {
                    if (DtXml.Rows[i]["XmlProtNFe"].ToString() != string.Empty)
                    {
                        mXmlCliente.IncluirXmlCliente(DtXml.Rows[i]["TxChAcessoNFe"].ToString(), Convert.ToInt32(DtXml.Rows[i]["id_loja"]), ng.XmlnfeProc(DtXml.Rows[i]["xmlNFe"].ToString(), DtXml.Rows[i]["xmlprotNFe"].ToString()));
                    }
                    else
                    {
                        mXmlCliente.UpdateLoteNFe(Convert.ToInt32(DtXml.Rows[i]["id_loja"]), Convert.ToInt32(DtXml.Rows[i]["NrLote"]));
                    }
                }
                catch
                {
                    continue;
                }
            }
        }

        private void sBtnCancelamento_Click(object sender, EventArgs e)
        {
            if (TmCancelamento.Enabled == false)
            {
                TmCancelamento.Enabled = true;
                
            }
            else
            {
                TmCancelamento.Enabled = false;
            }
        }

        private void sCartaEletronica_Click(object sender, EventArgs e)
        {
            if (TmCartaEletronica.Enabled == false)
            {
                TmCartaEletronica.Enabled = true;
            }
            else
            {
                TmCartaEletronica.Enabled = false;
            }
        }

        private void TmCartaEletronica_Tick(object sender, EventArgs e)
        {

            TmCartaEletronica.Enabled = false;

            EnviarCartaEletronica EnvCarta = new EnviarCartaEletronica();
            Entidade_CCe ObjEntCCe = new Entidade_CCe();
            EnvCarta.Enviar(ObjEntCCe, out ObjEntCCe);

            TmCartaEletronica.Enabled = true;
        }

        private void TmEnviarEmailCliente_Tick(object sender, EventArgs e)
        {
            EnviarEmail EnvEmail = new EnviarEmail();
            Model_Email mEmail = new Model.Model_Email();
            mLog = new Model_LogNfe();
            NFuncoes = new NegocioFuncoesGerais();


            mXmlCliente = new Model.Model_XmlCliente();


            TmEnviarEmailCliente.Enabled = false;

            if (mEmail.ExisteTipoEmail("N"))
            {
                var DtNotas = mXmlCliente.ConsultaNotasFiscais();

                foreach (DataRow rows in DtNotas.Rows)
                {
                    try
                    {
                        if (rows["NmEmailParaEnvioNFe"] != null && rows["NmEmailParaEnvioNFe"].ToString().Trim() != string.Empty)
                        {
                            XmlDocument docXml = new XmlDocument();

                            docXml.LoadXml(rows["XmlCliente"].ToString());

                            if (EnvEmail.SendEmail(rows["NmEmailParaEnvioNFe"].ToString(), "Xml - Nota Fiscal [" + rows["TxChAcessoNFe"].ToString().Trim() + "]", string.Empty, MontarHtml(rows["serienf"].ToString() + "|" + rows["NrNf"].ToString(), rows["TxChAcessoNFe"].ToString(), rows["NmRazaoSocial"].ToString(), rows["NmCliente"].ToString()), docXml, rows["TxChAcessoNFe"].ToString(), int.Parse(rows["id_loja"].ToString())))
                            {
                                mXmlCliente.AtualizarEnvioXmlCliente(rows["TxChAcessoNFe"].ToString(), int.Parse(rows["id_loja"].ToString()), true);
                            }
                        }
                    }
                    catch (Exception Ex)
                    {
                        TmEnviarEmailCliente.Enabled = true;
                        mLog.InsertErroLog("Erro ao enviar o Email.\nErro encontrado:\n" + NFuncoes.TiraCampos(Ex.Message.ToString()) + "\nChave de Acesso:" + rows["TxChAcessoNFe"].ToString() + "\nEmail:" + rows["NmEmailParaEnvioNFe"].ToString());
                        mXmlCliente.EmailNaoEnviados(int.Parse(rows["id_loja"].ToString()), int.Parse(rows["NrNf"].ToString()), rows["serienf"].ToString().Trim());
                        continue;
                    }
                }
            }
            TmEnviarEmailCliente.Enabled = true;

        }
        public string MontarHtml(string NotaFiscal, string ChaveAcesso, string Empresa, string Cliente)
        {
            StringBuilder Sb = new StringBuilder();

            Sb.Append("<style type=\"text/css\">");
            Sb.Append("h1,h2{font-family: 'Arial';color: #35BDB2;}");
            Sb.Append("h3{color:#fff;background: #DF5555;}");
            Sb.Append("p{font-family: 'Arial';}");
            Sb.Append(".cordestaque{color: #35BDB2;}");
            Sb.Append(".dvLargura{height:200px;width: 550px;}");
            Sb.Append("i{font-family: 'Arial';color:#DF5555}");
            Sb.Append("</style>");
            //Informações
            Sb.Append("<div class=\"dvLargura\">");
            Sb.Append("<h1>" + Empresa + "</h1>");
            Sb.Append("<hr>");
            Sb.Append("<p>Segue em anexo o arquivo XML referente à NF-e descrimada abaixo</p>");
            Sb.Append("<p>Nota Fiscal: <span class='cordestaque'>" + NotaFiscal + "</span></p>");
            Sb.Append("<p>Cliente: <span class='cordestaque'>" + Cliente + "</span></p>");
            Sb.Append("<p>Chave de Acesso: <span class='cordestaque'>" + ChaveAcesso + "</span></p>");
            Sb.Append("<p class='cordestaque'>Para alterar seu e-mail de recepção entre em contato através do e-mail</p><br>");
            Sb.Append("<p><i>Obs:</i></p>");
            Sb.Append("<p><i>Serviço de e-mail automático</i></p>");
            Sb.Append("<p><i>Para alterar seu e-mail de recepção entre em contato</i></p>");

            return Sb.ToString();

        }
    }
}
