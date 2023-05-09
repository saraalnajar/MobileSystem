﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Sedco.SelfService.Kiosk.DataManager.BuyAndRechargeService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="BuyAndRechargeService.IBuyAndRechargeService")]
    public interface IBuyAndRechargeService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBuyAndRechargeService/HandleCustomer", ReplyAction="http://tempuri.org/IBuyAndRechargeService/HandleCustomerResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, string>))]
        object HandleCustomer(string action, System.Collections.Generic.Dictionary<string, string> customerData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBuyAndRechargeService/HandleCustomer", ReplyAction="http://tempuri.org/IBuyAndRechargeService/HandleCustomerResponse")]
        System.Threading.Tasks.Task<object> HandleCustomerAsync(string action, System.Collections.Generic.Dictionary<string, string> customerData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBuyAndRechargeService/HandlePackages", ReplyAction="http://tempuri.org/IBuyAndRechargeService/HandlePackagesResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, string>))]
        object HandlePackages(string action, System.Collections.Generic.Dictionary<string, string> PackageData);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/IBuyAndRechargeService/HandlePackages", ReplyAction="http://tempuri.org/IBuyAndRechargeService/HandlePackagesResponse")]
        System.Threading.Tasks.Task<object> HandlePackagesAsync(string action, System.Collections.Generic.Dictionary<string, string> PackageData);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface IBuyAndRechargeServiceChannel : Sedco.SelfService.Kiosk.DataManager.BuyAndRechargeService.IBuyAndRechargeService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class BuyAndRechargeServiceClient : System.ServiceModel.ClientBase<Sedco.SelfService.Kiosk.DataManager.BuyAndRechargeService.IBuyAndRechargeService>, Sedco.SelfService.Kiosk.DataManager.BuyAndRechargeService.IBuyAndRechargeService {
        
        public BuyAndRechargeServiceClient() {
        }
        
        public BuyAndRechargeServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public BuyAndRechargeServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BuyAndRechargeServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public BuyAndRechargeServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public object HandleCustomer(string action, System.Collections.Generic.Dictionary<string, string> customerData) {
            return base.Channel.HandleCustomer(action, customerData);
        }
        
        public System.Threading.Tasks.Task<object> HandleCustomerAsync(string action, System.Collections.Generic.Dictionary<string, string> customerData) {
            return base.Channel.HandleCustomerAsync(action, customerData);
        }
        
        public object HandlePackages(string action, System.Collections.Generic.Dictionary<string, string> PackageData) {
            return base.Channel.HandlePackages(action, PackageData);
        }
        
        public System.Threading.Tasks.Task<object> HandlePackagesAsync(string action, System.Collections.Generic.Dictionary<string, string> PackageData) {
            return base.Channel.HandlePackagesAsync(action, PackageData);
        }
    }
}
