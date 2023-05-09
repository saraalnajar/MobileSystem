using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Sedco.SelfService.Kiosk.LogicManagerSystem;
using Sedco.SelfService.Kiosk.SharedProject;


namespace Buy_and_recharge_mobile_system
{
    public static class Validation
    {
        private static readonly SystemLogicManager LogicManager = new SystemLogicManager();

        public static bool IsNullTextBox(TextBox fieldContent, ErrorProvider errorProvider, string regex = "", string regexType = "")
        {
            bool checkResult;
            string setErrorContent;
            if (string.IsNullOrWhiteSpace(fieldContent.Text))
            {
                fieldContent.Focus();
                setErrorContent = "this field is required!";
                checkResult = false;
            }
            else
            {
                if (regex != "")
                {
                    if (!LogicManager.CheckValidity(regex, fieldContent.Text))
                    {
                        setErrorContent = $"this field should contain only {regexType}";
                        checkResult = false;
                    }
                    else
                    {
                        setErrorContent = "";
                        checkResult = true;
                    }
                }
                else
                {
                    setErrorContent = "";
                    checkResult = true;
                }
            }
            errorProvider.SetError(fieldContent, setErrorContent);
            return checkResult;
        }

        public static bool IsNullComboBox(ComboBox comboBox, ErrorProvider errorProvider)
        {
            bool checkResult;
            string setErrorContent;
            if (comboBox.SelectedItem == null)
            {
                setErrorContent = "You should choose Package";
                checkResult = false;
            }
            else
            {
                setErrorContent = "";
                checkResult = true;
            }
            errorProvider.SetError(comboBox, setErrorContent);

            return checkResult;
        }

        public static bool IsCheckedRadioButtons(RadioButton optionOne, RadioButton optionTwo, ErrorProvider errorProvider)
        {
            bool checkResult;
            string setErrorContent;

            if (!(optionOne.Checked || optionTwo.Checked))
            {
                setErrorContent = "Paid type is required!";
                checkResult = false;
            }
            else
            {
                setErrorContent = "";
                checkResult = true;
            }
            errorProvider.SetError(optionTwo, setErrorContent);
            return checkResult;
        }

        public static bool ValidityBirthDate(int age, ErrorProvider errorProvider, DateTimePicker birthDate)
        {
            bool checkResult;
            string setErrorContent;
            if (age < 18)
            {
                setErrorContent = "The age must be 18 or more";
                checkResult = false;
            }
            else
            {
                setErrorContent = "";
                checkResult = true;
            }
            errorProvider.SetError(birthDate, setErrorContent);

            return checkResult;
        }

        public static bool CheckValidityPhoneNumber(TextBox fieldName, ErrorProvider errorProvider)
        {
            bool checkValidity;
            string setErrorContent;
            bool isExistPhoneNumbers = LogicManager.CheckExistingPhoneNumber(fieldName.Text);
            if (!isExistPhoneNumbers)
            {
                setErrorContent = "Can't Find the Number";
                checkValidity = false;
            }
            else
            {
                setErrorContent = "";
                checkValidity = true;
            }
            errorProvider.SetError(fieldName, setErrorContent);
            return checkValidity;
        }
        public static bool CheckExistingPackage(TextBox fieldName, ErrorProvider errorProvider)
        {
            string setErrorContent;
            bool checkValidity;
            bool packageExist = LogicManager.CheckExistingPackages(fieldName.Text);
            if (packageExist)
            {
                setErrorContent = "This Package already exist";
                checkValidity = false;
            }
            else
            {
                setErrorContent = "";
                checkValidity = true;
            }
            errorProvider.SetError(fieldName, setErrorContent);
            return checkValidity;
        }
        public static void DisplayErrorMessage(Exception ex, string errorMessageBox = "")
        {
            if (errorMessageBox != "")
            {
                MessageBox.Show(errorMessageBox);
            }

            WriteToLogFile.WriteToLogErrorFile(ex);
        }
    }
}