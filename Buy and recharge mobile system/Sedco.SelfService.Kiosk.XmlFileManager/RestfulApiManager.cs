using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Sedco.SelfService.Kiosk.SharedProject;

namespace Sedco.SelfService.Kiosk.DataManager
{
    public class RestfulApiManager : IDataManager
    {
        public HttpClient Client;
        public HttpResponseMessage Response;

        public RestfulApiManager()
        {
            Client = new HttpClient();
            Client.BaseAddress = new Uri("https://localhost:44302/");
        }

        public bool CheckExistingPhoneNumber(string phoneNumberChecking)
        {
            bool result = true;
            Response = Client
                .GetAsync("api/BuyAndRecharge/phoneNumberChecking" + phoneNumberChecking).Result;
            if (Response.IsSuccessStatusCode)
            {
                result = Response.Content.ReadAsAsync<bool>().Result;
            }
            else
            {
                result = true;
            }

            return result;
        }

        public List<Customer> GetCustomers()
        {
            Response = Client.GetAsync($"api/BuyAndRecharge/GetCustomers").Result;
            List<Customer> customers = Response.Content.ReadAsAsync<List<Customer>>().Result;
            return customers;
        }

        public string RechargeBalance(string phoneNumber, string amount)
        {
            Response = Client.PutAsync($"api/BuyAndRecharge/RechargeBalance?phoneNumber={phoneNumber}&amount={amount}",null).Result;
            string result = Response.Content.ReadAsStringAsync().Result;
            return result;
        }

        public List<string> GetPackagesList(string packageType)
        {
            Response = Client.GetAsync($"api/BuyAndRecharge/GetFilteredCustomers?packageType=" + packageType).Result;
            List<string> packages = Response.Content.ReadAsAsync<List<string>>().Result;
            return packages;
        }

        public List<Customer> GetFilteredCustomers(string search)
        {
            Response = Client.GetAsync($"api/BuyAndRecharge/GetFilteredCustomers?search={search}").Result;
            List<Customer> customers = Response.Content.ReadAsAsync<List<Customer>>().Result;
            return customers;
        }

        public bool DeleteCustomers(string[] customerPhoneNumber)
        {
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            Response = Client.PostAsJsonAsync("api/BuyAndRecharge/DeleteCustomers", customerPhoneNumber).Result;
            bool result = Response.Content.ReadAsAsync<bool>().Result;
            return result;

        }

        public bool AddPackage(string packageType, string packageName, string packagePrice)
        {
            Response = Client.GetAsync($"api/BuyAndRecharge/AddPackage?packageType={packageType}&packageName={packageName}&packagePrice={packagePrice}").Result;
            bool result = Response.Content.ReadAsAsync<bool>().Result;
            return result;
        }

        public bool CheckIfNoCustomers()
        {
            Response = Client.GetAsync("api/BuyAndRecharge/CheckIfNoCustomers").Result;
            bool result = Response.Content.ReadAsAsync<bool>().Result;
            return result;
        }

        public bool AddCustomer(string packageName, string phoneNumber, string customerName, string customerBirthDate)
        {
            bool result = false;
            Customer customer = new Customer()
            {
                CustomerName = customerName,
                CustomerBirthdate = customerBirthDate,
                CustomerPhoneNumber = phoneNumber,
                CustomerPackageName = packageName
            };
            Response = Client.PostAsJsonAsync("api/BuyAndRecharge/AddCustomer", customer).Result;
            if (Response.IsSuccessStatusCode)
            {
                result = Response.Content.ReadAsAsync<bool>().Result;
            }
            else
            {
                result = false;
            }

            return result;
        }

        public string GetPackageTypeFromName(string packageName)
        {
            Response = Client.GetAsync($"api/BuyAndRecharge/GetPackageTypeFromName?packageName={packageName}").Result;
            string packageType = Response.Content.ReadAsAsync<string>().Result;
            return packageType;
        }

        public DateTime GetExpireDate(string phoneNumber)
        {
            Response = Client.GetAsync($"api/BuyAndRecharge/GetExpireDate"+phoneNumber).Result;
            DateTime result = Response.Content.ReadAsAsync<DateTime>().Result;
            return result;
        }

        public bool EditCustomer(string packageName, string editCustomerName, string editCustomerBirthDate,
            string phoneNumber)
        {
            Response = Client.PutAsync($"api/BuyAndRecharge/EditCustomer?packageName={packageName}&editCustomerName={editCustomerName}&editCustomerBirthDate={editCustomerBirthDate}&phoneNumber={phoneNumber}", null).Result;
            bool result = Response.Content.ReadAsAsync<bool>().Result;
            return result;
        }

        public bool CheckExistingPackages(string packageName)
        {
            Response = Client.GetAsync($"api/BuyAndRecharge/CheckExistingPackages?packageName={packageName}").Result;
            bool result = Response.Content.ReadAsAsync<bool>().Result;
            return result;
        }

        public Customer GetCustomerInformation(string phoneNumber)
        {
            Customer customer = null;
            Response = Client.GetAsync("api/BuyAndRecharge/GetCustomerInformation" + phoneNumber).Result;

            if (Response.IsSuccessStatusCode)
            {
                customer = Response.Content.ReadAsAsync<Customer>().Result;
            }
            else
            {
                throw new Exception();
            }

            return customer;
        }
    }
}
