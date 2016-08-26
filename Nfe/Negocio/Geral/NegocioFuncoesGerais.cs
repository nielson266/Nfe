using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using Nfe.Model;

namespace Nfe.Negocio.Geral
{
    public class NegocioFuncoesGerais
    {
        public NegocioFuncoesGerais()
        {
        }

        string dados;
        Model_NotaFiscal mNF;
        Model_Lote mLote;

        #region Codigo do Ibge - enum
        enum CodigoIbge
        {
            Item11,
            Item12,
            Item13,
            Item14,
            Item15,
            Item16,
            Item17,
            Item21,
            Item22,
            Item23,
            Item24,
            Item25,
            Item26,
            Item27,
            Item28,
            Item29,
            Item31,
            Item32,
            Item33,
            Item35,
            Item41,
            Item42,
            Item43,
            Item50,
            Item51,
            Item52,
            Item53
        }
        #endregion

        public enum CodigoUF
        {

            /// <remarks/>
            AC,

            /// <remarks/>
            AL,

            /// <remarks/>
            AM,

            /// <remarks/>
            AP,

            /// <remarks/>
            BA,

            /// <remarks/>
            CE,

            /// <remarks/>
            DF,

            /// <remarks/>
            ES,

            /// <remarks/>
            GO,

            /// <remarks/>
            MA,

            /// <remarks/>
            MG,

            /// <remarks/>
            MS,

            /// <remarks/>
            MT,

            /// <remarks/>
            PA,

            /// <remarks/>
            PB,

            /// <remarks/>
            PE,

            /// <remarks/>
            PI,

            /// <remarks/>
            PR,

            /// <remarks/>
            RJ,

            /// <remarks/>
            RN,

            /// <remarks/>
            RO,

            /// <remarks/>
            RR,

            /// <remarks/>
            RS,

            /// <remarks/>
            SC,

            /// <remarks/>
            SE,

            /// <remarks/>
            SP,

            /// <remarks/>
            TO,
        }

        public void AssinaturaDigital(XmlDocument xmlAssinar)
        {
            //string retorno = teste.Substring(0, teste.Length - 5);

        }

        public bool ValidarEstruturaXml(string xml, string schema)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            XmlSchemaSet schemaset = new XmlSchemaSet();
            settings.ValidationType = ValidationType.Schema;
            settings.Schemas = schemaset;
            schemaset.Add("http://www.portalfiscal.inf.br/nfe", @"C:\Nfe\schema\" + schema + ".xsd");
            XmlReader xReader = XmlReader.Create(new StringReader(xml));
            XmlReader xReaderValidar = XmlReader.Create(xReader, settings);


            try
            {
                while (xReaderValidar.Read())
                {
                    xReaderValidar.ReadOuterXml();
                }
                return true;
            }
            catch (XmlException ex)
            {
                ex.Message.ToString();
                return false;
            }
        }

        public static int RetornoCodigoIbge(int IdIbge)
        {
            int i = 0;
            foreach (var item in Enum.GetValues(typeof(CodigoIbge)))
            {
                if (item.ToString().Substring(item.ToString().Count() - 2, 2) == IdIbge.ToString())
                {
                    break;
                }
                i += 1;
            }

            return i;
        }
        public static int RetornoCodigoUF(string Uf)
        {
            int i = 0;
            foreach (var item in Enum.GetValues(typeof(CodigoUF)))
            {
                if (item.ToString() == Uf.ToString())
                {
                    break;
                }
                i += 1;
            }
            return i;
        }
        public static int RetornoCodigoCFOP(string CFOP)
        {
            int i = 0;
            foreach (var item in Enum.GetValues(typeof(nfe.TCfop)))
            {
                if (item.ToString().Substring(item.ToString().Count() - 4, 4) == CFOP.ToString())
                {
                    break;
                }
                i += 1;
            }
            return i;
        }
        public string GerarChaveAcessoNfe(int Loja,int CodFornecedor,string Uf, DateTime DtEmissao, string CnpjEmissor, string ModNfe, string Serie, string NumeroNf, string TpEmis, int Lote, out string cNF, out string cDV)
        {
            mNF = new Model_NotaFiscal();
            mLote = new Model_Lote();

            Random rd = new Random();
            string CodigoRandom = string.Format("{0:00000000}",rd.Next(99999999));

            List<int> arrayMult = new List<int>() { 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            string ChaveNfe = Uf + string.Format("{0:yyMM}", DtEmissao) + TiraCampos(CnpjEmissor) + ModNfe + Serie.PadLeft(3, '0') + NumeroNf.PadRight(9, '0') + TpEmis + CodigoRandom;

            int[] arryaCh = ChaveNfe.Select(x => Int32.Parse(x.ToString())).ToArray();
            int soma = 0;
            int resto = 0;
            int DV = 0;            

            for (int i = 0; i < arryaCh.Length; i++)
            {
                soma += arrayMult[i] * Convert.ToInt32(arryaCh[i]);
            }

            resto = soma % 11;
            
            cNF = CodigoRandom;

            if (resto == 0 || resto == 1)
            {
                cDV = "0";

                if (CodFornecedor == 0)
                {
                    mNF.UpdateChavedeAcessocNfe(Convert.ToInt32(NumeroNf), Serie == "9" ? "D1" : Serie, Loja, 0, ChaveNfe + cDV.ToString(), CodigoRandom);
                    mLote.UpdateChAcessoItemLote(Loja, Convert.ToInt32(NumeroNf), Serie, Lote, ChaveNfe + cDV.ToString());
                }
                else
                {
                    mNF.UpdateChavedeAcessocNfe(Convert.ToInt32(NumeroNf), Serie == "9" ? "D1" : Serie, Loja, CodFornecedor, ChaveNfe + cDV.ToString(), CodigoRandom);
                    mLote.UpdateChAcessoItemLote(Loja, Convert.ToInt32(NumeroNf), Serie, Lote, ChaveNfe + cDV.ToString());
                }

                return ChaveNfe += "0";
            }
            else
            {
                DV = 11 - resto;

                cDV = DV.ToString();

                if (CodFornecedor == 0)
                {
                    mNF.UpdateChavedeAcessocNfe(Convert.ToInt32(NumeroNf), Serie == "9" ? "D1" : Serie, Loja, 0, ChaveNfe + DV.ToString(), CodigoRandom);
                    mLote.UpdateChAcessoItemLote(Loja, Convert.ToInt32(NumeroNf), Serie == "9" ? "D1" : Serie, Lote, ChaveNfe + DV.ToString());
                }
                else
                {
                    mNF.UpdateChavedeAcessocNfe(Convert.ToInt32(NumeroNf), Serie == "9" ? "D1" : Serie, Loja, CodFornecedor, ChaveNfe + DV.ToString(), CodigoRandom);
                    mLote.UpdateChAcessoItemLote(Loja, Convert.ToInt32(NumeroNf), Serie == "9" ? "D1" : Serie, Lote, ChaveNfe + DV.ToString());
                }
                return ChaveNfe += DV.ToString();
            }
        }
        public string TiraCampos(string valor)
        {
            dados = string.Empty;
            if (valor != string.Empty)
            {
                dados = valor.Replace("-", "");
                dados = dados.Replace(",", "");
                dados = dados.Replace(".", "");
                dados = dados.Replace("/", "");
                dados = dados.Replace("á", "a");
                dados = dados.Replace("ã", "a");
                dados = dados.Replace("(", "");
                dados = dados.Replace(")", "");
                dados = dados.Replace("é", "e");
                dados = dados.Replace("ú", "u");
                dados = dados.Replace("'", "");
            }

            return dados;
        }

        public string XmlnfeProc(string XmlNFe, string XmlprotNFe)
        {
            //nodeListNfe.Item(0).AppendChild(docEnviNfe.ImportNode(nodeListCarregarNfe.Item(0), true));

            XmlDocument docXml = new XmlDocument();

            docXml.LoadXml("<?xml version=\"1.0\"?><nfeProc versao=\"3.10\" xmlns=\"http://www.portalfiscal.inf.br/nfe\"></nfeProc>");

            XmlDocument xDocNFe = new XmlDocument();
            xDocNFe.LoadXml(XmlNFe);
            XmlDocument xDocProt = new XmlDocument();
            xDocProt.LoadXml(XmlprotNFe);

            XmlNode xnode = (XmlNode)xDocNFe;
            XmlNodeList xList = xnode.ChildNodes;

            XmlNode xnodeProt = (XmlNode)xDocProt;
            XmlNodeList xListProt = xnodeProt.ChildNodes;

            XmlNodeList xListNfeProc = docXml.GetElementsByTagName("nfeProc");

            xListNfeProc.Item(0).AppendChild(docXml.ImportNode(xList.Item(1),true));
            xListNfeProc.Item(0).AppendChild(docXml.ImportNode(xListProt.Item(0), true));

            return docXml.OuterXml;
        }
    }
}


