using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sedco.SelfService.Kiosk.DataManager;
using Sedco.SelfService.Kiosk.SharedProject;
using System.Configuration;

namespace Sedco.SelfService.Kiosk.LogicManagerSystem
{
    public class SystemLogicManager
    {
        private readonly IDataManager _dataManager;
        private const string DataSourceType = "dataSourceType";
        private const string Xml = "XML";
        private const string Database = "Database";
        private const string WCF = "WCF";
        private const string Restful= "Restful";


        public SystemLogicManager()
        {

            string databaseType = ConfigurationManager.AppSettings[DataSourceType];
            if (databaseType.Equals(Xml, StringComparison.OrdinalIgnoreCase))
            {
                _dataManager = new XmlFileManager();
            }
            else if (databaseType.Equals(Database, StringComparison.OrdinalIgnoreCase))
            {
                _dataManager = new DataBaseManager();
            }
            else if (databaseType.Equals( WCF , StringComparison.OrdinalIgnoreCase))
            {
                _dataManager = new WcfManager();
            }
            else if (databaseType.Equals(Restful, StringComparison.OrdinalIgnoreCase))
            {
                _dataManager = new RestfulApiManager();
            }
        }

        public bool CheckExistingPhoneNumber(string phoneNumber)
        {
            return _dataManager.CheckExistingPhoneNumber(phoneNumber);
        }

        public bool CheckValidity(string regex, string fieldContent)
        {
            bool matchRegex = false;
            matchRegex = Regex.IsMatch(fieldContent, regex);

            return matchRegex;
        }
        public string RechargeBalance(string phoneNumber, string amount)
        {
            return _dataManager.RechargeBalance(phoneNumber, amount);
        }

        public List<string> AddItems(string packageType)
        {
            return _dataManager.GetPackagesList(packageType);
        }

        public int CalculateAge(DateTime birthDate)
        {
            int years = DateTime.Now.Year - birthDate.Year;

            if ((birthDate.Month > DateTime.Now.Month) ||
                (birthDate.Month == DateTime.Now.Month && birthDate.Day >= DateTime.Now.Day))
            {
                years--;

            }

            return years;
        }

        public List<Customer> FilterTheSearch(string search)
        {
            return _dataManager.GetFilteredCustomers(search);
        }

        public List<Customer> GetAllCustomer()
        {
            return _dataManager.GetCustomers();
        }

        public bool DeleteCustomers(string[] customerPhoneNumber)
        {
            return _dataManager.DeleteCustomers(customerPhoneNumber);
        }

        public bool AddPackage(string packageType, string packageName, string packagePrice)
        {
            return _dataManager.AddPackage(packageType, packageName, packagePrice);
        }


        public string GenerateNumber()
        {
            string randomNumber;
            bool phoneNumber;
            Random random;

            while (true)
            {
                randomNumber = "";
                random = new Random();
                int i;
                for (i = 0; i < 7; i++)
                {
                    randomNumber += random.Next(0, 9).ToString();
                }
                if (_dataManager.CheckIfNoCustomers())
                {
                    break;
                }
                else
                {
                    phoneNumber = CheckExistingPhoneNumber(randomNumber);
                    if (!phoneNumber)
                        break;
                }
            }

            return randomNumber;
        }

        public bool AddCustomer(string packageName, string phoneNumber, string customerName, string customerBirthDate)
        {
            return _dataManager.AddCustomer(packageName, phoneNumber, customerName, customerBirthDate);
        }

        public string GetPackageType(string edit)
        {
            return _dataManager.GetPackageTypeFromName(edit);
        }

        public DateTime GetExpireDate(string expireDate)
        {
            return _dataManager.GetExpireDate(expireDate);
        }

        public Customer GetCustomerInformation(string phoneNumber)
        {
            return _dataManager.GetCustomerInformation(phoneNumber);
        }
        public bool EditCustomer(string packageName, string editCustomerName, string editCustomerBirthDate, string phoneNumber)
        {
            return _dataManager.EditCustomer(packageName, editCustomerName, editCustomerBirthDate, phoneNumber);
        }

        public bool CheckExistingPackages(string packageName)
        {
            return _dataManager.CheckExistingPackages(packageName);

        }
    }
}
