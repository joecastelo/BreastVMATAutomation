using BreastVMATAutomation.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace VMS.TPS
{
    public class Script
    {
        public void Execute(ScriptContext context)
        {
            var content = new MainWindow((ExternalPlanSetup)context.ExternalPlanSetup);
            var window = new System.Windows.Window();
            window.Content = content;
            window.Width = 560;
            window.Height = 600;
            window.ShowDialog();
        }
    }
}
