using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Nfe.Negocio.InterfacesAbstratos
{
    interface IMetodosBase<T>
    {
        void Enviar(T ObjEnt,out T objDados);
        void EnviarXml(XmlDocument doc, X509Certificate2 cert, ref T obj);
        void DeserilizarEvento(object obj, ref T objDes);

    }
}
