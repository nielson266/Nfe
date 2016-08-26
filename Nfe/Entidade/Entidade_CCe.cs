using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nfe.Entidade
{
    public class Entidade_CCe
    {
        public int Id_CCe_Lote { get; set; }
        public int id_loja { get; set; }
        public DateTime Dt_CCe { get; set; }
        public int Qt_Nf { get; set; }
        public int cUF { get; set; }
        public string TipoAmbiente { get; set; }
        public List<Entidade_ItemCCe> ItemCCe { get; set; }

    }
}
