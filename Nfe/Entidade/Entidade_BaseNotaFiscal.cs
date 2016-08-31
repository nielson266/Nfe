using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public abstract class Entidade_BaseNotaFiscal:Entidade_Loja
    {
        public int NotaFiscal { get; set; }
        public string sSerieNf { get; set; }
        public int CdFornecedor { get; set; }
        public DateTime DtEmissao { get; set; }
        public DateTime DtEntrada { get; set; }
        public int TpEmis { get; set; }
        public string TpAmbiente { get; set; }
        public string ChaveAcessoNfe { get; set; }
        public string cNF { get; set; }
        public string CdRetorno { get; set; }
        public int ModNfe { get; set; }
        public string TpNfe { get; set; }
        public string NrProtocoloAutorizNfe { get; set; }
        public string NaturezaOperacao { get; set; }
        public int TipoOperacao { get; set; }
        public int TipoPagamento { get; set; }
        public bool OpInterestadual { get; set; }
        /// <summary>
        /// Referice ao cDV da Nfe
        /// </summary>
        public int CdNfe { get; set; }
        public DateTime DataHoraConting { get; set; }

        public int NotaFiscalReferida { get; set; }
        public string SerieReferida { get; set; }
        public int ModReferida { get; set; }
        public string COORef { get; set; }
        public string NrEquipamento { get; set; }
        public string ChaveNfeReferida { get; set; }
        public string Observacao { get; set; }
        public int TipoFInalidadeNfe { get; set; }
        public int Lote { get; set; }
    }
}
