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
        public static VVector IsocenterSelectionBreast(Structure target, bool left)
        {
            var addX = left ? 1 : -1;
            return new VVector(target.CenterPoint.x - 20*addX, target.CenterPoint.y +20 , target.CenterPoint.z);
        }
        public static VVector ChangeIsocenter(ExternalPlanSetup planSetup, Structure target, bool left)
        {
            var newIso = IsocenterSelectionBreast(target, left);
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
