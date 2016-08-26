using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_ItemNotaFiscal:Entidade_Totais
    {
        public int NrSeqItem { get; set; }
        public string CodigoProduto { get; set; }
        public string DescricaoProdutos { get; set; }
        public string Unidade { get; set; }
        public decimal Qtd { get; set; }
        public decimal PercMVA { get; set; }
        public decimal PercReducao { get; set; }
        public decimal ValorUnitario { get; set; }
        public string NCM { get; set; }
        public string Origem { get; set; }
        public string CFOP { get; set; }
        public string CSTICMS { get; set; }
        public string CSTPI { get; set; }
        public string CSTCOFINS { get; set; }
        public string CSTPIS { get; set; }
        public decimal Desconto { get; set; }
        public decimal PercentualRedICMS { get; set; }
        public decimal ValorMVAPercentual { get; set; }
    }
}
