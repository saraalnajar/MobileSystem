using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;

namespace Buy_and_recharge_mobile_system
{
    public partial class Number : Form
    { RechargeForm Recharge = new RechargeForm();

        string RandomNumber = "";
        public Number(RechargeForm charge)
        {
            Recharge = charge;

            InitializeComponent();
        }
        public string GenerateNumber()
        {

        repeat:
            Random random = new Random();
            
            int i;
            for (i = 0; i < 7; i++)
            {
                RandomNumber += random.Next(0, 9).ToString();
            }

            try
            {
                XmlDocument xml = new XmlDocument();
                xml.Load(@"customers.xml");
            }
            catch (XmlException e)
            {
                return RandomNumber;

            }
            XDocument Customersxdoc = XDocument.Load(@"customers.xml");

            var query = from PhoneNum in Customersxdoc.Descendants("Customer")
                        where PhoneNum.Element("PhoneNumber").Value == RandomNumber
                        select PhoneNum.Element("PhoneNumber").Value;
            if (query.Count() == 0)
                return RandomNumber;
            else
                goto repeat;


        }
        private void Number_Load(object sender, EventArgs e)
        {
            Num.Text = "078";
            Num.Text += GenerateNumber();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                XDocument Packagesxdoc = XDocument.Load(@"packages.xml");
                String PackageName = Recharge.PackagescomboBox.GetItemText(Recharge.PackagescomboBox.SelectedItem);
                IEnumerable<string> GetPackageIdQuery = from Package in Packagesxdoc.Descendants("Package")
                                            where Package.Element("Name").Value == PackageName
                                            select Package.Element("Id").Value;

                int PackageId = Int32.Parse(GetPackageIdQuery.First());
                IEnumerable<string> PackagePriceQuery = from Package in Packagesxdoc.Descendants("Package")
                                            where Package.Element("Name").Value == PackageName
                                            select Package.Element("Price").Value;

                int CustomerBalance = Int32.Parse(PackagePriceQuery.First());

                DateTime ExpireDate = DateTime.Now.AddMonths(3);


                XDocument Customers = XDocument.Load(@"customers.xml");
                XElement NewCustomer = new XElement("Customer");
                NewCustomer.Add(new XElement("Name", Recharge.Nametxt.Text));
                NewCustomer.Add(new XElement("PhoneNumber", RandomNumber));
                NewCustomer.Add(new XElement("BirthDate", Recharge.BirthDate.Value.ToShortDateString()));
                NewCustomer.Add(new XElement("ExpireDate", ExpireDate));
                NewCustomer.Add(new XElement("PackageId", PackageId));
                NewCustomer.Add(new XElement("Balance", CustomerBalance));
                Customers.Element("Customers").Add(NewCustomer);
                Customers.Save(@"customers.xml");
                Recharge.Nametxt.Text = "";
                Recharge.BirthDate.Value = DateTime.Now;
                Recharge.rdoPost.Checked = false;
                Recharge.rdoPre.Checked = false;
                Recharge.Searchtxt.Text = "";
                Recharge.DisplayCustomers();
                Recharge.PackagescomboBox.Items.Clear();
                this.Close();
            }
            catch (FileNotFoundException)
            {
                MessageBox.Show("The file does not exist");

            }
            catch (Exception)
            {
                MessageBox.Show("Error");
            }

        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}