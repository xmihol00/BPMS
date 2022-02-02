using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class AgendaCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public Guid AdministratorId { get; set; }
    }
}
