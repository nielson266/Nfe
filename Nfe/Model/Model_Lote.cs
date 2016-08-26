using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Model
{
    public class Model_Lote
    {
        public DataTable LotesNaoProcessados()
        {
            return BancoDados.Consultar(" SELECT L.id_loja, CdRetorno, DtEnvio, DtRecSefaz, DtResultProcSefaz, NrLote, NrRecibo, NrTempoProcSefaz, StLote, TpNFe, Uf \n" +
                                        " FROM LoteNFe LT () INNER JOIN Loja L () ON LT.id_loja = L.id_loja \n " +
                                        " WHERE (CdRetorno IN (103, 105)) and DtRecSefaz >='05/01/2015'");
        }
        public void UpdateLoteRecebidos(string StatusLote, DateTime RecSefaz, string NrTempoProcSefaz, string NumeroRecibo, string Retorno, string NrLote, int Loja)
        {
            BancoDados.InsertAlterarExcluir(" UPDATE LoteNFe \n" +
                                            " SET NrRecibo = '" + NumeroRecibo.Trim() + "', StLote = '" + StatusLote.Trim() + "', CdRetorno= " + Retorno + ",  DtRecSefaz = '" + string.Format("{0:MM/dd/yyyy hh:mm:ss}", RecSefaz.Date) + "', NrTempoProcSefaz = '" + NrTempoProcSefaz.Trim() + "' \n" +
                                            " WHERE (NrLote = " + NrLote + ") AND (id_loja = "+ Loja +")");
        }

        public void UpdateXmlTpSaida(int loja, int nf, string serienf, int lote, string tpemis, string xml,int Fornecedor = 0)
        {
            BancoDados.InsertAlterarExcluir(" UPDATE ItemLoteNFe \n" +
                                            " SET xmlNFe = '"+ xml +"', tpEmis = "+ tpemis +" \n" +
                                            " WHERE  (id_loja = "+ loja +") AND (serienf = '" + serienf +"') AND (NrNf = " + nf + ") AND (NrLote = " + lote+ ")" + (Fornecedor == 0 ? " " : " AND CdFornec = " + Fornecedor ));
        }
        public void UpdateChAcessoItemLote(int loja, int nf, string serienf, int lote, string chAcessoNfe)
        {
            BancoDados.InsertAlterarExcluir(" UPDATE ItemLoteNFe \n" +
                                            " SET TxChAcessoNfe = '" + chAcessoNfe + "' \n" +
                                            " WHERE (NrNf = "+ nf +") AND (serienf = '"+ serienf +"') AND (id_loja = " + loja +")  AND (NrLote = "+ lote +")"); 
        }
        public bool LoteProcessado(string Retorno, DateTime DtProcessamento, string Recibo)
        {
            BancoDados.InsertAlterarExcluir(" UPDATE LoteNFe "+
                                            " SET StLote = 'P', CdRetorno =" + Retorno +", DtResultProcSefaz = '"+ string.Format("{0:MM/dd/yyyy HH:mm:ss}", DtProcessamento) +"' "+
                                            " WHERE (NrRecibo = " + Recibo + ")");

            return true;
        }
        public bool ItemLoteNfe(string Retorno, string ChAcesso, string XmlProtNFe)
        {
            BancoDados.InsertAlterarExcluir(" UPDATE ItemLoteNFe \n"+
                                            " SET CdRetorno = " + Retorno + ", xmlprotNFe = '" + XmlProtNFe + "'\n" +
                                            " WHERE TxChAcessoNfe = '" + ChAcesso + "' ");

            return true;
        }
        public void UpdateLoteErro(int retorno, int loja, string Serie, int notafiscal, int Fornecedor = 0)
        {
            BancoDados.InsertAlterarExcluir(" UPDATE    ItemLoteNFe \n" +
                                            " SET       CdRetorno = "+ retorno +" \n" +
                                            " WHERE     (id_loja = " + loja + ") AND (serienf = '" + Serie + "') AND (NrNf = " + notafiscal + ")" + (Fornecedor == 0 ? " " : " AND CdFornec = " + Fornecedor));
        }
    }
}
