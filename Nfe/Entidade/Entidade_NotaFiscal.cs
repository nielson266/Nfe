using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Entidade
{
    public class Entidade_NotaFiscal : Entidade_BaseNotaFiscal
    {
        public Entidade_Emitente EntEmit { get; set; }
        public Entidade_Destinatario EntDest { get; set; }
        public Entidade_Totais EntTotais { get; set; }
        public List<Entidade_NotaFiscalReferida> NotaFiscalReferida { get; set; }
        public Entidade_LocalEntrega EntLocalEntrega { get; set; }
        public Entidade_Transportador EntTransportador { get; set; }
        public List<Entidade_Duplicatas> EntDuplicata { get; set; }
        public List<Entidade_ItemNotaFiscal> EntItemNotaFiscal { get; set; }
    }
}
