using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_Loja:Entidade_Empresa
    {
        public int Loja { get; set; }
        public string Cnpj { get; set; }
        public string DDD { get; set; } 
        public string TeleFone { get; set; }
        public string Email { get; set; }
        public string sCodCertificadoDigital { get; set; }
    }
}
