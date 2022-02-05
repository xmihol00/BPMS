using BPMS_DAL.Entities.WorkflowBlocks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL.Entities
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public List<SystemRoleEntity> Roles { get; set; } = new List<SystemRoleEntity>();
        public List<AgendaEntity> Agendas { get; set; } = new List<AgendaEntity>();
        public List<TaskWorkflowEntity> Tasks { get; set; } = new List<TaskWorkflowEntity>();
        public List<AgendaRoleUserEntity> UserRoles { get; set; } = new List<AgendaRoleUserEntity>();
    }
}
