using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.System
{
    public class SystemIdAgendaIdDTO
    {
        public Guid? SystemId { get; set; }
        public Guid? AgendaId { get; set; }
    }
}
