using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nfe.Negocio.Geral
{
     public class Mensagem
    {
         public enum TipoMensagem
         {
             Nfe,
             RetAutoriz,
             Inutilizacao,
             Cancelamento,
             Reenvio,
             Status,
             XmlLoteGerados,
             CartaEletronica
         }

         public TipoMensagem EnumTipoMensagem;
         public Mensagem()
         {
             
         }
         public static void MensagemErro(TipoMensagem SelecioneMensagem, string TipoNotaFiscal,string Mensagem)
         {
             StreamWriter vWriter;

             if (!Directory.Exists(@"c:\MensagensNFe\"))
             {
                 Directory.CreateDirectory(@"c:\MensagensNFe\");
             }

             if (SelecioneMensagem == TipoMensagem.Nfe)
             {
                 vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoNotaFiscal.txt", true);
                 vWriter.WriteLine("Servico Nfe: " + DateTime.Now.ToString());
                 vWriter.WriteLine("Servico Nfe: " + Mensagem);
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.XmlLoteGerados)
             {
                 vWriter = new StreamWriter(@"c:\MensagensNFe\LotesGerados.txt", true);
                 vWriter.WriteLine("Lote: " + DateTime.Now.ToString());
                 vWriter.WriteLine("Lote: " + Mensagem);
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.RetAutoriz)
             {
                 vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoNotaFiscal.txt", true);
                 vWriter.WriteLine("Servico RetAutoriz: " + DateTime.Now.ToString());
                 vWriter.WriteLine("Servico RetAutoriz: " + Mensagem);
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.Status)
             {
                 vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoNotaFiscal.txt", true);
                 vWriter.WriteLine("Servico Status: " + DateTime.Now.ToString());
                 vWriter.WriteLine("Servico Status: " + Mensagem);
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.Inutilizacao)
             {
                 vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoInutilizacao.txt", true);
                 vWriter.WriteLine("Servico Inutilizacao: " + DateTime.Now.ToString());
                 vWriter.WriteLine("Servico Inutilizacao: " + Mensagem);
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.Reenvio)
             {
                 vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoReenvio.txt", true);
                 vWriter.WriteLine("Servico Reenvio: " + DateTime.Now.ToString());
                 vWriter.WriteLine("Servico Reenvio: " + Mensagem);
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.Cancelamento)
             {
                 vWriter = new StreamWriter(@"c:\MensagensNFe\ServicoCancelamento.txt", true);
                 vWriter.WriteLine("Servico Cancelamento: " + DateTime.Now.ToString());
                 vWriter.WriteLine("Servico Cancelamento: " + Mensagem);
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.CartaEletronica)
             {
                 vWriter = new StreamWriter(@"c:\MensagensNFe\CartaEletronica.txt", true);
                 vWriter.WriteLine("Servico Carta Eletronica: " + DateTime.Now.ToString());
                 vWriter.WriteLine("Servico Carta Eletronica: " + Mensagem);
                 vWriter.Flush();
                 vWriter.Close();
             }
         }
         public static void MensagemServicoIniciado(TipoMensagem SelecioneMensagem, string TipoNotaFiscal, string Mensagem)
         {
             if (SelecioneMensagem == TipoMensagem.Nfe)
             {
                 StreamWriter vWriter = new StreamWriter(@"c:\ServicoNotaFiscal.txt", true);
                 vWriter.WriteLine("Servico Iniciado: " + DateTime.Now.ToString());
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.Inutilizacao)
             {
                 StreamWriter vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
                 vWriter.WriteLine("Servico Iniciado: " + DateTime.Now.ToString());
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.Reenvio)
             {
                 StreamWriter vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
                 vWriter.WriteLine("Servico Iniciado: " + DateTime.Now.ToString());
                 vWriter.Flush();
                 vWriter.Close();
             }
             else if (SelecioneMensagem == TipoMensagem.Cancelamento)
             {
                 StreamWriter vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
                 vWriter.WriteLine("Servico Iniciado: " + DateTime.Now.ToString());
                 vWriter.Flush();
                 vWriter.Close();
             }
         }
    }
}
