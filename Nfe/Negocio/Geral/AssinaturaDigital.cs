using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

using System.Xml;
using System.IO;
using System.Security.Cryptography.Xml;

namespace Nfe.Negocio.Geral
{
    class AssinaturaDigital
    {
        public AssinaturaDigital()
        {
        }

        public static X509Certificate2 FindCertOnStore(int loja)
        {
            X509Store st = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            X509Certificate2 cert2 = new X509Certificate2();
            st.Open(OpenFlags.ReadOnly | OpenFlags.OpenExistingOnly);
            string CertificadoEmpresa;
            CertificadoEmpresa = Model.FuncoesGerais.CertificadoEmpresa(loja);

            foreach (var cert in st.Certificates)
            {
                int ret = string.Compare(cert.SerialNumber.ToUpper(), CertificadoEmpresa.ToUpper());

                if (ret == 0)
                {
                    cert2 = cert;
                    break;
                }
            }
            return cert2;
        }

        public static XmlDocument SignXml(XmlDocument Doc, X509Certificate2 Cert, string RefUri)
        {
            try
            {
                //StreamWriter vWriter;
                //vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
                //vWriter.WriteLine("Iniciando a assinatura.");
                //vWriter.Flush();
                //vWriter.Close();

                Reference reference = new Reference();
                SignedXml signedXml = new SignedXml(Doc);
                KeyInfoX509Data keyInfData = new KeyInfoX509Data(Cert);
                KeyInfo kInfo = new KeyInfo();

                // PEGANDO O URI
                XmlAttributeCollection _Uri = Doc.GetElementsByTagName(RefUri).Item(0).Attributes;
                foreach (XmlAttribute _atributo in _Uri)
                {
                    if (_atributo.Name == "Id")
                    {
                        reference.Uri = "#" + _atributo.InnerText;
                    }
                }

                // Passamos a chave :)
                signedXml.SigningKey = Cert.PrivateKey;

                // Adicionamos uma transformação enveloped à referencia.
                XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
                reference.AddTransform(env);
                XmlDsigC14NTransform c14 = new XmlDsigC14NTransform();
                reference.AddTransform(c14);

                // Adicionamos a referência ao objecto SignedXml.
                signedXml.AddReference(reference);

                kInfo.AddClause(keyInfData);

                signedXml.KeyInfo = kInfo;

                // Assinamos.
                signedXml.ComputeSignature();

                XmlElement xmlDigitalSignature = signedXml.GetXml();

                XmlElement retXmlElem = Doc.GetElementsByTagName(RefUri)[0] as XmlElement;

                retXmlElem.ParentNode.AppendChild(Doc.ImportNode(xmlDigitalSignature, true));
                //vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
                //vWriter.WriteLine("Finalizando a assinatura.");
                //vWriter.Flush();
                //vWriter.Close();

                return Doc;
            }
            catch (Exception ex)
            {
                //StreamWriter vWriter = new StreamWriter(@"c:\ServicoCancelamentoNfe.txt", true);
                //vWriter.WriteLine("OCORREU O SEGUINTE EERRO AO ASSINAR O XML: " + ex.Message.ToString());
                //vWriter.Flush();
                //vWriter.Close();
                return null;
            }
        }
    }
}
