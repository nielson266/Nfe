using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Nfe.Entidade;
using System.IO;
using Npgsql;

namespace Nfe.Model
{
    public class FuncoesGerais
    {
        public FuncoesGerais()
        {
        }

        static DataTable dtReturn;
        static NpgsqlDataReader dtReader;

        public enum TipoTramitacao
        {
            EnvioNotFiscalSaida,
            EnvioNotFiscalEntrada,
            ReenvioLote,
            Cancelamento,
            Inutilizacao,
            ConsultaNfe,
            SolicitacaoConsultaNfe,
            SolicitacaoCancelamento,
            SolicitacaoInutilizacao,
            AutorizacaoNFe,
            Indefinido
        }

        public TipoTramitacao EnumTipoTramitacaoNfe;

        public bool TramitacaoNfe(TipoTramitacao TramitacaoNfe, int Loja, int NotaFiscal, string SerieNf, int Fornecedor)
        {
            try
            {
                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("SpIncluirTramitacaoNFe", BancoDados.conexao);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id_loja", Loja);
                command.Parameters.AddWithValue("@serienf", SerieNf);
                command.Parameters.AddWithValue("@NrNf", NotaFiscal);
                command.Parameters.AddWithValue("@CdFornec", Fornecedor);
                command.Parameters.AddWithValue("@FlErro", 0);
                if (TramitacaoNfe == TipoTramitacao.SolicitacaoCancelamento)
                {
                    command.Parameters.AddWithValue("@TxTramitacao", "Cancelamento Solicitado");
                    command.Parameters.AddWithValue("@cdStatusNFe", 6);
                }
                else if (TramitacaoNfe == TipoTramitacao.Cancelamento)
                {
                    command.Parameters.AddWithValue("@TxTramitacao", "Cancelamento Autorizado");
                    command.Parameters.AddWithValue("@cdStatusNFe", 7);
                }
                else if (TramitacaoNfe == TipoTramitacao.SolicitacaoConsultaNfe)
                {
                    command.Parameters.AddWithValue("@TxTramitacao", "Consulta de Nfe Solicitado.");
                    command.Parameters.AddWithValue("@cdStatusNFe", 3);
                }
                else if (TramitacaoNfe == TipoTramitacao.ConsultaNfe)
                {
                    command.Parameters.AddWithValue("@TxTramitacao", "Nota Fiscal Autorizada");
                    command.Parameters.AddWithValue("@cdStatusNFe", 11);
                }
                else if (TramitacaoNfe == TipoTramitacao.SolicitacaoInutilizacao)
                {
                    command.Parameters.AddWithValue("@TxTramitacao", "Inutilizacao Solicitada");
                    command.Parameters.AddWithValue("@cdStatusNFe", 6);
                }
                else if (TramitacaoNfe == TipoTramitacao.Inutilizacao)
                {
                    command.Parameters.AddWithValue("@TxTramitacao", "Inutilizacao Autorizada");
                    command.Parameters.AddWithValue("@cdStatusNFe", 6);
                }
                else if (TramitacaoNfe == TipoTramitacao.AutorizacaoNFe)
                {
                    command.Parameters.AddWithValue("@TxTramitacao", "Atualizando NFe de acordo com a SEFAZ");
                    command.Parameters.AddWithValue("@cdStatusNFe", 6);
                }
                else if (TramitacaoNfe == TipoTramitacao.Indefinido)
                {
                    command.Parameters.AddWithValue("@TxTramitacao", "Retorno indefinido");
                    command.Parameters.AddWithValue("@cdStatusNFe", 6);
                }
                else
                {
                    command.Parameters.AddWithValue("@TxTramitacao", "");
                    command.Parameters.AddWithValue("@cdStatusNFe", 6);
                }


                command.ExecuteNonQuery();

                BancoDados.CloseConection();

                return true;

            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();
                StreamWriter vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
                vWriter.Flush();
                vWriter.Close();
                return false;
            }
        }
        public static string TipoAmbiente()
        {
            var result = BancoDados.Consultar("SELECT VALOR FROM ConfiguracaoNfe WHERE Chave='AMBIENTE'");

            return result.Rows[0][0].ToString().ToUpper();
        }
        public static string TipoEmissao()
        {
            var result = BancoDados.Consultar("SELECT VALOR FROM ConfiguracaoNfe  WHERE Chave='TIPOEMISSAO'");

            return result.Rows[0][0].ToString().ToUpper();
        }
        public static string ParamatroTributacaoEmpresa(int loja = 1)
        {
            var result = BancoDados.Consultar("SELECT tipo_regime FROM LOJA WHERE id=" + loja);

            return result.Rows[0][0].ToString().Trim().ToUpper();
        }
        public static DataTable LojasEmitentes()
        {
            try
            {
                dtReturn = new DataTable();

                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM SpNfeConsultaLojasEmitentes()", BancoDados.conexao);
                command.CommandType = CommandType.Text;
                dtReader = command.ExecuteReader();

                dtReturn.Load(dtReader);

                BancoDados.CloseConection();

                return dtReturn;
            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();
                throw new Exception(ex.Message.ToString());
            }
        }
        public static string CertificadoEmpresa(int Loja)
        {
            try
            {
                dtReturn = new DataTable();

                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM SpNfeConsultaLojasEmitentes() WHERE idloja=@idloja", BancoDados.conexao);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("idloja", Loja);
                dtReader = command.ExecuteReader();

                dtReturn.Load(dtReader);

                BancoDados.CloseConection();

                return dtReturn.Rows[0]["NmCertificadoDigital"].ToString();
            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();
                throw new Exception(ex.Message.ToString());
            }
        }
        public static string UfIbgeEmpresa(int Loja)
        {
            try
            {
                dtReturn = new DataTable();

                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM SpNfeConsultaLojasEmitentes() WHERE idloja=@idloja", BancoDados.conexao);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("idloja", Loja);
                dtReader = command.ExecuteReader();

                dtReturn.Load(dtReader);

                BancoDados.CloseConection();

                return dtReturn.Rows[0]["id_uf"].ToString().ToUpper();

            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();
                throw new Exception(ex.Message.ToString());
            }
        }
    }
}
