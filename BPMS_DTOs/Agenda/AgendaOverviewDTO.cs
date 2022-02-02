using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class AgendaOverviewDTO
    {
        public List<AgendaAllDTO> Agendas { get; set; } = new List<AgendaAllDTO>();
    }
}
