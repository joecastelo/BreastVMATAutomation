using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreastVMATAutomation.Model.Templating
{
    public class LINACDefaultCalculation
    {
        public string LINAC { get; set; }
        public string PhotonCalculationModel { get; set; }
        public string DVHEstimationModel { get; set; }

        public string PortalDoseModel { get; set; }
        public string OptimizationModel { get; set; }

    }
}
