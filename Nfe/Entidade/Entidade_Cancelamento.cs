using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nfe.Entidade
{
    public class Entidade_Cancelamento
    {
        public Entidade_Cancelamento()
        {
        }

        public int id { get; set; }
        public int Loja { get; set; }
        public string CnpjCpf { get; set; }
        public string TpNf { get; set; }
        public string TpAmb { get; set; }
        public string NmSerie { get; set; }
        public int NrNf { get; set; }
        public int CdFornec { get; set; }
        
        public int cUf { get; set; }
        public string DataHora { get; set; }
        public string ChaveAcessoNfe { get; set; }
        public string Justificatica { get; set; }
        public string ProtocoloAutoriz { get; set; }
        public string Ambiente { get; set; }
        public string CodigoIbgeEmpresa { get; set; }
    }
}

