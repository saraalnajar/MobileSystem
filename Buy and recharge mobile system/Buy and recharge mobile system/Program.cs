using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.ComponentModel;

namespace Buy_and_recharge_mobile_system
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MyHandler);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            string[] args = Environment.GetCommandLineArgs();
            SingleInstanceController controller = new SingleInstanceController();
            controller.Run(args);
        }

        public static void MyHandler(object sender, UnhandledExceptionEventArgs e)
        {
            string message = "Sorry, something went wrong.\r\n Please contact support.";
            Validation.DisplayErrorMessage(((Exception)e.ExceptionObject), message);
        }
    }
   
}
