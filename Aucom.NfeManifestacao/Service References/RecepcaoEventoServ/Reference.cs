﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Scire.NFeManifestacao.RecepcaoEventoServ {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4", ConfigurationName="RecepcaoEventoServ.NFeRecepcaoEvento4Soap")]
    public interface NFeRecepcaoEvento4Soap {
        
        // CODEGEN: Generating message contract since the operation nfeRecepcaoEventoNF is neither RPC nor document wrapped.
        [System.ServiceModel.OperationContractAttribute(Action="http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4/nfeRecepcaoEventoNF", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        Scire.NFeManifestacao.RecepcaoEventoServ.nfeRecepcaoEventoNFResponse nfeRecepcaoEventoNF(Scire.NFeManifestacao.RecepcaoEventoServ.nfeRecepcaoEventoNFRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class nfeRecepcaoEventoNFRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4", Order=0)]
        public System.Xml.XmlNode nfeDadosMsg;
        
        public nfeRecepcaoEventoNFRequest() {
        }
        
        public nfeRecepcaoEventoNFRequest(System.Xml.XmlNode nfeDadosMsg) {
            this.nfeDadosMsg = nfeDadosMsg;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class nfeRecepcaoEventoNFResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/NFeRecepcaoEvento4", Order=0)]
        public System.Xml.XmlNode nfeRecepcaoEventoNFResult;
        
        public nfeRecepcaoEventoNFResponse() {
        }
        
        public nfeRecepcaoEventoNFResponse(System.Xml.XmlNode nfeRecepcaoEventoNFResult) {
            this.nfeRecepcaoEventoNFResult = nfeRecepcaoEventoNFResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface NFeRecepcaoEvento4SoapChannel : Scire.NFeManifestacao.RecepcaoEventoServ.NFeRecepcaoEvento4Soap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class NFeRecepcaoEvento4SoapClient : System.ServiceModel.ClientBase<Scire.NFeManifestacao.RecepcaoEventoServ.NFeRecepcaoEvento4Soap>, Scire.NFeManifestacao.RecepcaoEventoServ.NFeRecepcaoEvento4Soap {
        
        public NFeRecepcaoEvento4SoapClient() {
        }
        
        public NFeRecepcaoEvento4SoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public NFeRecepcaoEvento4SoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NFeRecepcaoEvento4SoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NFeRecepcaoEvento4SoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Scire.NFeManifestacao.RecepcaoEventoServ.nfeRecepcaoEventoNFResponse Scire.NFeManifestacao.RecepcaoEventoServ.NFeRecepcaoEvento4Soap.nfeRecepcaoEventoNF(Scire.NFeManifestacao.RecepcaoEventoServ.nfeRecepcaoEventoNFRequest request) {
            return base.Channel.nfeRecepcaoEventoNF(request);
        }
        
        public System.Xml.XmlNode nfeRecepcaoEventoNF(System.Xml.XmlNode nfeDadosMsg) {
            Scire.NFeManifestacao.RecepcaoEventoServ.nfeRecepcaoEventoNFRequest inValue = new Scire.NFeManifestacao.RecepcaoEventoServ.nfeRecepcaoEventoNFRequest();
            inValue.nfeDadosMsg = nfeDadosMsg;
            Scire.NFeManifestacao.RecepcaoEventoServ.nfeRecepcaoEventoNFResponse retVal = ((Scire.NFeManifestacao.RecepcaoEventoServ.NFeRecepcaoEvento4Soap)(this)).nfeRecepcaoEventoNF(inValue);
            return retVal.nfeRecepcaoEventoNFResult;
        }
    }
}
