using BPMS_Common.Enums;
using Newtonsoft.Json;
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

        [JsonIgnore]
        public BlockModelEntity? InBlock { get; set; }
        public Guid OutBlockId { get; set; }

        [JsonIgnore]
        public BlockModelEntity? OutBlock { get; set; }
    }
}
