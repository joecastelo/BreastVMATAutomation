using BreastVMATAutomation.Model.Templating;
using BreastVMATAutomation.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace BreastVMATAutomation.Model.ESAPI
{
    public class BreastGeometryTool
    {
        public IBreastGeometryTool GeometryTool { get; set; }
        public BreastGeometryTool(ExternalPlanSetup plan, IEnumerable<Structure> selectedTargets,
    ArcModel selectedSetup, NonRDSOptions nonRDSOptions)
        {
            var body = plan.StructureSet.Structures.First(e => e.DicomType == "EXTERNAL");
            if (selectedTargets.First().CenterPoint.x > body.CenterPoint.x)
            {
                GeometryTool = new LeftBreastGeometryTool(plan, selectedTargets, selectedSetup, nonRDSOptions);

            }
            else
            {
                GeometryTool =
                            new RightBreastGeometryTool(plan, selectedTargets, selectedSetup, nonRDSOptions);
            }

        }

        internal void ChangeIsocenter()
        {
            GeometryTool.ChangeIsocenter();
        }

        internal void CreateArcsBasedOnModel()
        {
            GeometryTool.CreateArcsBasedOnModel();
        }

        internal void RemoveCreatedStructures()
        {
            GeometryTool.RemoveCreatedStructures();
        }
    }
}
