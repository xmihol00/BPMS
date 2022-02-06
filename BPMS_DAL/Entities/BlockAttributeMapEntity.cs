using BPMS_Common.Enums;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Entities.WorkflowBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class BlockAttributeMapEntity
    {
        public Guid BlockId { get; set; }
        public BlockModelEntity? Block { get; set; }
        public Guid AttributeId { get; set; }
        public BlockAttributeEntity? Attribute { get; set; }
    }
}
