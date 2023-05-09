using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic.ApplicationServices;


namespace Buy_and_recharge_mobile_system
{
    public class SingleInstanceController : WindowsFormsApplicationBase
    {
        public SingleInstanceController()
        {
            IsSingleInstance = true;
        }

        protected override void OnStartupNextInstance(StartupNextInstanceEventArgs eventArgs)
        {
            RechargeForm form = MainForm as RechargeForm;
            form.Activate();
            base.OnStartupNextInstance(eventArgs);
        }

        protected override void OnCreateMainForm()
        {
            MainForm = new RechargeForm();
        }
    }
}
