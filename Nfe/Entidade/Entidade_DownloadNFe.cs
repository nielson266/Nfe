using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nfe.Entidade
{
    public class Entidade_DownloadNFe
    {
        public int id_loja { get; set; }
        public List<string> ChaveNFe { get; set; }

        public XmlDocument xmlNfe { get; set; }
    }
}
