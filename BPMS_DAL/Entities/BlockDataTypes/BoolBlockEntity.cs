﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.BlockDataTypes
{
    public class BoolBlockEntity : TaskDataEntity
    {
        public bool? Value { get; set; }
    }
}
