using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nfe.Entidade;

namespace Nfe.Entidade
{
    public class Entidade_Transportador:Entidade_Pessoa
    {
        public string IE { get; set; }
        public Entidade_Endereco Ent_Transportador { get; set; }
    }
}
