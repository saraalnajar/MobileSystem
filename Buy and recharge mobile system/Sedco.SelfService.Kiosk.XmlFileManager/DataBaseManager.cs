using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Globalization;
using System.Linq;
using EntityFarmework;
using Sedco.SelfService.Kiosk.SharedProject;
namespace Sedco.SelfService.Kiosk.DataManager
{
    public class DataBaseManager : IDataManager
    {
        public readonly BuyAndRechargeSystemModel _dataBaseEntities = new BuyAndRechargeSystemModel(new DatabaseConfiguration().GetAppConfiguration());

        public bool CheckExistingPhoneNumber(string phoneNumberChecking)
        {
            return (_dataBaseEntities.Customers.Count(number => number.PhoneNumber == phoneNumberChecking) != 0);
        }
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

        public Customer GetCustomerInformation(string phoneNumber)
        {
            Customer customerInformation;
            customerInformation = _dataBaseEntities.Customers.Where(customer => customer.PhoneNumber.Equals(phoneNumber)).Select(customer => new Customer
            {
                CustomerName = customer.Name,
                CustomerPhoneNumber = customer.PhoneNumber,
                CustomerBirthdate = customer.BirthDate,
                CustomerPackageName = customer.Packages.Name,
                CustomerBalance = (decimal)customer.Balance
            }).Single();

            return customerInformation;
        }
        public string RechargeBalance(string phoneNumber, string amount)
        {//FirstOrDefault --> single
            Customers customer = _dataBaseEntities.Customers.Single(customerPhoneNumber =>
                customerPhoneNumber.PhoneNumber.Equals(phoneNumber));
            customer.Balance = customer.Balance + Convert.ToDecimal(amount);
            IFormatProvider culture = System.Globalization.CultureInfo.CurrentCulture;
            customer.ExpireDate = DateTime.ParseExact(DateTime.Now.AddMonths(1).ToString("dd-MM-yyyy"), "dd-MM-yyyy", culture);
            _dataBaseEntities.SaveChanges();
            return customer.Balance.ToString();
        }

        public List<string> GetPackagesList(string packageType)
        {
            List<string> packagesList = _dataBaseEntities.Packages.Where(package => package.Type.Equals(packageType))
                .Select(package => package.Name).ToList();
            return packagesList;
        }

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

        public bool AddCustomer(string packageName, string phoneNumber, string customerName, string customerBirthDate)
        {
            bool result = false;
            try
            {
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
                customer.BirthDate = customerBirthDate;
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
                WriteToLogFile.WriteToLogErrorFile(e);
            }
            catch (Exception ex)
            {
                result = false;
                WriteToLogFile.WriteToLogErrorFile(ex);
            }

            return result;
        }

        public string GetPackageTypeFromName(string packageName)
        {
            string packageType = _dataBaseEntities.Packages.Where(package => package.Name.Equals(packageName))
                .Select(package => package.Type).Single();
            return packageType;
        }

        public DateTime GetExpireDate(string phoneNumber)
        {
            DateTime? expireDate = _dataBaseEntities.Customers
                .Where(customer => customer.PhoneNumber.Equals(phoneNumber)).Select(customer => customer.ExpireDate)
                .Single();
            return (DateTime)expireDate;
        }

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
