using BPMS_Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class LaneEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string? Name { get; set; }
        public Guid PoolId { get; set; }

        [JsonIgnore]
        public PoolEntity? Pool { get; set; }
        public Guid? RoleId { get; set; }

        [JsonIgnore]
        public SolvingRoleEntity? Role { get; set; }

        [JsonIgnore]
        public List<BlockModelEntity> Blocks { get; set; } = new List<BlockModelEntity>();
    }
}
