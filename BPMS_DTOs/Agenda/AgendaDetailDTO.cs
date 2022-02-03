using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class AgendaDetailDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid AdministratorId { get; set; }
        public string AdministratorName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<AllModelDTO> Models { get; set; } = new List<AllModelDTO>();
    }
}
