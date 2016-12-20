using Nfe.Entidade;
using Npgsql;
using System.Data;

namespace Nfe.Model
{
    public class Model_DownloadNFe
    {

        NpgsqlTransaction BeginTrans;

        public DataTable ConsultaNFeDownload()
        {
            return BancoDados.Consultar("SELECT txchacessonfe FROM itemmanifestacao WHERE codmanifestacao=210200 AND codretorno=135 limit 10");
        }


        public void IncluirDownloadNFe(int IdLoja ,string ChAcessoNFe, string XmlNFe)
        {
            try
            {
                BancoDados.OpenConection();
                NpgsqlCommand command;
                BeginTrans = BancoDados.conexao.BeginTransaction();

                command = new NpgsqlCommand("spincluirdownloadnfe", BancoDados.conexao);
                command.Transaction = BeginTrans;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("id_loja", IdLoja);
                command.Parameters.AddWithValue("txchacessonfe", ChAcessoNFe);
                command.Parameters.AddWithValue("xmlnfe", XmlNFe);
                command.ExecuteNonQuery();

                BeginTrans.Commit();
            }
            catch (System.Exception)
            {
                BeginTrans.Rollback();
                throw;
            }
        }
    }
}
