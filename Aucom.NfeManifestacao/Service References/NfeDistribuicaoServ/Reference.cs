﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Scire.NFeManifestacao.NfeDistribuicaoServ {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe", ConfigurationName="NfeDistribuicaoServ.NFeDistribuicaoDFeSoap")]
    public interface NFeDistribuicaoDFeSoap {
        
        // CODEGEN: Generating message contract since element name nfeDadosMsg from namespace http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe is not marked nillable
        [System.ServiceModel.OperationContractAttribute(Action="http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe/nfeDistDFeInteresse", ReplyAction="*")]
        Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseResponse nfeDistDFeInteresse(Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class nfeDistDFeInteresseRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="nfeDistDFeInteresse", Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe", Order=0)]
        public Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseRequestBody Body;
        
        public nfeDistDFeInteresseRequest() {
        }
        
        public nfeDistDFeInteresseRequest(Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe")]
    public partial class nfeDistDFeInteresseRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.Linq.XElement nfeDadosMsg;
        
        public nfeDistDFeInteresseRequestBody() {
        }
        
        public nfeDistDFeInteresseRequestBody(System.Xml.Linq.XElement nfeDadosMsg) {
            this.nfeDadosMsg = nfeDadosMsg;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class nfeDistDFeInteresseResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="nfeDistDFeInteresseResponse", Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe", Order=0)]
        public Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseResponseBody Body;
        
        public nfeDistDFeInteresseResponse() {
        }
        
        public nfeDistDFeInteresseResponse(Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://www.portalfiscal.inf.br/nfe/wsdl/NFeDistribuicaoDFe")]
    public partial class nfeDistDFeInteresseResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public System.Xml.Linq.XElement nfeDistDFeInteresseResult;
        
        public nfeDistDFeInteresseResponseBody() {
        }
        
        public nfeDistDFeInteresseResponseBody(System.Xml.Linq.XElement nfeDistDFeInteresseResult) {
            this.nfeDistDFeInteresseResult = nfeDistDFeInteresseResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface NFeDistribuicaoDFeSoapChannel : Scire.NFeManifestacao.NfeDistribuicaoServ.NFeDistribuicaoDFeSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class NFeDistribuicaoDFeSoapClient : System.ServiceModel.ClientBase<Scire.NFeManifestacao.NfeDistribuicaoServ.NFeDistribuicaoDFeSoap>, Scire.NFeManifestacao.NfeDistribuicaoServ.NFeDistribuicaoDFeSoap {
        
        public NFeDistribuicaoDFeSoapClient() {
        }
        
        public NFeDistribuicaoDFeSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public NFeDistribuicaoDFeSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NFeDistribuicaoDFeSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public NFeDistribuicaoDFeSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseResponse Scire.NFeManifestacao.NfeDistribuicaoServ.NFeDistribuicaoDFeSoap.nfeDistDFeInteresse(Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseRequest request) {
            return base.Channel.nfeDistDFeInteresse(request);
        }
        
        public System.Xml.Linq.XElement nfeDistDFeInteresse(System.Xml.Linq.XElement nfeDadosMsg) {
            Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseRequest inValue = new Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseRequest();
            inValue.Body = new Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseRequestBody();
            inValue.Body.nfeDadosMsg = nfeDadosMsg;
            Scire.NFeManifestacao.NfeDistribuicaoServ.nfeDistDFeInteresseResponse retVal = ((Scire.NFeManifestacao.NfeDistribuicaoServ.NFeDistribuicaoDFeSoap)(this)).nfeDistDFeInteresse(inValue);
            return retVal.Body.nfeDistDFeInteresseResult;
        }
    }
}
