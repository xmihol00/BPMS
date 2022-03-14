using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class AgendaEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid AdministratorId { get; set; }
        public UserEntity? Administrator { get; set; }
        public List<AgendaRoleEntity> AgendaRoles { get; set; } = new List<AgendaRoleEntity>();
        public List<SystemAgendaEntity> Systems { get; set; } = new List<SystemAgendaEntity>();
        public List<ModelEntity> Models { get; set; } = new List<ModelEntity>();
        public List<WorkflowEntity> Workflows { get; set; } = new List<WorkflowEntity>();
    }
}
