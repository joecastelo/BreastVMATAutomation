using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VMS.TPS.Common.Model.API;

namespace BreastVMATAutomation.Model.ESAPI
{
    public class NonRDSOptions
    {
        public NonRDSOptions(bool xLessThan15, bool includeNodes,  bool avoidEntryCLBreast, Structure breastContralateral)
        {
            XLessThan15 = xLessThan15;
            IncludeNodes = includeNodes;
            BreastContralateral = breastContralateral;
            AvoidEntryCLBreast = avoidEntryCLBreast;
        }

        public bool XLessThan15 { get; set; }
        public bool IncludeNodes { get; set; }
        public Structure BreastContralateral { get; set; }
        public bool AvoidEntryCLBreast { get; set; }



    }
}
