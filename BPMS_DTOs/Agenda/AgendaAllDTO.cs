using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class AgendaAllDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int UserCount { get; set; }
        public int MissingRolesCount { get; set; }
        public int ModelsCount { get; set; }
        public int ActiveWorkflowsCount { get; set; }
        public int PausedWorkflowsCount { get; set; }
        public int FinishedWorkflowsCount { get; set; }
        public int CanceledWorkflowsCount { get; set; }
        public int SystemsCount { get; set; }
    }
}
