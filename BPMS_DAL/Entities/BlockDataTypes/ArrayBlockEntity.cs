﻿using BPMS_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.BlockDataTypes
{
    public class ArrayBlockEntity : BlockDataEntity
    {
        public BlockTypeEnum Type { get; set; }
    }
}
