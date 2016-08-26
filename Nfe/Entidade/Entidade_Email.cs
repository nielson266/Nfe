using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nfe.Entidade
{
    public class Entidade_Email
    {
        public int CdEmail { get; set; }
        public string Email { get; set; }
        public string Usuario { get; set; }
        public string Senha { get; set; }
        public string Smtp { get; set; }
        public int Porta { get; set; }
        public string TipoEmail { get; set; }
        public int id_loja { get; set; }
    }
}
