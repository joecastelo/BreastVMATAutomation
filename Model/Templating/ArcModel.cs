using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreastVMATAutomation.Model
{
    public class ArcModel
    {
        public static ArcModel GetDefaultValues()
        {
            var arcmodel = new ArcModel {
                GapSectorAngle = 40,
                NbPartialArcs = 4,
                NbIsoCenters = 1,
                InnerAngleMargin = 10,
                OuterAngleMargin = 50,

            };
            arcmodel.ResolveId();
            return arcmodel;
        }
        public void ResolveId()
        {
            Id = $"{NbPartialArcs} Arcs/ {NbIsoCenters} Isos/ {GapSectorAngle} deg Avoidance/ {InnerAngleMargin} in / {OuterAngleMargin} out";
        }
        public string Id { get; set; }
        public int GapSectorAngle { get; set; }

        public int NbPartialArcs { get; set; }

        public int NbIsoCenters { get; set; }

        public int InnerAngleMargin { get; set; }

        public int OuterAngleMargin { get; set; }


    }
}
