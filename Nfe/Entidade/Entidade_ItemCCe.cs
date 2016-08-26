using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nfe.Entidade
{
    public class Entidade_ItemCCe
    {
        public int Id_item_cce { get; set; }
        public int Id_cce_lote { get; set; }
        public int id_loja { get; set; }
        public string CdCpfCgc { get; set; }
        public int NrNf { get; set; }
        public string serienf { get; set; }
        public string TipoNf { get; set; }
        public int CdRetorno  { get; set; }
        public string XmlCCe { get; set; }
        public string Desc_Correcao { get; set; }
        public DateTime Dt_Aprovacao { get; set; }
        public DateTime Dt_Prot_Nfe { get; set; }
        public int CdUfCidadeIbge_Empresa { get; set; }
        public string ProtocoloAutorizacao { get; set; }
        public string TpEmis { get; set; }
        public string Ambiente { get; set; }
        public string TxChAcessoNfe { get; set; }
        public int NrSeqEnvio { get; set; }
        public int NrSeqCount { get; set; }
    }
}
