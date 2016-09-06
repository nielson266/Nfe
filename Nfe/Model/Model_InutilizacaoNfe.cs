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
    public class Model_InutilizacaoNfe : IPersistencia<Entidade_Inutilizacao>
    {
        DataTable dtReturn;
        NpgsqlDataReader dtReader;
        StringBuilder sbMsgErro;
        public bool Incluir(Entidade_Inutilizacao ObjDados)
        {
            return false;
        }
        public bool Salvar(Entidade_Inutilizacao ObjDados)
        {
            BancoDados.InsertAlterarExcluir("UPDATE InutilizacaoNFe Set CdRetorno=" + ObjDados.CdRetorno + ",TxChAcessoInutilizNfe='" + ObjDados.ChaveAcessoNfe + "' WHERE numero_ini = " + ObjDados.NrIni + " AND numero_fim = " + ObjDados.NrFim +"  AND id_loja = " + ObjDados.Loja);
            return true;
        }
        public bool Deletar(Entidade_Inutilizacao ObjDados)
        {
            return false;
        }
        public DataTable Pesquisar()
        {
            try
            {
                dtReturn = new DataTable();

                BancoDados.OpenConection();

                NpgsqlCommand command = new NpgsqlCommand("SELECT * FROM vw_inutilizacaonfe", BancoDados.conexao);
                command.CommandType = CommandType.Text;
                dtReader = command.ExecuteReader();

                dtReturn.Load(dtReader);

                BancoDados.CloseConection();

                return dtReturn;
            }
            catch (SqlException ex)
            {
                sbMsgErro = new StringBuilder();
                for (int i = 0; i < ex.Errors.Count; i++)
                {
                    sbMsgErro.Append("Index #" + i + "\n" +
                                     "Mensagem: " + ex.Errors[i].Message + "\n" +
                                     "LineNumber: " + ex.Errors[i].LineNumber + "\n" +
                                     "Source: " + ex.Errors[i].Source + "\n" +
                                     "Procedimento: " + ex.Errors[i].Procedure + "\n");
                }
                BancoDados.CloseConection();
                StreamWriter vWriter = new StreamWriter(@"c:\C:\Nota_Fiscal_Eletronica\ErroTransacional\ServicoCancelamentoNfe.txt", true);
                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + sbMsgErro.ToString());
                vWriter.Flush();
                vWriter.Close();
                return null;
            }
            catch (Exception ex)
            {
                BancoDados.CloseConection();
                StreamWriter vWriter = new StreamWriter(@"c:\C:\Nota_Fiscal_Eletronica\ErroTransacional\ServicoCancelamentoNfe.txt", true);
                vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + ex.Message.ToString());
                vWriter.Flush();
                vWriter.Close();
                return null;
            }
        }
        public DataTable Pesquisar(int obj)
        {
            return null;
        }
        public DataTable Pesquisar(string obj)
        {
            return null;
        }
    }
}
