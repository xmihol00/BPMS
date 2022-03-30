using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Workflow;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.User
{
    public class UserMyDetailDTO
    {
        public Guid Id { get; set; }
        public string? Title { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Surname { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public List<SystemRoleEnum> Roles { get; set; } = new List<SystemRoleEnum>();
        public List<AgendaAllDTO> Agendas { get; set; } = new List<AgendaAllDTO>();
        public List<WorkflowAllDTO> Workflows { get; set; } = new List<WorkflowAllDTO>();
        public List<WorkflowActiveBlocksDTO> ActiveBlocks { get; set; } = new List<WorkflowActiveBlocksDTO>();
    }
}
