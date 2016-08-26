using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Npgsql;

namespace Nfe.Model
{
    class Model_Cancelamento
    {
        DataTable dtReturnNfCancalemento;
        NpgsqlDataReader dtReaderCancelamento;
        public Model_Cancelamento()
        {
        }

        public DataTable ConsultarCancelementosSolicitados()
        {
            try
            {
                dtReturnNfCancalemento = new DataTable();

                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("SpConsultaCancelamentosSolicitados", BancoDados.conexao);
                command.CommandType = CommandType.StoredProcedure;
                dtReaderCancelamento = command.ExecuteReader();

                dtReturnNfCancalemento.Load(dtReaderCancelamento);

                BancoDados.CloseConection();

                return dtReturnNfCancalemento;
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
        public bool UpdateCancelamentoNfe(int cdRetorno,DateTime DataOperacao,int loja,string SerieNf,string notafiscal,int cdFornec)
        {
            try
            {
                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand(" UPDATE CancelamentoNFe SET CdRetorno = @CdRetorno" +
                                                    " WHERE (id_loja = @id_loja) AND (serienf = @serienf) AND (NrNf = @NrNf) AND (CdFornec = @CdFornec)", BancoDados.conexao);
                command.Parameters.AddWithValue("@CdRetorno", cdRetorno);
                command.Parameters.AddWithValue("@id_loja", loja);
                command.Parameters.AddWithValue("@serienf", SerieNf);
                command.Parameters.AddWithValue("@NrNf", notafiscal);
                command.Parameters.AddWithValue("@CdFornec", cdFornec);

                command.ExecuteNonQuery();

                BancoDados.CloseConection();

                return true;
            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();
                StreamWriter vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoCancelamentoNfe.txt", true);
                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
                vWriter.Flush();
                vWriter.Close();

                return false;
            }
        }
        public bool UpdateNfSaidaCancelamento(int cdRetorno, string ProtocoloAutorizacao, int loja, string SerieNf, string notafiscal)
        {
            try
            {
                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand(" UPDATE NfSaida SET CdRetorno = @CdRetorno, NrProtocoloCancelInutilizNfe = @NrProtocoloCancelInutilizNfe " +
                                                    " WHERE  (id_loja = @id_loja) AND (serienf = @serienf) AND (NrNf = @NrNf)", BancoDados.conexao);
                command.Parameters.AddWithValue("@CdRetorno", cdRetorno);
                command.Parameters.AddWithValue("@NrProtocoloCancelInutilizNfe", ProtocoloAutorizacao == null ? 0 : long.Parse(ProtocoloAutorizacao));
                command.Parameters.AddWithValue("@id_loja", loja);
                command.Parameters.AddWithValue("@serienf", SerieNf);
                command.Parameters.AddWithValue("@NrNf", notafiscal);

                command.ExecuteNonQuery();

                BancoDados.CloseConection();

                return true;
            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();
                StreamWriter vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoCancelamentoNfe.txt", true);
                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
                vWriter.Flush();
                vWriter.Close();

                return false;
            }
        }
        public bool UpdateNfEntradaCancelamento(int cdRetorno, string ProtocoloAutorizacao, int loja, string SerieNf, string notafiscal,int CdFornecedor)
        {
            try
            {
                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand(" UPDATE NfEntrada SET CdRetorno = @CdRetorno, NrProtocoloCancelInutilizNfe  = @NrProtocoloCancelInutilizNfe " +
                                                    " WHERE (id_loja = @id_loja) AND (serienf=@serienf) AND  (NrNf =@NrNf) AND (CdFornec = @CdFornec)", BancoDados.conexao);
                command.Parameters.AddWithValue("@CdRetorno", cdRetorno);
                command.Parameters.AddWithValue("@NrProtocoloCancelInutilizNfe", ProtocoloAutorizacao);
                command.Parameters.AddWithValue("@id_loja", loja);
                command.Parameters.AddWithValue("@serienf", SerieNf);
                command.Parameters.AddWithValue("@NrNf", notafiscal);
                command.Parameters.AddWithValue("@CdFornec", CdFornecedor);

                command.ExecuteNonQuery();

                BancoDados.CloseConection();

                return true;
            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();

                StreamWriter vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoCancelamentoNfe.txt", true);
                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
                vWriter.Flush();
                vWriter.Close();
                return false;
            }
        }
        public void TramitacaoNfe(int Loja, string SerieNf,string NotaFiscal,int CdFornecedor,int Erro,string txtTramitacao)
        {
            try
            {
                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("SpIncluirTramitacaoNFe", BancoDados.conexao);
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@id_loja", Loja);
                command.Parameters.AddWithValue("@serienf", SerieNf);
                command.Parameters.AddWithValue("@NrNf", NotaFiscal);
                command.Parameters.AddWithValue("@CdFornec", CdFornecedor);
                command.Parameters.AddWithValue("@FlErro", Erro);
                command.Parameters.AddWithValue("@TxTramitacao", txtTramitacao);
                command.Parameters.AddWithValue("@cdStatusNFe", 6);

                command.ExecuteNonQuery();

                BancoDados.CloseConection();

            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();
                StreamWriter vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoCancelamentoNfe.txt", true);
                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
                vWriter.Flush();
                vWriter.Close();
            }
        }
    }
}
