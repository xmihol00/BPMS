using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities.ModelBlocks
{
    public class ExclusiveGatewayModelEntity : BlockModelEntity
    {
        public string Condition { get; set; } = string.Empty;
        public List<ConditionDataEntity> Conditions { get; set; } = new List<ConditionDataEntity>();
    }
}
