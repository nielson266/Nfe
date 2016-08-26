using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_Duplicatas
    {
        public string NumeroDup { get; set; }
        public DateTime DataVencimento { get; set; }
        public decimal ValorDup { get; set; }
        public int TipoPagamento { get; set; }
    }
}
