using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_ConsNFDest
    {
        public int id { get; set; }
        public int id_loja { get; set; }
        public int codstatus { get; set; }
        public DateTime dhresp { get; set; }
        public string indcont { get; set; }
        public List<Entidade_ItemConsNFDest> ListItemConsDest { get; set; }
    }
}
