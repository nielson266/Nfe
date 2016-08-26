using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Nfe.Negocio.Geral
{
    public class UTF8StringWriter : StringWriter
    {
        public override Encoding Encoding
        {
            get
            {
                return Encoding.UTF8;
            }
        }
    }
}
