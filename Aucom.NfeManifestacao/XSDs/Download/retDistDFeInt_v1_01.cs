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
// This source code was auto-generated by xsd, Version=4.0.30319.1.
// 
namespace Scire.NFeManifestacao.XSDs.Download.Resposta
{
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
    [System.Xml.Serialization.XmlRootAttribute(Namespace="http://www.portalfiscal.inf.br/nfe", IsNullable=false)]
    public partial class retDistDFeInt {
        
        private TAmb tpAmbField;
        
        private string verAplicField;
        
        private string cStatField;
        
        private string xMotivoField;
        
        private string dhRespField;
        
        private string ultNSUField;
        
        private string maxNSUField;
        
        private retDistDFeIntLoteDistDFeInt loteDistDFeIntField;
        
        private TVerDistDFe versaoField;
        
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
        public string verAplic {
            get {
                return this.verAplicField;
            }
            set {
                this.verAplicField = value;
            }
        }
        
        /// <remarks/>
        public string cStat {
            get {
                return this.cStatField;
            }
            set {
                this.cStatField = value;
            }
        }
        
        /// <remarks/>
        public string xMotivo {
            get {
                return this.xMotivoField;
            }
            set {
                this.xMotivoField = value;
            }
        }
        
        /// <remarks/>
        public string dhResp {
            get {
                return this.dhRespField;
            }
            set {
                this.dhRespField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string ultNSU {
            get {
                return this.ultNSUField;
            }
            set {
                this.ultNSUField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(DataType="token")]
        public string maxNSU {
            get {
                return this.maxNSUField;
            }
            set {
                this.maxNSUField = value;
            }
        }
        
        /// <remarks/>
        public retDistDFeIntLoteDistDFeInt loteDistDFeInt {
            get {
                return this.loteDistDFeIntField;
            }
            set {
                this.loteDistDFeIntField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public TVerDistDFe versao {
            get {
                return this.versaoField;
            }
            set {
                this.versaoField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
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
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
    public partial class retDistDFeIntLoteDistDFeInt {
        
        private retDistDFeIntLoteDistDFeIntDocZip[] docZipField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("docZip")]
        public retDistDFeIntLoteDistDFeIntDocZip[] docZip {
            get {
                return this.docZipField;
            }
            set {
                this.docZipField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true, Namespace="http://www.portalfiscal.inf.br/nfe")]
    public partial class retDistDFeIntLoteDistDFeIntDocZip {
        
        private string nSUField;
        
        private string schemaField;
        
        private byte[] valueField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="token")]
        public string NSU {
            get {
                return this.nSUField;
            }
            set {
                this.nSUField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute()]
        public string schema {
            get {
                return this.schemaField;
            }
            set {
                this.schemaField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlTextAttribute(DataType="base64Binary")]
        public byte[] Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://www.portalfiscal.inf.br/nfe")]
    public enum TVerDistDFe {
        
        /// <remarks/>
        [System.Xml.Serialization.XmlEnumAttribute("1.01")]
        Item101,
    }
}
