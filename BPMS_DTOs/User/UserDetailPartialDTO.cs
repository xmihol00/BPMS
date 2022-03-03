using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Task;
using BPMS_DTOs.Workflow;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.User
{
    public class UserDetailPartialDTO : UserDetailHeaderDTO
    {
        public List<TaskAllDTO> Tasks { get; set; } = new List<TaskAllDTO>();
        public List<AgendaAllDTO> Agendas { get; set; } = new List<AgendaAllDTO>();
        public List<WorkflowAllDTO> Workflows { get; set; } = new List<WorkflowAllDTO>();
        public List<WorkflowActiveBlocksDTO> ActiveBlocks { get; set; } = new List<WorkflowActiveBlocksDTO>();
    }
}
