using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Model;
using BPMS_DTOs.Workflow;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class AgendaDetailDTO : AgendaDetailPartialDTO
    {
        public List<AgendaAllDTO> AllAgendas { get; set; } = new List<AgendaAllDTO>();
        public AgendaAllDTO SelectedAgenda { get; set; } = new AgendaAllDTO();
    }
}
