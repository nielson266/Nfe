using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace Nfe.Model
{
    public class Model_XmlCliente
    {
        public Model_XmlCliente()
        {

        }

        public DataTable NotasFiscaisSemEnvio()
        {
            return BancoDados.Consultar(" SELECT n.nrnf,n.serienf,n.id_loja,it.xmlNFe,it.xmlprotNFe,L.NrRecibo,it.TxChAcessoNFe,IT.NrLote \n" +
                                        " FROM NfSaida N \n" +
                                        " INNER JOIN ItemLoteNfe IT on n.nrnf = it.nrnf and n.serienf = it.serienf and n.id_loja = it.id_loja and n.nrlote = it.nrlote \n" +
                                        " INNER JOIN LoteNFe L on L.id_loja = IT.id_loja AND L.NrLote = IT.NrLote \n" +
                                        " WHERE \n" + 
                                        " DtEmissao >= DateAdd(d,-60,GetDate()) \n" +
                                        " AND IT.CdRetorno = 100 \n"+
                                        " AND N.CdRetorno = 100 \n" +
                                        " AND IT.xmlcliente is null ");
        }

        public bool IncluirXmlCliente(string ChAcessoNFe,int Loja, string XmlCliente)
        {
            BancoDados.InsertAlterarExcluir(" UPDATE ItemLoteNFe SET XmlCliente='" + XmlCliente + "'\n" +
                                            " WHERE id_loja =" + Loja + " AND TxChAcessoNfe = '" + ChAcessoNFe + "'");
            return true;
        }

        public bool UpdateLoteNFe(int Loja, int nrlote)
        {
            BancoDados.InsertAlterarExcluir("UPDATE LoteNFe SET CdRetorno=103 WHERE id_loja =" + Loja + " AND NrLote =" + nrlote +"");
            return true;
        }

        public DataTable ConsultaNotasFiscais()
        {
            return BancoDados.Consultar(" SELECT top 50 it.XmlCliente,N.NmCliente,it.TxChAcessoNFe,n.NrNf,N.serienf,N.id_loja,it.TpNFe,C.NmEmailParaEnvioNFe,L.NmRazaoSocial,L.NmLoja,L.NmEnder,L.NrEnder,L.NmBairro\n" +
                                        " FROM NfSaida n () \n"+
                                        " INNER JOIN ItemLoteNfe it () on n.nrnf = it.nrnf and n.serienf = it.serienf and n.id_loja = it.id_loja and n.nrlote=it.nrlote \n"+
                                        " INNER JOIN Cliente c on n.cdcpfcgc = c.cdcpfcgc \n" +
                                        " INNER JOIN Loja L on N.id_loja = L.id_loja \n"+
                                        " WHERE \n"+
                                        " n.serienf = (SELECT serienfe FROM Loja l () WHERE L.id_loja = N.id_loja ) \n"+
                                        " AND n.cdretorno=100 \n"+ 
                                        " AND it.cdretorno=100 \n"+
                                        " AND xmlcliente is not null \n"+
                                        " AND it.FlXmlEnviado is null \n" +
                                        " AND C.NmEmailParaEnvioNFe is not null \n" +
                                        " AND LTRIM(RTRIM(C.NmEmailParaEnvioNFe)) != '' \n" +
                                        " AND NOT EXISTS ( SELECT * FROM NFEmailSemEnvio NFS WHERE N.id_loja = NFS.id_loja AND N.NrNf = NFS.NrNF AND N.serienf= NFS.serienf) \n" +
                                        " ORDER BY dtemissao");
        }
        public bool AtualizarEnvioXmlCliente(string ChAcessoNFe, int Loja,bool FlXmlEnviado)
        {
            BancoDados.InsertAlterarExcluir(" UPDATE ItemLoteNFe SET FlXmlEnviado='" + FlXmlEnviado + "'\n" +
                                            " WHERE id_loja =" + Loja + " AND TxChAcessoNfe = '" + ChAcessoNFe + "'");
            return true;
        }

        public bool EmailNaoEnviados(int Loja, int NotaFiscal, string SerieNf)
        {
            BancoDados.InsertAlterarExcluir(" INSERT INTO NFEmailSemEnvio VALUES (" + Loja + "," + NotaFiscal + ",'" + SerieNf + "')");

            return true;
        }
    }
}
