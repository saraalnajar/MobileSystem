using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using EntityFarmework;
using Sedco.SelfService.Kiosk.SharedProject;

namespace BuyAndRechargeRestfulService.Controllers
{
    public class BuyAndRechargeController : ApiController
    {
        public readonly BuyAndRechargeSystemModel _dataBaseEntities = new BuyAndRechargeSystemModel(new DatabaseConfiguration().GetAppConfiguration());
        [HttpGet]
        [ActionName("phoneNumberChecking")]
        public bool CheckExistingPhoneNumber(string phoneNumberChecking)
        {
            return (_dataBaseEntities.Customers.Count(number => number.PhoneNumber == phoneNumberChecking) != 0);
        }

        [ActionName("GetCustomers")]
        public List<Customer> GetCustomers()
        {
            List<Customer> customerList = new List<Customer>();
            customerList = _dataBaseEntities.Customers.Select(customer => new Customer
            {
                CustomerName = customer.Name,
                CustomerPhoneNumber = customer.PhoneNumber,
                CustomerBirthdate = customer.BirthDate,
                CustomerPackageName = customer.Packages.Name,
                CustomerBalance = (decimal)customer.Balance
            }).ToList();

            return customerList;
        }

        [ActionName("GetCustomerInformation")]
        public Customer GetCustomerInformation(string phoneNumber)
        {
            Customer customerInformation;
            customerInformation = _dataBaseEntities.Customers
                .Where(customer => customer.PhoneNumber.Equals(phoneNumber)).Select(customer => new Customer
                {
                    CustomerName = customer.Name,
                    CustomerPhoneNumber = customer.PhoneNumber,
                    CustomerBirthdate = customer.BirthDate,
                    CustomerPackageName = customer.Packages.Name,
                    CustomerBalance = (decimal)customer.Balance
                }).Single();

            return customerInformation;
        }

        [HttpPut]
        [ActionName("RechargeBalance")]
        public string RechargeBalance(string phoneNumber, string amount)
        {
            Customers customer = _dataBaseEntities.Customers.Single(customerPhoneNumber =>
                customerPhoneNumber.PhoneNumber.Equals(phoneNumber));
            customer.Balance = customer.Balance + Convert.ToDecimal(amount);
            IFormatProvider culture = System.Globalization.CultureInfo.CurrentCulture;
            customer.ExpireDate = DateTime.ParseExact(DateTime.Now.AddMonths(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", culture);
            _dataBaseEntities.SaveChanges();
            return customer.Balance.ToString();
        }

        [ActionName("GetPackagesList")]
        public List<string> GetPackagesList(string packageType)
        {
            List<string> packagesList = _dataBaseEntities.Packages.Where(package => package.Type.Equals(packageType))
                .Select(package => package.Name).ToList();
            return packagesList;
        }

        [ActionName("GetFilteredCustomers")]
        public List<Customer> GetFilteredCustomers(string search)
        {
            List<Customer> customerList = new List<Customer>();
            customerList =
                _dataBaseEntities.Customers.Where(customer => customer.Name.Contains(search) || customer.PhoneNumber.Contains(search)).Select(customer => new Customer
                {
                    CustomerName = customer.Name,
                    CustomerPhoneNumber = customer.PhoneNumber,
                    CustomerBirthdate = customer.BirthDate,
                    CustomerPackageName = customer.Packages.Name,
                    CustomerBalance = Convert.ToDecimal(customer.Balance)
                }).ToList();
            return customerList;
        }

        [ActionName("DeleteCustomers")]
        public bool DeleteCustomers(string[] customerPhoneNumber)
        {
            bool isDeleted = true;
            bool result = false;
            List<Customers> deleteQuery = new List<Customers>();
            foreach (string phoneNumber in customerPhoneNumber)
            {
                Customers customerRow = _dataBaseEntities.Customers.Single(customer => customer.PhoneNumber.Equals(phoneNumber));
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

            return result;
        }

        [HttpPost]
        [ActionName("AddPackage")]
        public bool AddPackage(string packageType, string packageName, string packagePrice)
        {
            bool isAdded = false;
            try
            {
                Packages package = new Packages();
                package.Name = packageName;
                package.Type = packageType;
                package.price = Int32.Parse(packagePrice);
                _dataBaseEntities.Packages.Add(package);
                _dataBaseEntities.SaveChanges();
                isAdded = true;
            }
            catch
            {
                isAdded = false;
            }

            return isAdded;
        }

        [HttpGet]
        [ActionName("CheckIfNoCustomers")]
        public bool CheckIfNoCustomers()
        {
            bool isNoCustomer = false;
            int countOfCustomers = _dataBaseEntities.Customers.Count();
            if (countOfCustomers == 0)
            {
                isNoCustomer = true;
            }
            else
            {
                isNoCustomer = false;
            }
            return isNoCustomer;
        }

        [HttpPost]
        [ActionName("AddCustomer")]
        public bool AddCustomer(Customer newCustomer)
        {
            bool result = false;
            try
            {
                Customers customer = new Customers();
                int packageId = _dataBaseEntities.Packages.Where(package => package.Name.Equals(newCustomer.CustomerPackageName))
                    .Select(package => package.ID).Single();
                int packagePrice = _dataBaseEntities.Packages.Where(package => package.Name.Equals(newCustomer.CustomerPackageName))
                    .Select(package => package.price).Single();
                string expireDateADateTime = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd HH:mm:ss");
                DateTime expireDate = DateTime.ParseExact(expireDateADateTime, "yyyy-MM-dd HH:mm:ss",
                    CultureInfo.InvariantCulture);
                customer.Name = newCustomer.CustomerName;
                customer.PhoneNumber = newCustomer.CustomerPhoneNumber;
                customer.BirthDate = newCustomer.CustomerBirthdate;
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
            }
            catch (Exception ex)
            {
                result = false;
            }

            return result;
        }

        [ActionName("GetPackageTypeFromName")]
        public string GetPackageTypeFromName(string packageName)
        {
            string packageType = _dataBaseEntities.Packages.Where(package => package.Name.Equals(packageName))
                .Select(package => package.Type).Single();
            return packageType;
        }

        [ActionName("GetExpireDate")]
        public DateTime GetExpireDate(string phoneNumber)
        {
            DateTime? expireDate = _dataBaseEntities.Customers
                .Where(customer => customer.PhoneNumber.Equals(phoneNumber)).Select(customer => customer.ExpireDate)
                .Single();
            return (DateTime)expireDate;
        }

        [HttpPut]
        [ActionName("EditCustomer")]
        public bool EditCustomer(string packageName, string editCustomerName, string editCustomerBirthDate, string phoneNumber)
        {
            bool isEdited = false;

            Customers customer =
                _dataBaseEntities.Customers.FirstOrDefault(customerPhone => customerPhone.PhoneNumber.Equals(phoneNumber));
            if (customer != null)
            {
                int packageId = _dataBaseEntities.Packages.Where(package => package.Name.Equals(packageName))
                    .Select(package => package.ID).Single();
                customer.Name = editCustomerName;
                customer.BirthDate = editCustomerBirthDate;
                customer.PackageID = packageId;
                _dataBaseEntities.SaveChanges();
                isEdited = true;
            }
            else
            {
                isEdited = false;
            }
            return isEdited;
        }

        [HttpGet]
        [ActionName("CheckExistingPackages")]
        public bool CheckExistingPackages(string packageName)
        {
            bool existPackage = false;
            int packages = _dataBaseEntities.Packages.Where(package => package.Name == packageName)
                .Select(package => packageName).Count();
            if (packages != 0)
            {
                existPackage = true;
            }
            return existPackage;
        }
    }
}
