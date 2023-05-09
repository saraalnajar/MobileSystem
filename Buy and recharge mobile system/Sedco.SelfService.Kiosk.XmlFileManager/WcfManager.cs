using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Sedco.SelfService.Kiosk.DataManager.BuyAndRechargeService;
using Sedco.SelfService.Kiosk.SharedProject;
namespace Sedco.SelfService.Kiosk.DataManager
{
    public class WcfManager:IDataManager
    {
        private readonly BuyAndRechargeServiceClient _serviceClient = new BuyAndRechargeServiceClient("BasicHttpBinding_IBuyAndRechargeService");
        static List<Customer> ParseCustomers(string input)
        {
            List<Customer> customers = new List<Customer>();

            string pattern = "<Customer>(.*?)</Customer>";
            foreach (Match match in Regex.Matches(input, pattern, RegexOptions.Singleline))
            {
                Customer customer = new Customer();
                customer.CustomerName = Regex.Match(match.Value, "<CustomerName>(.*?)</CustomerName>").Groups[1].Value;
                customer.CustomerPhoneNumber = Regex.Match(match.Value, "<CustomerPhoneNumber>(.*?)</CustomerPhoneNumber>").Groups[1].Value;
                customer.CustomerBirthdate = Regex.Match(match.Value, "<CustomerBirthdate>(.*?)</CustomerBirthdate>").Groups[1].Value;
                customer.CustomerPackageName = Regex.Match(match.Value, "<CustomerPackageName>(.*?)</CustomerPackageName>").Groups[1].Value;
                customer.CustomerBalance = decimal.Parse(Regex.Match(match.Value, "<CustomerBalance>(.*?)</CustomerBalance>").Groups[1].Value);
                customers.Add(customer);
            }

            return customers;
        }
        public bool CheckExistingPhoneNumber(string phoneNumberChecking)
        {
            return (bool)_serviceClient.HandleCustomer("checkExistingPhoneNumber", new Dictionary<string, string> { { "PhoneNumber", phoneNumberChecking } });
        }

        public List<Customer> GetCustomers()
        {
            string customersXML =(_serviceClient.HandleCustomer("getCustomers", new Dictionary<string, string>())).ToString();

            List<Customer> customers = ParseCustomers(customersXML);
            return customers;
        }

        public string RechargeBalance(string phoneNumber, string amount)
        {
            return (string)_serviceClient.HandleCustomer("rechargeBalance", new Dictionary<string, string> { { "PhoneNumber", phoneNumber }, { "Amount", amount } });
        }

        public List<string> GetPackagesList(string packageType)
        {
            string packages=(_serviceClient.HandlePackages("getPackagesList", new Dictionary<string, string> { { "packageType", packageType } })).ToString();
            List<string>packagesList= packages.Split(',').ToList();
            return packagesList;
        }

        public List<Customer> GetFilteredCustomers(string search)
        {
            string filteredCustomers = (_serviceClient
                .HandleCustomer("getFilteredCustomers", new Dictionary<string, string> { { "Search", search } }))
                .ToString();
            List <Customer> customers = ParseCustomers(filteredCustomers);
            return customers;
        }

        public bool DeleteCustomers(string[] customerPhoneNumber)
        {
            Dictionary<string, string> customerInfo = new Dictionary<string, string>();
            customerInfo.Add("PhoneNumber", string.Join(",", customerPhoneNumber));
           
            return (bool)_serviceClient.HandleCustomer("deleteCustomers", customerInfo);
        }

        public bool AddPackage(string packageType, string packageName, string packagePrice)
        {
            return (bool)_serviceClient.HandlePackages("addPackage",new Dictionary<string, string>{{"packageType", packageType},{"packageName", packageName},{"packagePrice", packagePrice}});
        }

        public bool CheckIfNoCustomers()
        {
            return (bool)_serviceClient.HandleCustomer("checkIfNoCustomers", new Dictionary<string, string>());
        }

        public bool AddCustomer(string packageName, string phoneNumber, string customerName, string customerBirthDate)
        {
            return (bool)_serviceClient.HandleCustomer("addCustomer", new Dictionary<string, string> { { "packageName", packageName }, { "phoneNumber", phoneNumber }, { "customerName", customerName }, { "customerBirthDate", customerBirthDate } });
        }

        public string GetPackageTypeFromName(string packageName)
        {
            return (string)_serviceClient.HandlePackages("getPackageTypeFromName", new Dictionary<string, string> {{ "packageName", packageName } });
    }

        public DateTime GetExpireDate(string phoneNumber)
        {
            return (DateTime)_serviceClient.HandleCustomer("getExpireDate", new Dictionary<string, string> { { "phoneNumber", phoneNumber } });
        }

        public bool EditCustomer(string packageName, string editCustomerName, string editCustomerBirthDate, string phoneNumber)
        {
            return (bool)_serviceClient.HandleCustomer("editCustomer", new Dictionary<string, string> { { "packageName", packageName},{"editCustomerName", editCustomerName},{"editCustomerBirthDate", editCustomerBirthDate},{"phoneNumber", phoneNumber}});
        }

        public bool CheckExistingPackages(string packageName)
        {
            return (bool)_serviceClient.HandlePackages("checkExistingPackages",new Dictionary<string, string>{{"packageName", packageName}});
        }

        public Customer GetCustomerInformation(string phoneNumber)
        {
            string customerValues = (_serviceClient.HandleCustomer("getCustomerInformation",
                new Dictionary<string, string> { { "PhoneNumber", phoneNumber } })).ToString();
            string[] customerInformation = customerValues.Split(',');

            Customer customer = new Customer
            {
                CustomerName = customerInformation[0],
                CustomerPhoneNumber = customerInformation[1],
                CustomerBirthdate = customerInformation[2],
                CustomerPackageName = customerInformation[3],
                CustomerBalance = decimal.Parse(customerInformation[4])
            };
            return customer;
        }
    }
}
