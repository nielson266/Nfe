using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_Empresa:Entidade_Endereco
    {
        public string Nome { get; set; }
        public string RazaoSocial { get; set; }
        public string Uf { get; set; }
        public int cUf { get; set; }


    }
}
