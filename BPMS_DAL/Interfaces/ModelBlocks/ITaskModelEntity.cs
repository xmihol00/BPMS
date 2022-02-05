using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Interfaces.ModelBlocks
{
    public interface ITaskModelEntity : IBlockModelEntity
    {
        public Guid? RoleId { get; set; }
        public SolvingRoleEntity? Role { get; set; }
        public TimeSpan Span { get; set; }
        public List<BlockAttributeEntity> Attributes { get; set; }
    }
}
