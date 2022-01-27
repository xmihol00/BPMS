﻿using BPMS_DAL.Entities.ModelBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class ConditionDataEntity
    {
        public Guid BlockId { get; set; }
        public Guid ConditionId { get; set; }
        public ExclusiveGatewayModelEntity ExclusiveGateway { get; set; } = new ExclusiveGatewayModelEntity();
        public BlockDataSchemaEntity DataSchema { get; set; } = new BlockDataSchemaEntity();
    }
}
