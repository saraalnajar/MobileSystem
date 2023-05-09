using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using Sedco.SelfService.Kiosk.SharedProject;

namespace BuyAndRechargeWcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IBuyAndRechargeService" in both code and config file together.
    [ServiceContract]
    public interface IBuyAndRechargeService
    {
        [OperationContract]
         object HandleCustomer(string action, Dictionary<string, string> customerData);
        [OperationContract]
        object HandlePackages(string action, Dictionary<string, string> packageData);
    }
}
