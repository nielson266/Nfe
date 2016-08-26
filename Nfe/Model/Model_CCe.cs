using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nfe.Model
{
    public class Model_CCe
    {
        public DataTable ConsultaCartas()
        {
            return BancoDados.Consultar("SELECT top 20 C.id_loja,C.Id_cce_lote,NrNf,serienf,CdRetorno,Desc_Correcao,Dt_Aprovacao,TxChAcessoNfe, \n" +
                                        "(SELECT Valor FROM ConfiguracaoNFe where Chave= 'AMBIENTE') as TipoEmissao, Dt_Prot_Nfe, \n" +
                                        "L.CdCpfCgc, dbo.FgIbgeCidadeCodigo_porcdUfcdCidade(L.cduf, L.cdcidade) as cdUfCidadeIbge_Empresa, NrSeqEnvio \n" +
                                        "FROM CCe C  \n" +
                                        "INNER JOIN ItemCCe I On C.Id_cce_lote = I.Id_cce_lote \n" +
                                        "INNER JOIN Loja L On C.id_loja = L.id_loja \n" +
                                        "WHERE \n" +
                                        "CdRetorno is null");
        }

        public bool UpdateRetornoCCe(int Id_Lote_CCe, string ChAcessoNFe, int CdRetorno, DateTime DtAprov)
        {
            BancoDados.InsertAlterarExcluir("UPDATE ItemCCe SET CdRetorno=" + CdRetorno + ", Dt_Aprovacao = '"+ string.Format("{0:MM/dd/yyyy hh:mm:ss}", DtAprov) +"' WHERE id_cce_lote =" + Id_Lote_CCe + " AND TxChAcessoNFe = '" + ChAcessoNFe.Trim() + "'");
            return true;
        }
        public bool UpdateRetornoCCe(int Id_Lote_CCe, int CdRetorno, string ChAcessoNFe)
        {
            BancoDados.InsertAlterarExcluir("UPDATE ItemCCe SET CdRetorno=" + CdRetorno + " WHERE id_cce_lote =" + Id_Lote_CCe + " AND TxChAcessoNFe = '" + ChAcessoNFe.Trim() + "'");
            return true;
        }

        public bool UpdateRetornoCCeConsSit(int NrSeqLote, int CdRetorno, string ChAcessoNFe)
        {
            BancoDados.InsertAlterarExcluir("UPDATE ItemCCe SET CdRetorno=" + CdRetorno + " WHERE NrSeqEnvio =" + NrSeqLote + " AND TxChAcessoNFe = '" + ChAcessoNFe.Trim() + "'");
            return true;
        }
    }
}
