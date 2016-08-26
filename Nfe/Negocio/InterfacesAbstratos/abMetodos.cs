using Nfe.Negocio.Geral;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Nfe.Negocio.InterfacesAbstratos
{
    public abstract class abMetodos
    {
        public XmlDocument docTran;

        public XmlDocument docEnviNfe;

        public X509Certificate2 CertEmpresa;

        public StreamWriter vWriter;

        public XmlSerializerNamespaces ns;

        public XmlWriterSettings Settings;

        public XmlSerializer xmlStatus;

        public UTF8StringWriter Sw;

        public XmlWriter Wx;

        public XmlNodeReader retXmlNodeReader;

        public XmlSerializer xmlDesSerializar;

        public XmlNodeList nodeList;

        public XmlNodeList nodeListNfe;

        public XmlNodeList nodeListCarregarNfe;

        public XmlNode nodeStatus;

        public UrlNfesEstados UrlEstados;
    }
}
