using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class AgendaTargetDTO
    {
        public Guid Id { get; set; }
        public Guid AdministratorId { get; set; }
    }
}
