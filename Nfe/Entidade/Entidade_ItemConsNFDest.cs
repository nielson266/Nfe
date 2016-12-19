using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_ItemConsNFDest
    {
        public int id { get; set; }
        public int id_consdest { get; set; }
        public string nsu { get; set; }
        public string chacessonfe { get; set; }
        public string cnpjcpf { get; set; }
        public string nomeemitente { get; set; }
        public DateTime dtemissao { get; set; }
        public string tpnfe { get; set; }
        public decimal vlnfe { get; set; }
        public DateTime dhautorizacao { get; set; }
        public string sitnfe { get; set; }
        public string sit_manifestacao_dest { get; set; }
    }
}
