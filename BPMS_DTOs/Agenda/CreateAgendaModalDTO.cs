using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class CreateAgendaModalDTO
    {
        public int AgendaId { get; set; }
        public IFormFile? BPMN { get; set; }
        public IFormFile? SVG { get; set; }
    }
}
