using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;

namespace BPMS_DAL.Interfaces.ModelBlocks
{
    public interface IServiceTaskModelEntity : IBlockModelEntity
    {
        public Guid? ServiceId { get; set; }
        public ServiceEntity? Service { get; set; }
        public Guid? RoleId { get; set; }
        public SolvingRoleEntity? Role { get; set; }
        public ServiceStateEnum State { get; set; }
        public List<DataSchemaMapEntity> MappedSchemas { get; set; }
    }
}
