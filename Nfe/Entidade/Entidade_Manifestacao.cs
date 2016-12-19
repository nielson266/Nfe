using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_Manifestacao
    {
        public int id { get; set; }
        public int id_loja { get; set; }
        public DateTime dtManifestacao { get; set; }
        public string tipo_ambiente { get; set; }
        public List<Entidade_ItemManifestacao> ListNfeManifestacao { get; set; }
    }
}
