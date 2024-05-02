using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMS.TPS.Common.Model.API;
using VMS.TPS.Common.Model.Types;

namespace BreastVMATAutomation.Model.ESAPI
{
    public class SelectIsocenter
    {
        public static VVector IsocenterSelectionBreast(Structure target)
        {
            return new VVector(target.CenterPoint.x - 25, target.CenterPoint.y +30 , target.CenterPoint.z);
        }
        public static VVector ChangeIsocenter(ExternalPlanSetup planSetup, Structure target)
        {
            var newIso = IsocenterSelectionBreast(target);
            foreach (var beam in planSetup.Beams)
            {
               var beamParameters =  beam.GetEditableParameters();
                beamParameters.Isocenter = newIso;
                try
                {
                    beam.ApplyParameters(beamParameters);


                }
                catch (Exception)
                {

                }

            }
            return newIso;

        }
    }
}
