using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Runtime.Serialization;

namespace Sedco.SelfService.Kiosk.SharedProject
{
    [DataContract]
    public class Customer
    {

        public Customer()
        {

        }
        public Customer(string customerName, string customerPhoneNumber, string customerBirthdate,
            string customerPackageName, decimal customerBalance)
        {
            CustomerName = customerName;
            CustomerPhoneNumber = customerPhoneNumber;
            CustomerBirthdate = customerBirthdate;
            CustomerPackageName = customerPackageName;
            CustomerBalance = customerBalance;
        }
        [DataMember]
        public string CustomerName
        {
            get;
            set;
        }
        [DataMember]
        public string CustomerPhoneNumber
        {
            get;
            set;
        }
        [DataMember]
        public string CustomerBirthdate
        {
            get;
            set;
        }
        [DataMember]
        public string CustomerPackageName
        {
            get;
            set;
        }
        [DataMember]
        public decimal CustomerBalance
        {
            get;
            set;
        }
    }
}
