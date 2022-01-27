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
        public List<BlockDataSchemaEntity> Data { get; set; } = new List<BlockDataSchemaEntity>();
    }
}
