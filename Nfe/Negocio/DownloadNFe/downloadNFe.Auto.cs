﻿//------------------------------------------------------------------------------
// <auto-generated>
//     O código foi gerado por uma ferramenta.
//     Versão de Tempo de Execução:4.0.30319.42000
//
//     As alterações ao arquivo poderão causar comportamento incorreto e serão perdidas se
//     o código for gerado novamente.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This source code was auto-generated by xsd, Version=4.6.1055.0.
// 
namespace Nfe.downloadNFe {
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
    [System.Xml.Serialization.XmlRootAttribute("downloadNFe", Namespace="http://www.portalfiscal.inf.br/nfe", IsNullable=false)]
    public partial class TDownloadNFe {
        
        private TAmb tpAmbField;
        
        private TDownloadNFeXServ xServField;
        
        private string cNPJField;
        
        private string[] chNFeField;
        
        private TVerDownloadNFe versaoField;
        
        /// <remarks/>
        public TAmb tpAmb {
            get {
                return this.tpAmbField;
            }
            set {
                this.tpAmbField = value;
            }
        }
        
        /// <remarks/>
        public TDownloadNFeXServ xServ {
            get {
                return this.xServField;
            }
            set {
                this.xServField = value;
            }
        }
        
        /// <remarks/>
        public string CNPJ {
            get {
                return this.cNPJField;
            }
            set {
                this.cNPJField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("chNFe")]
        public string[] chNFe {
            get {
                return this.chNFeField;
            }
            set {
                this.chNFeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TVerDownloadNFe versao {
            get {
                return this.versaoField;
            }
            set {
                this.versaoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
    public enum TAmb {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1")]
        Item1,
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("2")]
        Item2,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
    public enum TDownloadNFeXServ {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("DOWNLOAD NFE")]
        DOWNLOADNFE,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1055.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
    public enum TVerDownloadNFe {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1.00")]
        Item100,
    }
}
