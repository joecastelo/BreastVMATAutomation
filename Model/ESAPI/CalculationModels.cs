using BreastVMATAutomation.Model.Templating;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace BreastVMATAutomation.Model.ESAPI
{
    public class CalculationModels
    {
        public static void PrepareCalcOptions(ExternalPlanSetup planSetup)
        {
            var ctx = new LINACDefaultCalculationContext("v1");
            var models = ctx.GetAllLINACDefaultCalculations();
            var done = models.Where(e => e.LINAC == planSetup.Beams.FirstOrDefault().TreatmentUnit.Id);
            if (done.Any())
            {
                try
                {
                    var doneOps = done.FirstOrDefault();
                    planSetup.ClearCalculationModel(VMS.TPS.Common.Model.Types.CalculationType.PhotonVolumeDose);
                    planSetup.SetCalculationModel(VMS.TPS.Common.Model.Types.CalculationType.PhotonVolumeDose, doneOps.PhotonCalculationModel);
                    planSetup.SetCalculationModel(VMS.TPS.Common.Model.Types.CalculationType.PhotonVMATOptimization, doneOps.OptimizationModel);
                    planSetup.SetCalculationModel(VMS.TPS.Common.Model.Types.CalculationType.DVHEstimation, doneOps.DVHEstimationModel);
                    planSetup.SetCalculationOption("PO_15.6.05", "MRLevelAtRestart", "MR4");


                    //planSetup.SetCalculationModel(VMS.TPS.Common.Model.Types.CalculationType.PhotonVMATOptimization, "PO_15151");
                }
                catch (Exception e)
                {

                    Console.WriteLine("Did not change Calculation Models");
                    Console.WriteLine(e.Message);
                }
            }





        }
    }
}
