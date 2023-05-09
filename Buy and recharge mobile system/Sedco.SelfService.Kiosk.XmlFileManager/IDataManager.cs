using System;
using System.Collections.Generic;
using Sedco.SelfService.Kiosk.SharedProject;

namespace Sedco.SelfService.Kiosk.DataManager
 { 

  public interface IDataManager
    {
        bool CheckExistingPhoneNumber(string phoneNumberChecking);
        List<Customer> GetCustomers();
        string RechargeBalance(string phoneNumber, string amount);
        List<string> GetPackagesList(string packageType);
        List<Customer> GetFilteredCustomers(string search);
        bool DeleteCustomers(string [] customerPhoneNumber);
        bool AddPackage(string packageType, string packageName, string packagePrice);
        bool CheckIfNoCustomers();
        bool AddCustomer(string packageName, string phoneNumber, string customerName, string customerBirthDate);
        string GetPackageTypeFromName(string packageName);
        DateTime GetExpireDate(string phoneNumber);
        bool EditCustomer(string packageName, string editCustomerName,string editCustomerBirthDate,string phoneNumber); 
        bool CheckExistingPackages(string packageName);
       Customer GetCustomerInformation(string phoneNumber);
    }
}
