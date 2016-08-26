using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_Endereco
    {
        public string Lagradouro { get; set; }
        public string Numero { get; set; }
        public string Complemento { get; set; }
        public string Telefone { get; set; }
        public string Bairro { get; set; }
        public int cMunicipio { get; set; }
        public string sMunicipio { get; set; }
        public string sUf { get; set; }
        public string sCep { get; set; }
        public int cPais { get; set; }
        public string sPais { get; set; }
        public string IE { get; set; }
        public bool FlIsento { get; set; }
        public string IESub { get; set; }
    }
}
