using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Sedco.SelfService.Kiosk.LogicManagerSystem;
using Sedco.SelfService.Kiosk.SharedProject;

namespace Buy_and_recharge_mobile_system
{
    public partial class Edit : Form
    {
        private readonly string _customerPhoneNumber = "";
        private const string DateFormat = "dd/MM/yyyy";
        private const string Prepaid = "Prepaid";
        private const string Postpaid = "Postpaid";
        private readonly SystemLogicManager _logicManager = new SystemLogicManager();
        public Edit(string customerPhoneNumber)
        {
            this._customerPhoneNumber = customerPhoneNumber;
            InitializeComponent();
        }

        private void Edit_Load(object sender, EventArgs e)
        {
            try
            {
                Customer editCustomer = _logicManager.GetCustomerInformation(_customerPhoneNumber);
                DateTime birthdate = DateTime.ParseExact((editCustomer.CustomerBirthdate), DateFormat, null);
                editCustomerNameText.Text = editCustomer.CustomerName;
                editCustomerBirthDatePicker.Format = DateTimePickerFormat.Custom;
                editCustomerBirthDatePicker.CustomFormat = DateFormat;
                editCustomerPhoneNumberText.Text = editCustomer.CustomerPhoneNumber;
                editCustomerBirthDatePicker.Value = birthdate;
                editCustomerBalanceText.Text = editCustomer.CustomerBalance.ToString();
                string typeOfPackage = _logicManager.GetPackageType(editCustomer.CustomerPackageName);
                DateTime expireDate = _logicManager.GetExpireDate(editCustomer.CustomerPhoneNumber);
                editCustomerExpireDateText.Text = expireDate.ToString("dd/MM/yyyy");

                if (typeOfPackage == Prepaid)
                {
                    editCustomerPrepaidRadioButon.Checked = true;
                }
                else
                {
                    editCustomerPostpaidRadioButton.Checked = true;
                }

                for (int i = 0; i < editlCustomerPackagesComboBox.Items.Count; i++)
                {
                    if (editlCustomerPackagesComboBox.GetItemText(editlCustomerPackagesComboBox.Items[i]) ==
                        editCustomer.CustomerPackageName)
                    {
                        editlCustomerPackagesComboBox.SelectedIndex = i;
                    }
                }

                WriteToLogFile.WriteToLogStoryFile("The Edit Form has been displayed");
            }
            catch (Exception ex)
            {
                this.Close();
                Validation.DisplayErrorMessage(ex, "error in load Edit Form");

            }
        }

        private void EditCustomerPrepaidRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                editlCustomerPackagesComboBox.Items.Clear();
                List<string> nameOfPackages = _logicManager.AddItems(Prepaid);
                AddToComboBox(editlCustomerPackagesComboBox, nameOfPackages);
                editlCustomerPackagesComboBox.SelectedIndex = 0;

            }
            catch (Exception ex)
            {
                this.Close();
                Validation.DisplayErrorMessage(ex, "error in get prepaid package list in combobox");

            }
        }

        private void EditCustomerPostpaidRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                editlCustomerPackagesComboBox.Items.Clear();
                List<string> nameOfPackages = _logicManager.AddItems(Postpaid);
                AddToComboBox(editlCustomerPackagesComboBox, nameOfPackages);
                editlCustomerPackagesComboBox.SelectedIndex = 0;


            }
            catch (Exception ex)
            {
                this.Close();
                Validation.DisplayErrorMessage(ex, "error in get postpaid package list in combobox");
            }
        }

        public void AddToComboBox(ComboBox comboBoxName, List<string> items)
        {
            foreach (string packageItems in items)
            {
                comboBoxName.Items.Add(packageItems);
            }
        }
        private void EditSaveButton_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = true;
                int age = _logicManager.CalculateAge(editCustomerBirthDatePicker.Value);
                isValid = Validation.IsNullTextBox(editCustomerNameText, editNameError, @"^[a-zA-Z ]+$", "alphabets") && isValid;
                isValid = Validation.ValidityBirthDate(age, birthDateError, editCustomerBirthDatePicker) && isValid;
                if (isValid)
                {
                    try
                    {
                        if (!_logicManager.EditCustomer((editlCustomerPackagesComboBox.GetItemText(editlCustomerPackagesComboBox.SelectedItem).ToString()), editCustomerNameText.Text, editCustomerBirthDatePicker.Value.ToString("dd/MM/yyyy"), editCustomerPhoneNumberText.Text))
                        {
                            MessageBox.Show("Can't find this Customer");
                        }

                        WriteToLogFile.WriteToLogStoryFile($"The Save Button is clicked and the Customer information is edited to \n" +
                                                           $"Name:{editCustomerNameText.Text} \n BirthDate:{editCustomerBirthDatePicker.Value.ToShortDateString()}\n Package Name:{editlCustomerPackagesComboBox.GetItemText(editlCustomerPackagesComboBox.SelectedItem)}");

                        this.DialogResult = DialogResult.OK;
                    }
                    catch (FileNotFoundException ex)
                    {

                        this.Close();
                        Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

                    }
                }

                else
                {
                    failedLabel.Text = "Failed";
                    this.DialogResult = DialogResult.None;
                }
            }
            catch (Exception ex)
            {
                this.Close();
                Validation.DisplayErrorMessage(ex, "error in save editing");

            }

        }

        private void EditCancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.Close();
                WriteToLogFile.WriteToLogStoryFile("The Cancel Button is Clicked and the Form is Closed");
            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");
            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex, "error in cancel Edit Form");
            }

        }
    }
}