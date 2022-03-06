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
        public string? Title { get; set; }
        public string Email { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Password { get; set; }
        public string? PhoneNumber { get; set; }
        public List<SystemRoleEntity> SystemRoles { get; set; } = new List<SystemRoleEntity>();
        public List<AgendaEntity> Agendas { get; set; } = new List<AgendaEntity>();
        public List<UserTaskWorkflowEntity> Tasks { get; set; } = new List<UserTaskWorkflowEntity>();
        public List<ServiceTaskWorkflowEntity> Services { get; set; } = new List<ServiceTaskWorkflowEntity>();
        public List<WorkflowEntity> Workflows { get; set; } = new List<WorkflowEntity>();
        public List<UserRoleEntity> UserRoles { get; set; } = new List<UserRoleEntity>();
        public List<FilterEntity> Fitlers { get; set; } = new List<FilterEntity>();
    }
}
