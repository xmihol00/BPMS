using BPMS_Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class FlowEntity
    {
        public Guid InBlockId { get; set; }
        public BlockModelEntity InBlock { get; set; } = new BlockModelEntity();
        public Guid OutBlockId { get; set; }
        public BlockModelEntity OutBlock { get; set; } = new BlockModelEntity();
        public FlowTypeEnum Type { get; set; }
    }
}
