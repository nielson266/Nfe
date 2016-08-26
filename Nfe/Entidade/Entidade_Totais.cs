using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_Totais
    {
        public decimal ValorTotalProdutos { get; set; }
        public decimal ValorTotal { get; set; }
        public decimal ValorFrete { get; set; }
        public decimal ValorSeguro { get; set; }
        public decimal ValorDesconto { get; set; }
        public decimal ValorII { get; set; }
        public decimal ValorOutrasDesp { get; set; }
        public decimal ValorIcmsBaseSub { get; set; }
        public decimal ValorTotalIcms { get; set; }
        public decimal ValorBaseIcms { get; set; }
        public decimal ValorIcms { get; set; }
        public decimal ValorIcmsSub { get; set; }
        public decimal ValorBaseIpi { get; set; }
        public decimal ValorIpi { get; set; }
        public decimal ValorPis { get; set; }
        public decimal ValorCofins { get; set; }
        public decimal ValorAliqCofins { get; set; }
        public decimal ValorAliqPis { get; set; }
        public decimal AliqIcms { get; set; }
        public decimal AliqIpi { get; set; }
        public decimal AliqPis { get; set; }
        public decimal AliqCofins { get; set; }
    }
}
