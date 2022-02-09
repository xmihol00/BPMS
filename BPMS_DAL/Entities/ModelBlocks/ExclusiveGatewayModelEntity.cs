using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Interfaces;
using BPMS_DAL.Interfaces.ModelBlocks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class ExclusiveGatewayModelEntity : BlockModelEntity, IExclusiveGatewayModelEntity
    {
        public ExclusiveGatewayModelEntity() : base() {}
        public ExclusiveGatewayModelEntity(PoolEntity pool) : base(pool) { }

        public string Condition { get; set; } = string.Empty;
        public List<ConditionDataEntity> Conditions { get; set; } = new List<ConditionDataEntity>();
    }
}
