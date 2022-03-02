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
    public class AgendaInfoCardDTO : AgendaDetailInfoDTO
    {
        public AgendaAllDTO SelectedAgenda { get; set; } = new AgendaAllDTO();
    }
}
