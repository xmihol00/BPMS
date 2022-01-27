using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class BlockDataEntity
    {
        public Guid BlockId { get; set; }
        public BlockWorkflowEntity Block { get; set; } = new BlockWorkflowEntity();
        public Guid SchemaId { get; set; }
        public BlockDataSchemaEntity Schema { get; set; } = new BlockDataSchemaEntity();
    }
}
