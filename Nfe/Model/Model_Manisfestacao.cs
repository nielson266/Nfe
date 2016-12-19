using Nfe.Entidade;
using Npgsql;
using System;
using System.Data;

namespace Nfe.Model
{
    public class Model_Manisfestacao
    {
        NpgsqlTransaction BeginTrans;
        public DataTable ConsultaManifestacao()
        {
            return BancoDados.Consultar("SELECT m.*,itm.id as iditem,itm.idseq,itm.txchacessonfe,itm.codmanifestacao " +
                                        "FROM manifestacaonfe m "+
                                        "INNER JOIN itemmanifestacao itm on m.id = itm.idmanifestacao "+
                                        "WHERE codretorno is null");
        }

        public void UpdateManifestacao(int idManifestacao, int codretorno, string ProtReb,DateTime dhRegEvento)
        {
            BancoDados.InsertAlterarExcluir(" UPDATE ItemManifestacao " +
                                            " SET  codretorno =" + codretorno + "," +
                                            " dtregmanifestacao = '" + string.Format("{0:dd/MM/yyyy HH:mm:ss}", dhRegEvento)  + "'," +
                                            " nprot = '" + ProtReb + "'" +
                                            " WHERE id =" + idManifestacao);
        }

        public bool ConsultaChaveNfeExiteConsultaDest(string chaveacesso)
        {
            return BancoDados.CodigoExiste("SELECT * FROM itemconsultanfdest WHERE chacessonfe='" + chaveacesso + "'");
        }

        public void InsertConsNFDest(Entidade_ConsNFDest ObjConsNFDest)
        {
            BancoDados.OpenConection();
            NpgsqlCommand command;
            BeginTrans = BancoDados.conexao.BeginTransaction();

            command = new NpgsqlCommand("spincluirconsultadestinatario", BancoDados.conexao);
            command.Transaction = BeginTrans;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("codstatus", ObjConsNFDest.codstatus);
            command.Parameters.AddWithValue("dhresp", ObjConsNFDest.dhresp);
            command.Parameters.AddWithValue("indcont", ObjConsNFDest.indcont);

            var Id = command.ExecuteScalar();
            
            foreach (var item in ObjConsNFDest.ListItemConsDest)
            {
                command = new NpgsqlCommand("spincluiritemconsultanfdest", BancoDados.conexao);
                command.Transaction = BeginTrans;
                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("id_consdest", Convert.ToInt32(Id));
                command.Parameters.AddWithValue("nsu", item.nsu);
                command.Parameters.AddWithValue("chacessonfe", item.chacessonfe);
                command.Parameters.AddWithValue("cnpjcpf", item.cnpjcpf.Trim());
                command.Parameters.AddWithValue("nomeemitente", item.nomeemitente);
                command.Parameters.AddWithValue("dtemissao", item.dtemissao);
                command.Parameters.AddWithValue("tpnfe", item.tpnfe);
                command.Parameters.AddWithValue("vlnfe", item.vlnfe);
                command.Parameters.AddWithValue("dhautorizacao", item.dhautorizacao);
                command.Parameters.AddWithValue("sitnfe", item.sitnfe);
                command.Parameters.AddWithValue("sit_manifestacao_dest", item.sit_manifestacao_dest);

                command.ExecuteNonQuery();
            }

            BeginTrans.Commit();

            BancoDados.CloseConection();
        }
    }
}
