using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using Sedco.SelfService.Kiosk.LogicManagerSystem;
using Sedco.SelfService.Kiosk.SharedProject;


namespace Buy_and_recharge_mobile_system
{
    public partial class Number : Form
    {
        private readonly SystemLogicManager _logicManager = new SystemLogicManager();
        private readonly string _phoneNumber;

        private const string StandardNumber = "078";
        public Number(string phoneNumber)
        {
            this._phoneNumber = phoneNumber;
            InitializeComponent();
        }

        private void Number_Load(object sender, EventArgs e)
        {
            try
            {
                Num.Text = StandardNumber;
                Num.Text += _phoneNumber;

                WriteToLogFile.WriteToLogStoryFile($"The Phone Number Form display the generated Phone Number:{_phoneNumber}");
            }
            catch (FileNotFoundException ex)
            {
                this.Close();
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

            }
            catch (Exception ex)
            {
                Close();
                Validation.DisplayErrorMessage(ex, "error in load phone number form");

            }
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
                WriteToLogFile.WriteToLogStoryFile("The Cancel Button has been clicked and the form closed");
            }
            catch (FileNotFoundException ex)
            {
                Validation.DisplayErrorMessage(ex, $"Error \n {ex.FileName} not exist");

            }
            catch (Exception ex)
            {
                Validation.DisplayErrorMessage(ex, "error can't cancel Number Form");

            }
        }
    }
}