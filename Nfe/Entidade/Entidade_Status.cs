using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_Status
    {
        public int Loja { get; set; }
        public string versao { get; set; }
        public string tpAmbiente { get; set; }
        public string cUf { get; set; }
        public string Uf { get; set; }
        public string cStatus { get; set; }
        public string dhRet { get; set; }
        public string sMotivo { get; set; }

        public string AmbienteFormatado
        {
            get
            {
                if (tpAmbiente == "1")
                    return "PRO";
                else
                    return "HOM";
            }
        }
    }
}
