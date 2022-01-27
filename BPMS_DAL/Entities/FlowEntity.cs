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
        public Guid InId { get; set; }
        public Guid OutId { get; set; }
        public FlowTypeEnum Type { get; set; }
    }
}
