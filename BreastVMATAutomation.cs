using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Reflection;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;
using BreastVMATAutomation.UI;

// TODO: Replace the following version attributes by creating AssemblyInfo.cs. You can do this in the properties of the Visual Studio project.
[assembly: AssemblyVersion("1.0.0.1")]
[assembly: AssemblyFileVersion("1.0.0.1")]
[assembly: AssemblyInformationalVersion("1.0")]

// TODO: Uncomment the following line if the script requires write access.
[assembly: ESAPIScript(IsWriteable = true)]

namespace BreastVMATAutomation
{
  class Program
  {
    [STAThread]
    static void Main(string[] args)
    {
      try
      {
        using (Application app = Application.CreateApplication())
        {
          Execute(app);
        }
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e.ToString());
                Console.Read();        
      }
    }
    static void Execute(Application app)
    {
            // Breast VMAT Arc Geometry Tool

            var patient = app.OpenPatientById("Script Breast Boost 2");
            // Select Isocenter
            // Create Tangential Arcs
            // Select Calculation Models
            var plan = patient.Courses.First(e => e.Id == "Breast Apr24").PlanSetups.First(x => x.Id == "Plan1");
            var content = new MainWindow((ExternalPlanSetup)plan);
            var window = new System.Windows.Window();
            window.Content = content;
            window.Width = 560;
            window.Height = 650;
            window.ShowDialog();
    }


  }
}
