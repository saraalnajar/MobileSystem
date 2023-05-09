using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using Sedco.SelfService.Kiosk.LogicManagerSystem;
using Sedco.SelfService.Kiosk.SharedProject;

namespace Buy_and_recharge_mobile_system
{
    public partial class RechargeForm : Form
    {
        private readonly SystemLogicManager _logicManager = new SystemLogicManager();
        private const int PhoneNumber = 1;
        private const string DateFormat = "dd/MM/yyyy";
        private const string Success = "Added Successfully";
        private const string Fail = "Failed";
        private const string Names = "Name";
        private const string Phone = "Phone Number";
        private const string BirthDate = "BirthDate";
        private const string Package = "Package";
        private const string Balance = "Balance";

        public RechargeForm()
        {
            InitializeComponent();
        }

        private void EnableButton(bool enable)
        {
            deleteCustomerButton.Enabled = enable;
            editCustomerButton.Enabled = enable;
        }

        public string AddingToDataGridView(List<Customer> customers)
        {
            if (customers.Count > 0)
            {
                customersInformationDataView.Rows.Clear();
                EnableButton(true);
                foreach (Customer row in customers)
                {
                    customersInformationDataView.Rows.Add(row.CustomerName, row.CustomerPhoneNumber,
                        row.CustomerBirthdate, row.CustomerPackageName, row.CustomerBalance);
                }
            }
            else
            {
                customersInformationDataView.Rows.Clear();
                EnableButton(false);
            }
            return customersInformationDataView.RowCount.ToString();
        }

        public void AddToComboBox(ComboBox comboBoxName, List<string> items)
        {
            foreach (string packageItems in items)
            {
                comboBoxName.Items.Add(packageItems);
            }
        }
        private void RechargeButton_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = true;
                bool checkValidity;
                isValid = Validation.IsNullTextBox(amountText, amountError, @"^[0-9]([.][0-9]{1,3})?$", "Number") && isValid;
                checkValidity = Validation.IsNullTextBox(phoneNumberText, phoneNumberError, @"^\d{7}$", "number from 7 digits");
                if (checkValidity)
                {
                    isValid = Validation.CheckValidityPhoneNumber(phoneNumberText, phoneNumberError) && isValid;
                }
                else
                {
                    isValid = isValid && checkValidity;
                }
                if (isValid)
                {
                    string currentBalance = _logicManager.RechargeBalance(phoneNumberText.Text, amountText.Text);
                    WriteToLogFile.WriteToLogStoryFile($"The Recharge Button has been Clicked \n the Phone Number:{phoneNumberText.Text}\n the Amount is :{amountText.Text}");
                    phoneNumberText.Text = "";
                    amountText.Text = "";
                    searchText.Text = "";
                    List<Customer> customers = _logicManager.GetAllCustomer();
                    AddingToDataGridView(customers);
                    MessageBox.Show($"Success \n Current Balance:{currentBalance}");
                }
                else
                {
                    resultOfRechargeLabel.ForeColor = Color.Red;
                    resultOfRechargeLabel.Text = Fail;
                }

            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex);
            }
        }


        private void AddCustomerButton_Click(object sender, EventArgs e)
        {
            try
            {
                int age = _logicManager.CalculateAge(customerBirthDatePicker.Value);
                bool isValid = true;
                isValid = Validation.IsNullTextBox(customerNameText, customerNameError, @"^[a-zA-Z ]+$", "alphabets") && isValid;
                isValid = Validation.IsNullComboBox(customerPackagesComboBox, packageNameError) && isValid;
                isValid = Validation.IsCheckedRadioButtons(customerPrepaidRadioButton, customerPostpaidRadioButton, paidTypeError) && isValid;
                isValid = Validation.ValidityBirthDate(age, birthDateError, customerBirthDatePicker) && isValid;
                if (isValid)
                {

                    string phoneNumber = _logicManager.GenerateNumber();
                    Number phoneNumberForm = new Number(phoneNumber);
                    try
                    {
                        tabControl.Enabled = false;
                        DialogResult opeDialogResult = phoneNumberForm.ShowDialog();
                        if (opeDialogResult == DialogResult.OK)
                        {
                            if (!_logicManager.AddCustomer(
                                    customerPackagesComboBox.GetItemText(customerPackagesComboBox
                                        .SelectedItem).ToString(), phoneNumber, customerNameText.Text, customerBirthDatePicker.Value.ToString(DateFormat)))
                            {
                                MessageBox.Show(Fail);
                            }
                            else
                            {

                                WriteToLogFile.WriteToLogStoryFile(
                                    $"the ok is clicked and the Customer with the Name = {customerNameText.Text} \n PhoneNumber= {phoneNumber} \n Birthdate={customerBirthDatePicker.Value.ToString(DateFormat)} Package = " +
                                    $"{customerPackagesComboBox.SelectedItem.ToString()} is Added");

                                EnableButton(true);
                            }

                            ClearFields(); 
                            List<Customer> customers = _logicManager.GetAllCustomer();
                            AddingToDataGridView(customers);
                        }

                        tabControl.Enabled = true;
                    }
                    catch (FileNotFoundException ex)
                    {
                        Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

                    }
                    catch (Exception ex)
                    {
                        Validation.DisplayErrorMessage(ex, "error in open phone number window");
                        tabControl.Enabled = true;
                        phoneNumberForm.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex, "error in open phone number window");
            }
        }

        private void ClearFields()
        {
            customerNameText.Text = "";
            customerBirthDatePicker.Value = DateTime.Now;
            customerPostpaidRadioButton.Checked = false;
            customerPrepaidRadioButton.Checked = false;
            searchText.Text = "";
            customerPackagesComboBox.Items.Clear();
            customerPackagesComboBox.Enabled = false;
            customerPackagesComboBox.Items.Clear();
        }

        private void CustomerPrepaidRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                customerPackagesComboBox.Items.Clear();
                customerPackagesComboBox.Enabled = true;
                List<string> nameOfPackages = _logicManager.AddItems("Prepaid");
                AddToComboBox(customerPackagesComboBox, nameOfPackages);
            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");
            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex, "error in load prepaid package combobox");
            }
        }

        private void CustomerPostpaidRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            try
            {
                customerPackagesComboBox.Items.Clear();
                customerPackagesComboBox.Enabled = true;
                List<string> nameOfPackages = _logicManager.AddItems("Postpaid");
                AddToComboBox(customerPackagesComboBox, nameOfPackages);
            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex, "error in load postpaid package combobox");
            }
        }

        private void RechargeForm_Load(object sender, EventArgs e)
        {
            try
            {
                resultOfRechargeLabel.Text = "";
                resultOfAddPackageLabel.Text = "";
                customerBirthDatePicker.Format = DateTimePickerFormat.Custom;
                customerBirthDatePicker.CustomFormat = DateFormat;
                customersInformationDataView.ColumnCount = 5;
                customersInformationDataView.Columns[0].Name = Names;
                customersInformationDataView.Columns[1].Name = Phone;
                customersInformationDataView.Columns[2].Name = BirthDate;
                customersInformationDataView.Columns[3].Name = Package;
                customersInformationDataView.Columns[4].Name = Balance;
                for (int i = 0; i <= 4; i++)
                { customersInformationDataView.Columns[i].Width = 145; }

                List<Customer> customers = _logicManager.GetAllCustomer();
                AddingToDataGridView(customers);
            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex, "error in load the application");
                this.Close();
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            resultOfAddPackageLabel.Text = "";
            resultOfRechargeLabel.Text = "";
            searchText.Text = "";
            List<Customer> customers = _logicManager.GetAllCustomer();
            AddingToDataGridView(customers);
            ClearErrors();
            if (customerPrepaidRadioButton.Checked)
            {
                customerPostpaidRadioButton.Checked = true;
                customerPrepaidRadioButton.Checked = true;
            }
            else if (customerPostpaidRadioButton.Checked)
            {
                customerPrepaidRadioButton.Checked = true;
                customerPostpaidRadioButton.Checked = true;
            }
        }

        private void ClearErrors()
        {
            customerNameError.SetError(customerNameText, "");
            birthDateError.SetError(customerBirthDatePicker, "");
            paidTypeError.SetError(customerPostpaidRadioButton, "");
            packagesNameError.SetError(customerPackagesComboBox, "");
            phoneNumberError.SetError(phoneNumberText, "");
            amountError.SetError(amountText, "");
            addPackagePaidTypeError.SetError(packagePostpaidRadiobutton, "");
            packageNameError.SetError(packageNameText, "");
            packagePriceError.SetError(packagePriceText, "");
           
        }

        private void AddPackageButton_Click(object sender, EventArgs e)
        {
            try
            {
                bool isValid = true;
                isValid = Validation.IsCheckedRadioButtons(packagePrepaidRadioButton, packagePostpaidRadiobutton,
                    addPackagePaidTypeError) && isValid;
                if (Validation.IsNullTextBox(packageNameText, packageNameError))
                {
                    isValid = Validation.CheckExistingPackage(packageNameText, packageNameError) && isValid;
                }
                else
                {
                    isValid = Validation.IsNullTextBox(packageNameText, packageNameError) && isValid;
                }

                isValid = Validation.IsNullTextBox(packagePriceText, packagePriceError, @"^[0-9]+$", "Number") && isValid;
                if (isValid)
                {
                    string packageTypeName = packagePrepaidRadioButton.Checked ? "Prepaid" : "Postpaid";
                    if (_logicManager.AddPackage(packageTypeName, packageNameText.Text, packagePriceText.Text))
                    {
                        WriteToLogFile.WriteToLogStoryFile(
                            $"The Package Added Button is Clicked \n Package Type:{packageTypeName}\n" +
                            $"the Name of Package:{packageNameText.Text}\n The Price: {packagePriceText.Text}");
                        packageNameText.Text = "";
                        packagePriceText.Text = "";
                        packagePrepaidRadioButton.Checked = false;
                        packagePostpaidRadiobutton.Checked = false;
                        resultOfAddPackageLabel.Text = Success;
                        resultOfAddPackageLabel.ForeColor = Color.Green;
                        resultOfAddPackageLabel.Location = new Point(413, 443);
                    }
                    else
                    {
                        resultOfAddPackageLabel.Location = new Point(493, 450);
                        resultOfAddPackageLabel.ForeColor = Color.Red;
                        resultOfAddPackageLabel.Text = Fail;
                    }
                }
                else
                {
                    resultOfAddPackageLabel.Location = new Point(493, 450);
                    resultOfAddPackageLabel.ForeColor = Color.Red;
                    resultOfAddPackageLabel.Text = Fail;

                }
            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex);
            }
        }


        private void DeleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                DialogResult deleteMessageBox = MessageBox.Show("Are you sure want to delete this customer ?", "Delete",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                WriteToLogFile.WriteToLogStoryFile($"The Delete Button is Clicked and The Message Box is displayed");
                if (deleteMessageBox == DialogResult.Yes)
                {
                    WriteToLogFile.WriteToLogStoryFile("The Yes Button on Message Box is Clicked");
                    IEnumerable<DataGridViewRow> selectedCells = customersInformationDataView.SelectedCells
                        .Cast<DataGridViewCell>()
                        .Select(cell => cell.OwningRow)
                        .Distinct();
                    DataGridViewRow[] selectedRows = selectedCells.OfType<DataGridViewRow>()
                        .Where(row => !row.IsNewRow).ToArray();
                    string[] phoneNumbers = new string[selectedRows.Length];
                    int numRows = customersInformationDataView.Rows.Count;

                    for (int i = 0; i < selectedRows.Length; i++)
                    {
                        phoneNumbers[i] = selectedRows[i].Cells[PhoneNumber].Value.ToString();
                    }
                    if (!_logicManager.DeleteCustomers(phoneNumbers))
                    {
                        MessageBox.Show("Can't find the selected Customer");
                        return;
                    }

                    foreach (var row in selectedRows)
                    {
                        customersInformationDataView.Rows.Remove(row);
                    }


                    if (phoneNumbers.Length == numRows)
                    {
                        EnableButton(false);
                    }
                    foreach (string customerPhoneNumber in phoneNumbers)
                    {
                        WriteToLogFile.WriteToLogStoryFile($"The Selected row with phoneNumber {customerPhoneNumber} is deleted");

                    }
                }
                else
                {
                    WriteToLogFile.WriteToLogStoryFile("The No Button on Message Box is Clicked And The Message Box is Closed");
                }


            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex, "error in delete customer/s");

            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            try
            {
                WriteToLogFile.WriteToLogStoryFile(
                    $"The Edit Button of the {customersInformationDataView.CurrentRow.Cells[0].Value.ToString()} Customer is Clicked");
                Edit editData = new Edit(customersInformationDataView.CurrentRow.Cells[PhoneNumber].Value.ToString());
                tabControl.Enabled = false;
                editData.ShowDialog();
                searchText.Text = "";
                List<Customer> customers = _logicManager.GetAllCustomer();
                AddingToDataGridView(customers);
                tabControl.Enabled = true;

            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex, "error in open edit customer window");
            }
        }

        private void exitTheApplicationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult exitMessageBox = MessageBox.Show("Are you sure want to exit the application ?", "Exit",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (exitMessageBox == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void phoneNumberText_Click(object sender, EventArgs e)
        {
            resultOfRechargeLabel.Text = "";
        }

        private void amountText_TextChanged(object sender, EventArgs e)
        {
            resultOfRechargeLabel.Text = "";
        }

        private void phoneNumberText_TextChanged(object sender, EventArgs e)
        {
            resultOfRechargeLabel.Text = "";
        }

        private void amountText_Click(object sender, EventArgs e)
        {
            resultOfRechargeLabel.Text = "";
        }

        private void packagePrepaidRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            resultOfAddPackageLabel.Text = "";
        }

        private void packagePostpaidRadiobutton_CheckedChanged(object sender, EventArgs e)
        {
            resultOfAddPackageLabel.Text = "";
        }

        private void packageNameText_TextChanged(object sender, EventArgs e)
        {
            resultOfAddPackageLabel.Text = "";
        }

        private void packagePriceText_TextChanged(object sender, EventArgs e)
        {
            resultOfAddPackageLabel.Text = "";
        }

        private void packageNameText_Click(object sender, EventArgs e)
        {
            resultOfAddPackageLabel.Text = "";
        }

        private void packagePriceText_Click(object sender, EventArgs e)
        {
            resultOfAddPackageLabel.Text = "";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (searchText.Text != "")
                {
                    List<Customer> specisfedCustomers = _logicManager.FilterTheSearch(searchText.Text);
                    string numberOfRows = AddingToDataGridView(specisfedCustomers);
                    WriteToLogFile.WriteToLogStoryFile($"Searching about {searchText.Text} and displayed {numberOfRows} customers");
                }
                else
                {
                    List<Customer> customers = _logicManager.GetAllCustomer();
                    string numberOfRows = AddingToDataGridView(customers);
                    WriteToLogFile.WriteToLogStoryFile($"The Search Text-box is empty and displayed {numberOfRows} customers");
                }
            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex, "error in get customers");
                customersInformationDataView.Rows.Clear();
            }
        }


    }
}