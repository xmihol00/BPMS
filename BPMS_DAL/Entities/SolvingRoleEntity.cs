using BPMS_DAL.Entities.ModelBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class SolvingRoleEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<AgendaRoleEntity> AgendaRoles { get; set; } = new List<AgendaRoleEntity>();
        public List<LaneEntity> Lanes { get; set; } = new List<LaneEntity>();
    }
}
