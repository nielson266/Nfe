using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfe.Entidade;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Npgsql;
namespace Nfe.Model
{
    public class Model_NotaFiscal : IPersistencia<Entidade_BaseNotaFiscal>
    {
        DataTable Dt;
        NpgsqlDataReader dtReader;
        public enum NotaFiscal
        {
            Entrada,
            Saida
        }

        public NotaFiscal TipoNotaFiscal { get; set; }
        public bool Incluir(Entidade_BaseNotaFiscal ObjDados)
        {
            return false;
        }
        public bool Salvar(Entidade_BaseNotaFiscal ObjDados)
        {
            return false;
        }
        public bool Deletar(Entidade_BaseNotaFiscal ObjDados)
        {
            return false;
        }
        public DataTable Pesquisar()
        {
            return null;
        }
        public DataTable Pesquisar(int obj)
        {
            return null;
        }
        public DataTable Pesquisar(string obj)
        {
            return null;
        }
        public DataTable ConsultarNotasFiscalSemRetornoOuDuplicadas()
        {
            try
            {
                Dt = new DataTable();

                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("SpConsultaNFesSemRetorno", BancoDados.conexao);
                command.CommandType = CommandType.StoredProcedure;
                dtReader = command.ExecuteReader();

                Dt.Load(dtReader);

                BancoDados.CloseConection();
                return Dt;
            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();
                StreamWriter vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
                vWriter.Flush();
                vWriter.Close();
                return null;
            }
        }

        public void AtualizaRetornoNfeSaida(string Retorno, string ProtAutoriz, int loja, string ChAcesso, NotaFiscal TipoNf)
        {
            if (TipoNf == NotaFiscal.Saida)
            {
                if (string.IsNullOrEmpty(ProtAutoriz))
                {
                    BancoDados.InsertAlterarExcluir(" UPDATE  NfSaida SET CdRetorno = " + Retorno + ", NrProtocoloAutorizNfe = NULL \n" +
                                                    " WHERE id_loja = " + loja + " AND TxChAcessoNfe = '" + ChAcesso.Trim() + "'");
                }
                else
                {
                    BancoDados.InsertAlterarExcluir(" UPDATE  NfSaida SET CdRetorno = " + Retorno + ", NrProtocoloAutorizNfe = " + ProtAutoriz + " \n" +
                                                    " WHERE id_loja = " + loja + " AND TxChAcessoNfe = '" + ChAcesso.Trim() + "'");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(ProtAutoriz))
                {
                    BancoDados.InsertAlterarExcluir(" UPDATE  NfEntrada SET CdRetorno = " + Retorno + ", NrProtocoloAutorizNfe = NULL \n" +
                                                    " WHERE id_loja = " + loja + " AND TxChAcessoNfe = '" + ChAcesso.Trim() + "'");
                }
                else
                {
                    BancoDados.InsertAlterarExcluir(" UPDATE  NfEntrada SET CdRetorno = " + Retorno + ", NrProtocoloAutorizNfe = " + ProtAutoriz.Trim() + " \n" +
                                                    " WHERE id_loja = " + loja + " AND TxChAcessoNfe = '" + ChAcesso.Trim() + "'");
                }
            }
        }
        public DataTable ConsultarNotasFiscais(int pLoja, int pNotaFiscal, string pSerieNf, int CodFornec, NotaFiscal TipoNF)
        {

            try
            {
                Dt = new DataTable();

                BancoDados.OpenConection();

                if (TipoNF == NotaFiscal.Saida)
                {
                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM vw_nfesaida WHERE cdloja=@id_loja AND nrnf=@nrnf AND nmserienf=@serienf", BancoDados.conexao);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("id_loja", pLoja);
                    command.Parameters.AddWithValue("nrNF", pNotaFiscal);
                    command.Parameters.AddWithValue("serienf", pSerieNf);
                    dtReader = command.ExecuteReader();
                }
                else
                {
                    NpgsqlCommand command = new NpgsqlCommand("SpConsultaNFeEntrada", BancoDados.conexao);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_loja", pLoja);
                    command.Parameters.AddWithValue("@nrNF", pNotaFiscal);
                    command.Parameters.AddWithValue("@serienf", pSerieNf);
                    command.Parameters.AddWithValue("@CdFornec", CodFornec);
                    dtReader = command.ExecuteReader();
                }

                Dt.Load(dtReader);

                BancoDados.CloseConection();
                return Dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }

        }
        public DataTable ConsultarNotasFiscalItens(int pLoja, int pNotaFiscal, string pSerieNf, int CodFornec, NotaFiscal TipoNF)
        {
            try
            {
                Dt = new DataTable();

                BancoDados.OpenConection();

                if (TipoNF == NotaFiscal.Saida)
                {
                    NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM vw_itemnfesaida WHERE cdloja=@id_loja AND nrnf=@nrnf AND nmserienf=@serienf", BancoDados.conexao);
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("id_loja", pLoja);
                    command.Parameters.AddWithValue("nrNF", pNotaFiscal);
                    command.Parameters.AddWithValue("serienf", pSerieNf);
                    dtReader = command.ExecuteReader();
                }
                else
                {
                    NpgsqlCommand command = new NpgsqlCommand("SpConsultaNFeItensEntrada", BancoDados.conexao);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_loja", pLoja);
                    command.Parameters.AddWithValue("@nrNF", pNotaFiscal);
                    command.Parameters.AddWithValue("@serienf", pSerieNf);
                    command.Parameters.AddWithValue("@CdFornec", CodFornec);
                    dtReader = command.ExecuteReader();
                }

                Dt.Load(dtReader);

                BancoDados.CloseConection();
                return Dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public DataTable ConsultarNotasFiscalReferidaEntrada(int pLoja, int pNotaFiscal, string pSerieNf, int pCdFornec)
        {
            try
            {
                Dt = new DataTable();

                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("SpConsultaReferidaNFeEntrada", BancoDados.conexao);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id_loja", pLoja);
                command.Parameters.AddWithValue("@nrNF", pNotaFiscal);
                command.Parameters.AddWithValue("@serienf", pSerieNf);
                command.Parameters.AddWithValue("@CdFornec", pCdFornec);
                command.Parameters.AddWithValue("@FlImportacao", 0);
                dtReader = command.ExecuteReader();

                Dt.Load(dtReader);

                BancoDados.CloseConection();
                return Dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public DataTable ConsultarNotasFiscalReferidaSaida(int pLoja, int pNotaFiscal, string pSerieNf)
        {
            try
            {
                Dt = new DataTable();

                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("dbo.SpConsultaReferidaNFeSaida", BancoDados.conexao);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id_loja", pLoja);
                command.Parameters.AddWithValue("@nrNF", pNotaFiscal);
                command.Parameters.AddWithValue("@serienf", pSerieNf);
                dtReader = command.ExecuteReader();

                Dt.Load(dtReader);

                BancoDados.CloseConection();
                return Dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public DataTable ConsultarNotasFiscalSaidaDuplicatas(int pLoja, int pNotaFiscal, string pSerieNf)
        {
            try
            {
                Dt = new DataTable();

                BancoDados.OpenConection();

                if (pSerieNf != "D1")
                {
                    NpgsqlCommand command = new NpgsqlCommand("SpConsultaDuplicatasNFeSaida", BancoDados.conexao);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_loja", pLoja);
                    command.Parameters.AddWithValue("@nrNF", pNotaFiscal);
                    command.Parameters.AddWithValue("@serienf", pSerieNf);
                    dtReader = command.ExecuteReader();
                }
                else
                {
                    NpgsqlCommand command = new NpgsqlCommand("SpConsultaNFeSaidaRecebCaixa", BancoDados.conexao);
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@id_loja", pLoja);
                    command.Parameters.AddWithValue("@nrNF", pNotaFiscal);
                    command.Parameters.AddWithValue("@serienf", pSerieNf);
                    dtReader = command.ExecuteReader();
                }

                Dt.Load(dtReader);

                BancoDados.CloseConection();
                return Dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message.ToString());
            }
        }
        public int IncluirLoteNfeSaida(int pLoja, NotaFiscal TipoNF)
        {
            try
            {
                Dt = new DataTable();

                BancoDados.OpenConection();
                NpgsqlCommand command;

                if (TipoNF == NotaFiscal.Saida)
                {
                    command = new NpgsqlCommand("SpIncluirLoteNFeSaida", BancoDados.conexao);
                }
                else
                {
                    command = new NpgsqlCommand("SpIncluirLoteNFeEntrada", BancoDados.conexao);
                }
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("loja", pLoja);
                //command.Parameters.Add("NextNrLote", NpgsqlTypes.NpgsqlDbType.Integer).Direction = ParameterDirection.Output;
                //command.ExecuteNonQuery();

                var Id = command.ExecuteScalar();

                BancoDados.CloseConection();
                //return Convert.ToInt32(command.Parameters["@NextNrLote"].Value);
                return Convert.ToInt32(Id);
            }
            catch (Exception ex)
            {
                
                throw new Exception(ex.Message.ToString());
            }
        }
        public DataTable PesquisarLoteNotaFiscais(int pLoja, int pLote)
        {
            return BancoDados.Consultar(" SELECT NrLote, CdRetorno, id_loja, id_participante, serienf, NrNf, TpNFe, xmlNFe, TxChAcessoNfe " +
                                        " FROM   ItemLoteNFe " +
                                        " WHERE  (NrLote = " + pLote + ") AND (id_loja = " + pLoja + ")");
        }
        /// <summary>
        /// Update da chave de acesso e codigo da nfe
        /// </summary>
        /// <param name="NotaFiscal"></param>
        /// <param name="Serie"></param>
        /// <param name="Loja"></param>
        /// <param name="fornecedor"></param>
        /// <param name="ChAcessoNfe"></param>
        /// <param name="CodigoNFe"> E o código randomico gerado</param>
        /// <returns></returns>
        public bool UpdateChavedeAcessocNfe(int NotaFiscal, string Serie, int Loja, int fornecedor, string ChAcessoNfe, string CodigoNFe)
        {
            if (fornecedor == 0)
                BancoDados.InsertAlterarExcluir("UPDATE NfSaida SET TxChAcessoNfe='" + ChAcessoNfe + "', CNf = " + CodigoNFe + " WHERE NrNf =" + NotaFiscal + " AND SerieNf = '" + Serie.Trim() + "' AND id_Loja = " + Loja);
            else
                BancoDados.InsertAlterarExcluir("UPDATE NfEntrada SET TxChAcessoNfe='" + ChAcessoNfe + "', CNf = " + CodigoNFe + " WHERE NrNf =" + NotaFiscal + " AND SerieNf = '" + Serie.Trim() + "' AND id_Loja = " + Loja + " AND CdFornec = " + fornecedor);

            return true;
        }
        public void UpdateNfErro(int retorno, int loja, string Serie, int notafiscal, int CodFornec, NotaFiscal TipoNF)
        {
            if (TipoNF == NotaFiscal.Saida)
            {
                BancoDados.InsertAlterarExcluir(" UPDATE    NfSaida \n" +
                                                " SET              CdRetorno = " + retorno + " \n" +
                                                " WHERE     (id_loja = " + loja + ") AND (SerieNf = '" + Serie + "') AND (NrNf = " + notafiscal + ")");
            }
            else
            {
                BancoDados.InsertAlterarExcluir(" UPDATE    NfEntrada \n" +
                                                " SET              CdRetorno = " + retorno + " \n" +
                                                " WHERE     (id_loja = " + loja + ") AND (serienf = '" + Serie + "') AND (NrNf = " + notafiscal + ") AND CdFornec = " + CodFornec);
            }
        }
        public void UpdateNfRetornoAutorizado(int retorno, string ProtAutoriz ,int loja, string ChAcessoNfe ,NotaFiscal TipoNF)
        {
            if (TipoNF == NotaFiscal.Saida)
            {
                
                BancoDados.InsertAlterarExcluir(" UPDATE    NfSaida \n" +
                                                " SET              CdRetorno = " + retorno + ", NrProtocoloAutorizNfe = " + ProtAutoriz + " \n" +
                                                " WHERE     (id_loja = " + loja + ") AND (TxChAcessoNFe = '" + ChAcessoNfe + "')");
            }
            else
            {
                BancoDados.InsertAlterarExcluir(" UPDATE    NfEntrada \n" +
                                                " SET              CdRetorno = " + retorno + ", NrProtocoloAutorizNfe = " + ProtAutoriz + " \n" +
                                                " WHERE     (id_loja = " + loja + ") AND (TxChAcessoNFe = '" + ChAcessoNfe + "')");
            }
        }
        public void UpdateNfInutilizacaoCancelamento(int retorno, string ProtAutoriz, int loja, string ChAcessoNfe, NotaFiscal TipoNF)
        {
            if (TipoNF == NotaFiscal.Saida)
            {
                BancoDados.InsertAlterarExcluir(" UPDATE    NfSaida \n" +
                                                " SET              CdRetorno = " + retorno + ", NrProtocoloCancelInutilizNfe = " + ProtAutoriz + " \n" +
                                                " WHERE     (id_loja = " + loja + ") AND (TxChAcessoNFe = '" + ChAcessoNfe + "')");
            }
            else
            {
                BancoDados.InsertAlterarExcluir(" UPDATE    NfEntrada \n" +
                                                " SET              CdRetorno = " + retorno + ", NrProtocoloCancelInutilizNfe = " + ProtAutoriz + " \n" +
                                                " WHERE     (id_loja = " + loja + ") AND (TxChAcessoNFe = '" + ChAcessoNfe + "')");
            }
        }
        public void UpdateNfRetorno(int retorno, string ProtAutoriz, int loja, string ChAcessoNfe, NotaFiscal TipoNF)
        {
            if (TipoNF == NotaFiscal.Saida)
            {
                BancoDados.InsertAlterarExcluir(" UPDATE    NfSaida \n" +
                                                " SET              CdRetorno = " + retorno + " \n" +
                                                " WHERE     (id_loja = " + loja + ") AND (TxChAcessoNFe = '" + ChAcessoNfe + "')");
            }
            else
            {
                BancoDados.InsertAlterarExcluir(" UPDATE    NfEntrada \n" +
                                                " SET              CdRetorno = " + retorno + " \n" +
                                                " WHERE     (id_loja = " + loja + ") AND (TxChAcessoNFe = '" + ChAcessoNfe + "')");
            }
        }
        public void UpdateNfTpEmisVersaoNFe(string tpEmis, string conting, string versao, string nrlote, string notafiscal, string loja, string serie, int CodFornec, NotaFiscal TipoNF)
        {
            if (TipoNF == NotaFiscal.Saida)
            {
                if (string.IsNullOrEmpty(conting))
                {
                    BancoDados.InsertAlterarExcluir(" UPDATE NfSaida \n" +
                                                    " SET tpEmis= " + tpEmis + ", cdConting= NULL, NrVersaoNFe = '" + versao + "' \n" +
                                                    " WHERE  (NrLote = " + nrlote + ") AND (NrNf = " + notafiscal + ") AND (id_loja = " + loja + ") AND (serienf = '" + serie + "')");
                }
                else
                {
                    BancoDados.InsertAlterarExcluir(" UPDATE NfSaida \n" +
                                                    " SET tpEmis= " + tpEmis + ", cdConting= " + conting + ", NrVersaoNFe = '" + versao + "' \n" +
                                                    " WHERE  (NrLote = " + nrlote + ") AND (NrNf = " + notafiscal + ") AND (id_loja = " + loja + ") AND (serienf = '" + serie + "')");
                }
            }
            else
            {
                if (string.IsNullOrEmpty(conting))
                {
                    BancoDados.InsertAlterarExcluir(" UPDATE NfEntrada \n" +
                                                    " SET tpEmis= " + tpEmis + ", cdConting= NULL, NrVersaoNFe = '" + versao + "' \n" +
                                                    " WHERE  (NrLote = " + nrlote + ") AND (NrNf = " + notafiscal + ") AND (id_loja = " + loja + ") AND (serienf = '" + serie + "') AND (CdFornec = " + CodFornec + ")");
                }
                else
                {
                    BancoDados.InsertAlterarExcluir(" UPDATE NfEntrada \n" +
                                                    " SET tpEmis= " + tpEmis + ", cdConting= " + conting + ", NrVersaoNFe = '" + versao + "' \n" +
                                                    " WHERE  (NrLote = " + nrlote + ") AND (NrNf = " + notafiscal + ") AND (id_loja = " + loja + ") AND (serienf = '" + serie + "') AND (CdFornec = "+ CodFornec + ")");
                }
            }
        }
    }
}
