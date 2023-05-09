using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using EntityFarmework;
using log4net;
using Sedco.SelfService.Kiosk.SharedProject;

namespace BuyAndRechargeWcfService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "BuyAndRechargeService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select BuyAndRechargeService.svc or BuyAndRechargeService.svc.cs at the Solution Explorer and start debugging.
    public class BuyAndRechargeService : IBuyAndRechargeService
    {
        private readonly BuyAndRechargeSystemModel _dataBaseEntities = new BuyAndRechargeSystemModel(new DatabaseConfiguration().GetWebConfiguration());
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public object HandleCustomer(string action, Dictionary<string, string> customerData)
        {
            object result = null;

            try
            {
                switch (action)
                {
                    case "checkExistingPhoneNumber":
                        {
                            result = CheckExistingPhoneNumber(customerData);
                            break;
                        }

                    case "getCustomers":
                        {

                            result = GetCustomers();

                            break;
                        }

                    case "getCustomerInformation":
                        {
                            result = GetCustomerInformation(customerData);
                            break;
                        }
                    case "rechargeBalance":
                        {
                            result = RechargeBalance(customerData);
                            break;
                        }

                    case "getFilteredCustomers":
                        {
                            result = GetFilteredCustomers(customerData);
                            break;
                        }
                    case "deleteCustomers":
                        {
                            result = DeleteCustomers(customerData);
                            break;

                        }
                    case "checkIfNoCustomers":
                        {
                            result = CheckIfNoCustomers();
                            break;
                        }
                    case "addCustomer":
                        {
                            result = AddCustomer(customerData);
                            break;
                        }
                    case "getExpireDate":
                        {
                            result = GetExpireDate(customerData);
                            break;
                        }
                    case "editCustomer":
                        {
                            result = EditCustomer(customerData);
                            break;
                        }
                    default:
                        return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                result = null;
            }

            return result;
        }

        public object HandlePackages(string action, Dictionary<string, string> packageInfo)
        {
            object result = null;
            switch (action)
            {
                case "getPackagesList":
                    {
                        try
                        {
                            string packageType = packageInfo["packageType"];
                            List<string> packages = _dataBaseEntities.Packages
                                .Where(package => package.Type.Equals(packageType))
                                .Select(package => package.Name).ToList();
                            result = String.Join(",", packages);

                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);

                        }
                        break;
                    }
                case "addPackage":
                    {
                        try
                        {
                            string packageType = packageInfo["packageType"];
                            string packageName = packageInfo["packageName"];
                            string packagePrice = packageInfo["packagePrice"];
                            Packages package = new Packages();
                            package.Name = packageName;
                            package.Type = packageType;
                            package.price = Int32.Parse(packagePrice);
                            _dataBaseEntities.Packages.Add(package);
                            _dataBaseEntities.SaveChanges();
                            result = true;
                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                            result = false;
                        }

                        break;
                    }
                case "getPackageTypeFromName":
                    {
                        try
                        {
                            string packageName = packageInfo["packageName"];
                            result = _dataBaseEntities.Packages.Where(package => package.Name.Equals(packageName))
                                .Select(package => package.Type).Single();

                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                            result = "";
                        }
                        break;
                    }
                case "checkExistingPackages":
                    {
                        try
                        {
                            string packageName = packageInfo["packageName"];
                            int packages = _dataBaseEntities.Packages.Where(package => package.Name == packageName)
                                .Select(package => package.Name).Count();
                            result = packages != 0;

                        }
                        catch (Exception ex)
                        {
                            Log.Error(ex);
                            result = true;
                        }
                        break;
                    }
            }

            return result;
        }

        private bool EditCustomer(Dictionary<string, string> customerData)
        {
            bool isEdited = false;
            try
            {
                string packageName = customerData["packageName"];
                string phoneNumber = customerData["phoneNumber"];
                string customerName = customerData["editCustomerName"];
                string customerBirthDate = customerData["editCustomerBirthDate"];
                Customers customer =
                    _dataBaseEntities.Customers.Single(customerPhone =>
                        customerPhone.PhoneNumber.Equals(phoneNumber));
                if (customer != null)
                {
                    int packageId = _dataBaseEntities.Packages.Where(package => package.Name.Equals(packageName))
                        .Select(package => package.ID).Single();
                    customer.Name = customerName;
                    customer.BirthDate = customerBirthDate;
                    customer.PackageID = packageId;
                    _dataBaseEntities.SaveChanges();
                    isEdited = true;
                }
                else
                {
                    isEdited = false;
                }
            }
            catch (Exception ex)
            {
                isEdited = false;
                Log.Error(ex);
            }

            return isEdited;
        }

        private DateTime GetExpireDate(Dictionary<string, string> customerData)
        {
            DateTime? expireDate;
            try
            {
                string phoneNumber = customerData["phoneNumber"];
                expireDate = _dataBaseEntities.Customers
                   .Where(customer => customer.PhoneNumber.Equals(phoneNumber))
                   .Select(customer => customer.ExpireDate)
                   .Single();
            }
            catch (Exception ex)
            {
                expireDate = null;
                Log.Error(ex);
            }
            return (DateTime)expireDate;
        }

        private bool AddCustomer(Dictionary<string, string> customerData)
        {
            bool result = false;
            try
            {
                string packageName = customerData["packageName"];
                string customerName = customerData["customerName"];
                string phoneNumber = customerData["phoneNumber"];
                string birthDate = customerData["customerBirthDate"];

                Customers customer = new Customers();
                int packageId = _dataBaseEntities.Packages.Where(package => package.Name.Equals(packageName))
                    .Select(package => package.ID).Single();
                int packagePrice = _dataBaseEntities.Packages.Where(package => package.Name.Equals(packageName))
                    .Select(package => package.price).Single();
                string expireDateADateTime = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime expireDate = DateTime.ParseExact(expireDateADateTime, "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture);
                customer.Name = customerName;
                customer.PhoneNumber = phoneNumber;
                customer.BirthDate = birthDate;
                customer.PackageID = packageId;
                customer.ExpireDate = expireDate;
                customer.Balance = packagePrice;
                _dataBaseEntities.Customers.Add(customer);
                _dataBaseEntities.SaveChanges();
                result = true;
            }
            catch (DbEntityValidationException e)
            {
                e.EntityValidationErrors.SelectMany(error => error.ValidationErrors).ToList().ForEach(
                    item => Console.WriteLine("{0} - {1}", item.PropertyName, item.ErrorMessage));
                result = false;
                Log.Error(e);

            }
            catch (Exception ex)
            {
                result = false;
                Log.Error(ex);

            }

            return result;
        }

        private bool CheckIfNoCustomers()
        {
            bool isNoCustomer = false;
            try
            {
                int countOfCustomers = _dataBaseEntities.Customers.Count();
                if (countOfCustomers == 0)
                {
                    isNoCustomer = true;
                }
                else
                {
                    isNoCustomer = false;
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return isNoCustomer;
        }

        private bool DeleteCustomers(Dictionary<string, string> customerData)
        {
            bool result = false;
            try
            {
                string phoneNumbers = customerData["PhoneNumber"];
                bool isDeleted = true;
                List<Customers> deleteQuery = new List<Customers>();

                foreach (string phoneNumber in phoneNumbers.Split(','))
                {
                    Customers customerRow =
                        _dataBaseEntities.Customers.Single(customer => customer.PhoneNumber.Equals(phoneNumber));
                    deleteQuery.Add(customerRow);

                    if (customerRow != null)
                    {
                        isDeleted = true && isDeleted;
                    }
                    else
                    {
                        isDeleted = false && isDeleted;
                        break;
                    }
                }

                foreach (Customers delete in deleteQuery)
                {
                    if (isDeleted)
                    {
                        _dataBaseEntities.Customers.Remove(delete);
                        _dataBaseEntities.SaveChanges();
                        result = true;
                    }
                    else
                    {
                        result = false;
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                Log.Error(ex);
            }
            return result;
        }

        private string GetFilteredCustomers(Dictionary<string, string> customerData)
        {
            XmlNode[] xmlNodes = new XmlNode[2];
            try
            {
                string searchTerm = customerData["Search"];
                List<Customer> customersList = _dataBaseEntities.Customers
                    .Where(customers =>
                        customers.Name.Contains(searchTerm) ||
                        customers.PhoneNumber.Contains(searchTerm)).Select(customer => new Customer
                        {
                            CustomerName = customer.Name,
                            CustomerPhoneNumber = customer.PhoneNumber,
                            CustomerBirthdate = customer.BirthDate,
                            CustomerPackageName = customer.Packages.Name,
                            CustomerBalance = (decimal)customer.Balance
                        }).ToList();

                XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>));
                using (StringWriter stringWriter = new StringWriter())
                {
                    serializer.Serialize(stringWriter, customersList);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(stringWriter.ToString());
                    xmlNodes = xmlDoc.ChildNodes.Cast<XmlNode>().ToArray();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return xmlNodes[1].InnerXml;
        }

        private string RechargeBalance(Dictionary<string, string> customerData)
        {
            Customers customerBalance = new Customers();
            try
            {
                string phoneNumber = customerData["PhoneNumber"];
                string amount = customerData["Amount"];
                customerBalance = _dataBaseEntities.Customers.Single(customerPhoneNumber =>
                    customerPhoneNumber.PhoneNumber.Equals(phoneNumber));
                customerBalance.Balance = customerBalance.Balance + Convert.ToDecimal(amount);
                IFormatProvider culture = System.Globalization.CultureInfo.CurrentCulture;
                customerBalance.ExpireDate = DateTime.ParseExact(DateTime.Now.AddMonths(1).ToString("dd-MM-yyyy"),
                    "dd-MM-yyyy", culture);
                _dataBaseEntities.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return customerBalance.Balance.ToString();
        }

        private string GetCustomerInformation(Dictionary<string, string> customerData)
        {
            string customerValues = "";
            try
            {
                string phoneNumber = customerData["PhoneNumber"];
                Customer customerInformation = _dataBaseEntities.Customers
                    .Where(customers => customers.PhoneNumber.Equals(phoneNumber)).Select(
                        customer => new Customer
                        {
                            CustomerName = customer.Name,
                            CustomerPhoneNumber = customer.PhoneNumber,
                            CustomerBirthdate = customer.BirthDate,
                            CustomerPackageName = customer.Packages.Name,
                            CustomerBalance = (decimal)customer.Balance
                        }).Single();
                customerValues = string.Join(",", new[]
                {
                    customerInformation.CustomerName,
                    customerInformation.CustomerPhoneNumber,
                    customerInformation.CustomerBirthdate.ToString(),
                    customerInformation.CustomerPackageName,
                    customerInformation.CustomerBalance.ToString()
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return customerValues;
        }

        private string GetCustomers()
        {
            XmlNode[] xmlNodes = new XmlNode[2];
            try
            {
                List<Customer> customers = _dataBaseEntities.Customers.Select(customerInfo => new Customer
                {
                    CustomerName = customerInfo.Name,
                    CustomerPhoneNumber = customerInfo.PhoneNumber,
                    CustomerBirthdate = customerInfo.BirthDate,
                    CustomerPackageName = customerInfo.Packages.Name,
                    CustomerBalance = (decimal)customerInfo.Balance
                }).ToList();

                //Convert the List<Customer> object to an XmlNode[] object
                XmlSerializer serializer = new XmlSerializer(typeof(List<Customer>));
                using (StringWriter stringWriter = new StringWriter())
                {
                    serializer.Serialize(stringWriter, customers);
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(stringWriter.ToString());
                    xmlNodes = xmlDoc.ChildNodes.Cast<XmlNode>().ToArray();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }
            return xmlNodes[1].InnerXml;
        }

        private bool CheckExistingPhoneNumber(Dictionary<string, string> customerData)
        {
            bool isExist = true;
            try
            {
                string phoneNumber = customerData["PhoneNumber"];
                isExist = _dataBaseEntities.Customers.Count(
                    number => number.PhoneNumber.Equals(phoneNumber)) != 0;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
            }

            return isExist;
        }
    }
}
