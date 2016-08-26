using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_Pessoa:Entidade_Endereco
    {
        public string Nome { get; set; }
        public string RazaoSocial { get; set; }
        public string CpfCnpj { get; set; }

    }
}
