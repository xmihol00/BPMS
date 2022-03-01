using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Model;
using BPMS_DTOs.Role;
using BPMS_DTOs.System;
using BPMS_DTOs.Workflow;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class AgendaDetailPartialDTO : AgendaDetailHeaderDTO
    {
        public Guid AdministratorId { get; set; }
        public string AdministratorName { get; set; } = string.Empty;
        public string AdministratorEmail { get; set; } = string.Empty;
        public List<ModelAllAgendaDTO> Models { get; set; } = new List<ModelAllAgendaDTO>();
        public List<RoleDetailDTO> Roles { get; set; } = new List<RoleDetailDTO>();
        public List<SystemAllDTO> Systems { get; set; } = new List<SystemAllDTO>();
        public List<WorkflowAllAgendaDTO> Workflows { get; set; } = new List<WorkflowAllAgendaDTO>();
        public List<WorkflowActiveBlocksDTO> ActiveBlocks { get; set; } = new List<WorkflowActiveBlocksDTO>();
    }
}
