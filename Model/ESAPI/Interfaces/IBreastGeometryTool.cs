﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreastVMATAutomation.Model.ESAPI
{
    public interface IBreastGeometryTool
    {
        void ChangeIsocenter();
        void CreateArcsBasedOnModel();

        void RemoveCreatedStructures();
    }
}
