
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using Nfe.Model;
using System.IO;
using System.Xml;

namespace Nfe.Negocio.EnviarEmail
{
    public class EnviarEmail
    {
        Model_Email mEmail;
        public bool SendEmail(string Destinatarios, string cabecalho, string copia, string Body, XmlDocument DocXmlCliente, string ChaveNFe, int Loja)
        {
            MailMessage Messagem = new MailMessage();
            mEmail = new Model_Email();

            if (!Directory.Exists(@"c:\NFe\DistribuicaoNFe\" + Loja + @"\"))
            {
                Directory.CreateDirectory(@"c:\NFe\DistribuicaoNFe\" + Loja + @"\");
            }
            //Salvando xml
            DocXmlCliente.Save(@"c:\NFe\DistribuicaoNFe\" + Loja + @"\" + ChaveNFe + ".xml");

            var retEmail = mEmail.PesquisaTipoEmail("N"); // TIPO N: Email da Nota Fiscal Eletronica

            //Anexando Xml
            Attachment anexar = new Attachment(@"c:\NFe\DistribuicaoNFe\" + Loja + @"\" + ChaveNFe + ".xml");

            //Email Cliente
            Messagem.Bcc.Add(Destinatarios);
            if (copia != string.Empty)
                Messagem.CC.Add(copia);
            // Assunto
            Messagem.Subject = cabecalho;
            Messagem.Body = Body;
            Messagem.Attachments.Add(anexar);
            Messagem.From = new MailAddress(retEmail.Email, retEmail.Email);
            Messagem.SubjectEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            Messagem.BodyEncoding = System.Text.Encoding.GetEncoding("ISO-8859-1");
            Messagem.IsBodyHtml = true;
            SmtpClient SmtpCli = new SmtpClient(retEmail.Smtp, retEmail.Porta);
            SmtpCli.Credentials = new System.Net.NetworkCredential(retEmail.Usuario, retEmail.Senha);
            SmtpCli.Send(Messagem);

            return true;
        }
    }
}
