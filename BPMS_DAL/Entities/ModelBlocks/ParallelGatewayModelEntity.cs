﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class ParallelGatewayModelEntity : BlockModelEntity, IParallelGatewayModelEntity
    {
        public ParallelGatewayModelEntity() : base() {}
        public ParallelGatewayModelEntity(PoolEntity pool) : base(pool) { }
    }
}
