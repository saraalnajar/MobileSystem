using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using Sedco.SelfService.Kiosk.SharedProject;

namespace Sedco.SelfService.Kiosk.DataManager
{
    public class XmlFileManager : IDataManager
    {
        private readonly XDocument _customerXdoc;
        private readonly XDocument _packagesXdoc;

        public XmlFileManager()
        {
            string customersPath = AppDomain.CurrentDomain.BaseDirectory + "\\customers.xml";
            string packagesPath = AppDomain.CurrentDomain.BaseDirectory + "\\packages.xml";
            if (!File.Exists(customersPath))
            {
                new XDocument(new XElement("Customers", " ")).Save("customers.xml");
                WriteToLogFile.WriteToLogStoryFile("The Customer file is created");
            }

            if (!File.Exists(packagesPath))
            {
                new XDocument(new XElement("Packages", " ")).Save("packages.xml");
                WriteToLogFile.WriteToLogStoryFile("The Packages file is created");

            }

            _customerXdoc = XDocument.Load(@"customers.xml");
            WriteToLogFile.WriteToLogStoryFile("The Customer file loaded");

            _packagesXdoc = XDocument.Load(@"packages.xml");
            WriteToLogFile.WriteToLogStoryFile("The Packages file loaded");

        }

        public List<Customer> GetCustomers()
        {
            List<Customer> customersList = new List<Customer>();
            XDocument customerXdoc;
            IEnumerable<XElement> cusomersInfoQuery;
            IEnumerable<string> packagesNameQuery;
            string packageName;
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\customers.xml"))
            {
                new XDocument(new XElement("Customers", " ")).Save("customers.xml");
                WriteToLogFile.WriteToLogStoryFile("The Customer file is created");
            }
            customerXdoc = XDocument.Load(@"customers.xml");
            cusomersInfoQuery = customerXdoc.Descendants("Customer");
            if (cusomersInfoQuery.Count() > 0)
            {
                foreach (XElement find in cusomersInfoQuery)
                {
                    packagesNameQuery = from Package in _packagesXdoc.Descendants("Package")
                                        where Package.Element("Id").Value == find.Element("PackageId").Value
                                        select Package.Element("Name").Value;
                    packageName = packagesNameQuery.First();
                    customersList.Add(new SharedProject.Customer(find.Element("Name").Value,
                        find.Element("PhoneNumber").Value
                        , find.Element("BirthDate").Value, packageName, Convert.ToDecimal(find.Element("Balance").Value)));

                }
            }

            return customersList;
        }

        public bool CheckExistingPhoneNumber(string phoneNumberChecking)
        {
            bool isExist = false;
            int phoneNumberQuery = 0;
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\customers.xml"))
            {
                new XDocument(new XElement("Customers", " ")).Save("customers.xml");
                WriteToLogFile.WriteToLogStoryFile("The Customer file is created");
            }

            XDocument customerXdoc = XDocument.Load(@"customers.xml");


            phoneNumberQuery = (from item in customerXdoc.Descendants("Customer")
                                where item.Element("PhoneNumber").Value == phoneNumberChecking
                                select item.Element("PhoneNumber").Value).Count();
            if (phoneNumberQuery != 0)
            {
                isExist = true;
            }

            return isExist;
        }

        public string RechargeBalance(string phoneNumber, string amount)
        {
            XDocument customerXdoc;
            List<XElement> rechargeQuery = null;

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\customers.xml"))
            {
                new XDocument(new XElement("Customers", " ")).Save("customers.xml");
                WriteToLogFile.WriteToLogStoryFile("The Customer file is created");
            }
            
            customerXdoc = XDocument.Load(@"customers.xml");

            rechargeQuery = (from item in customerXdoc.Descendants("Customer")
                             where item.Element("PhoneNumber").Value == phoneNumber
                             select item).ToList();

            rechargeQuery[0].Element("Balance").Value =
                (Int32.Parse(rechargeQuery[0].Element("Balance").Value) + Int32.Parse(amount)).ToString();
            rechargeQuery[0].Element("ExpireDate").Value = (DateTime.Now.AddMonths(1)).ToString("dd/MM/yyyy");

            customerXdoc.Save(@"customers.xml");

            return rechargeQuery[0].Element("Balance").Value;
        }

        public List<string> GetPackagesList(string packageType)
        {
            List<string> nameOfPackages = new List<string>();

            nameOfPackages = (from Package in _packagesXdoc.Descendants("Package")
                              where Package.Element("Type").Value == packageType
                              select Package.Element("Name").Value).ToList();
            return nameOfPackages;
        }

        public List<Customer> GetFilteredCustomers(string search)
        {
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\customers.xml"))
            {
                new XDocument(new XElement("Customers", " ")).Save("customers.xml");
                WriteToLogFile.WriteToLogStoryFile("The Customer file is created");

            }

            List<Customer> customersList = new List<Customer>();
            XDocument customerXdoc = XDocument.Load(@"customers.xml");
            List<XElement> cusomersInfoQuery = null;
            string packageName;

            cusomersInfoQuery = (from customer in customerXdoc.Descendants("Customer")
                                 where customer.Element("PhoneNumber").Value.Contains(search) ||
                                       (customer.Element("Name").Value).ToLower().Contains(search.ToLower())
                                 select customer).ToList();
            foreach (XElement find in cusomersInfoQuery)
            {
                packageName = (from package in _packagesXdoc.Descendants("Package")
                               where package.Element("Id").Value == find.Element("PackageId").Value
                               select package.Element("Name").Value).First();
                customersList.Add(new SharedProject.Customer(find.Element("Name").Value,
                    find.Element("PhoneNumber").Value,
                    find.Element("BirthDate").Value, packageName, Convert.ToDecimal(find.Element("Balance").Value)));
            }

            return customersList;
        }

        public bool DeleteCustomers(string[] customerPhoneNumber)
        {
            bool result = false;
            List<bool> isDeleted = new List<bool>();
            XDocument customersXdoc = null;
            List<XElement> deleteQuery = new List<XElement>();
            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\customers.xml"))
            {
                new XDocument(new XElement("Customers", " ")).Save("customers.xml");
                WriteToLogFile.WriteToLogStoryFile("The Customer file is created");

            }

            customersXdoc = XDocument.Load(@"customers.xml");
            foreach (var phoneNumber in customerPhoneNumber)
            {
                deleteQuery.Add(customersXdoc.Descendants
                    ("Customer").First(del => del.Element("PhoneNumber").Value == phoneNumber));
                if (deleteQuery.Count() != 0)
                {
                    isDeleted.Add(true);
                }
                else
                {
                    isDeleted.Add(false);
                }
            }

            if (!isDeleted.Contains(false))
            {
                deleteQuery.Remove();
                result = true;
            }

            customersXdoc.Save(@"customers.xml");
            return result;
        }

        public bool AddPackage(string packageType, string packageName, string packagePrice)
        {
            bool isAdded = false;
            try
            {
                int packageID;
                IEnumerable<string> packageIDQuery = from package in _packagesXdoc.Descendants("Package")
                                                     select package.Element("Id").Value;
                if (packageIDQuery.Count() == 0)
                {
                    packageID = 1;
                }
                else
                {
                    packageID = Int32.Parse(packageIDQuery.Last()) + 1;
                }

                XElement NewPackage = new XElement("Package");
                NewPackage.Add(new XElement("Id", packageID));
                NewPackage.Add(new XElement("Name", packageName));
                NewPackage.Add(new XElement("Type", packageType));
                NewPackage.Add(new XElement("Price", packagePrice));
                _packagesXdoc.Element("Packages").Add(NewPackage);
                _packagesXdoc.Save(@"packages.xml");
                isAdded = true;
            }
            catch
            {
                isAdded = false;
            }

            return isAdded;
        }

        public bool CheckIfNoCustomers()
        {
            int count = _customerXdoc.Element("Customers").Elements().Count();
            if (count == 0)
                return true;
            else
                return false;
        }

        public bool AddCustomer(string packageName, string phoneNumber, string customerName, string customerBirthDate)
        {
            bool addResult = false;
            try
            {
                int packageId;
                int customerBalance;
                string expireDate;
                XDocument customers;
                XElement newCustomer;
                IEnumerable<string> packageIdQuery = from package in _packagesXdoc.Descendants("Package")
                                                     where package.Element("Name").Value == packageName
                                                     select package.Element("Id").Value;
                packageId = Int32.Parse(packageIdQuery.First());
                IEnumerable<string> packagePriceQuery = from package in _packagesXdoc.Descendants("Package")
                                                        where package.Element("Name").Value == packageName
                                                        select package.Element("Price").Value;
                customerBalance = Int32.Parse(packagePriceQuery.First());
                expireDate = DateTime.Now.AddMonths(3).ToString("dd/MM/yyyy");
                if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\customers.xml"))
                {
                    new XDocument(new XElement("Customers", " ")).Save("customers.xml");
                }

                customers = XDocument.Load(@"customers.xml");
                newCustomer = new XElement("Customer");
                newCustomer.Add(new XElement("Name", customerName));
                newCustomer.Add(new XElement("PhoneNumber", phoneNumber));
                newCustomer.Add(new XElement("BirthDate", customerBirthDate));
                newCustomer.Add(new XElement("ExpireDate", expireDate));
                newCustomer.Add(new XElement("PackageId", packageId));
                newCustomer.Add(new XElement("Balance", customerBalance));
                customers.Element("Customers").Add(newCustomer);
                customers.Save(@"customers.xml");
                addResult = true;
            }
            catch
            {
                addResult = false;
            }

            return addResult;
        }

        public string GetPackageTypeFromName(string packageName)
        {
            string packageType = "";
            packageType = (from package in _packagesXdoc.Descendants("Package")
                           where package.Element("Name").Value == packageName
                           select package.Element("Type").Value).First();
            return packageType;
        }

        public DateTime GetExpireDate(string phoneNumber)
        {
            DateTime expireDate = DateTime.Now;
            expireDate = DateTime.ParseExact((from package in _customerXdoc.Descendants("Customer")
                                              where package.Element("PhoneNumber").Value == phoneNumber
                                              select package.Element("ExpireDate").Value).First(), "dd/MM/yyyy", null);
            return expireDate;
        }

        public bool EditCustomer(string packageName, string editCustomerName, string editCustomerBirthDate,
            string phoneNumber)
        {
            var customerXdoc = XDocument.Load(@"customers.xml");
            bool isEdited = true;
            List<XElement> editCustomerInforQery;
            string packageId;
            editCustomerInforQery = (from item in customerXdoc.Descendants("Customer")
                                     where item.Element("PhoneNumber").Value == phoneNumber
                                     select item).ToList();
            if (editCustomerInforQery.Count > 0)
            {
                isEdited = true;
                IEnumerable<string> queryPackageId = from package in _packagesXdoc.Descendants("Package")
                                                     where package.Element("Name").Value == packageName
                                                     select package.Element("Id").Value;
                packageId = queryPackageId.First();
                foreach (var item in editCustomerInforQery)
                {
                    item.Element("Name").Value = editCustomerName;
                    item.Element("BirthDate").Value = editCustomerBirthDate;
                    item.Element("PackageId").Value = packageId;
                }

                customerXdoc.Save(@"customers.xml");
            }
            else
            {
                isEdited = false;
            }

            return isEdited;
        }

        public bool CheckExistingPackages(string packageName)
        {
            bool existPackage = false;

            int packages = (from package in _packagesXdoc.Descendants("Package")
                            where package.Element("Name").Value == packageName
                            select package).Count();

            if (packages != 0)
            {
                existPackage = true;
            }

            return existPackage;

        }

        public Customer GetCustomerInformation(string phoneNumber)
        {Customer customer = null;
            XDocument customerXdoc;
            List<XElement> cusomersInfoQuery = null;
            string packageName;

            if (!File.Exists(AppDomain.CurrentDomain.BaseDirectory + "\\customers.xml"))
            {
                new XDocument(new XElement("Customers", " ")).Save("customers.xml");
                WriteToLogFile.WriteToLogStoryFile("The Customer file is created");

            }

            customerXdoc = XDocument.Load(@"customers.xml");
            cusomersInfoQuery = (from customerElement in customerXdoc.Descendants("Customer")
                                 where customerElement.Element("PhoneNumber").Value.Equals(phoneNumber)
                                 select customerElement).ToList();

            foreach (XElement find in cusomersInfoQuery)
            {
                packageName = (from package in _packagesXdoc.Descendants("Package")
                               where package.Element("Id").Value == find.Element("PackageId").Value
                               select package.Element("Name").Value).First();
                customer = new Customer(find.Element("Name").Value,
                    find.Element("PhoneNumber").Value,
                    find.Element("BirthDate").Value, packageName, Convert.ToDecimal(find.Element("Balance").Value));
            }

            return customer;
        }
    }
}