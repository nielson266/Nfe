using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_Inutilizacao:Entidade_BaseNotaFiscal
    {
        public int CdOperacao { get; set; }        
        public int NrIni { get; set; }
        public int NrFim { get; set; }
        public Entidade_Usuario Usuario { get; set; }
    }
}
