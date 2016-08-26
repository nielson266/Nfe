using System;
using System.Reflection;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.IO;
using Npgsql;

namespace Nfe.Model
{
    public static class BancoDados
    {
        
        public static NpgsqlConnection conexao = new NpgsqlConnection();
        
        static NpgsqlCommand command;
        static NpgsqlDataAdapter adapter;
        static NpgsqlDataReader Reader;

        static int maxrows;
        static decimal valorEmpresa;
        static bool FlExiste;

        static string coluna;
        static string valor;

        public static void OpenConection()
        {

            if (conexao.State == ConnectionState.Closed)
                try
                {
                    conexao.ConnectionString = ConfigurationManager.ConnectionStrings["BancoDados"].ToString();
                    conexao.Open();
                }
                catch(Exception Ex)
                {
                    //StreamWriter vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
                    //vWriter.WriteLine("OCORREU O SEGUINTE EERRO: " + Ex.Message.ToString());
                    //vWriter.Flush();
                    //vWriter.Close();
                }
        }
        public static void CloseConection()
        {
            if (conexao.State == ConnectionState.Open)
                conexao.Close();
        }

        public static void InsertAlterarExcluir(string qryStr)
        {
            OpenConection();
            command = new NpgsqlCommand(qryStr, conexao);
            command.ExecuteNonQuery();
            CloseConection();
        }
        public static void InsertAlterarStoredProcedure (string NomeStoredProcedure,Object objProc)
        {
            OpenConection();
            command = new NpgsqlCommand(NomeStoredProcedure, conexao);
            command.CommandType = CommandType.StoredProcedure;

            foreach (PropertyInfo fInfo in objProc.GetType().GetProperties())
            {
                coluna = fInfo.Name;
                valor =  objProc.GetType().GetProperty(fInfo.Name).GetValue(objProc, null).ToString();
                if ((valor != string.Empty) && (valor != "0")) 
                    command.Parameters.AddWithValue(coluna, valor);
            }

            command.ExecuteNonQuery();
            CloseConection();
        }
        public static void StoredProcedure(string NomeStoredProcedure, string QryString)
        {
            OpenConection();
            command = new NpgsqlCommand();
            command.Connection = conexao;
            command.CommandText = NomeStoredProcedure + " " + QryString;
            command.ExecuteNonQuery();
            CloseConection();
        }
        public static DataTable Consultar(string QrySql)
        {
            DataTable Table = new DataTable();
            OpenConection();
            adapter = new NpgsqlDataAdapter(QrySql, conexao);

            adapter.Fill(Table);

            CloseConection();

            return Table;
        }
        public static int maxId(string qryStr)
        {
            try
            {
                OpenConection();
                command = new NpgsqlCommand(qryStr, conexao);
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    if (!Reader.IsDBNull(0))
                        maxrows = Convert.ToInt32(Reader[0]) + 1;
                    else
                        maxrows = 0;
                }
                Reader.Close();

            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message.ToString());
            }
            finally
            {
                CloseConection();
            }
            return maxrows;
        }
        public static decimal valorTabela(string qryStr)
        {

            try
            {
                OpenConection();
                command = new NpgsqlCommand(qryStr, conexao);
                Reader = command.ExecuteReader();
                while (Reader.Read())
                {
                    if (!Reader.IsDBNull(0))
                        valorEmpresa = Convert.ToDecimal(Reader[0]);
                    else
                        valorEmpresa = 0;
                }
                Reader.Close();

            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message.ToString());
            }
            finally
            {
                CloseConection();
            }
            return valorEmpresa;
        }
        public static bool CodigoExiste(string qryStr)
        {
            try
            {
                OpenConection();
                command = new NpgsqlCommand(qryStr, conexao);
                Reader = command.ExecuteReader();

                if(Reader.HasRows)
                    FlExiste= true;
                else
                    FlExiste= false;

                Reader.Close();

            }
            catch (Exception Ex)
            {
                throw new Exception(Ex.Message.ToString());
            }
            finally
            {
                CloseConection();
            }
            return FlExiste;
        }
    }
}
