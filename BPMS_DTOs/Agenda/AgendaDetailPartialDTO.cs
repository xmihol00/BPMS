using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Model;
using BPMS_DTOs.Role;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class AgendaDetailPartialDTO : AgendaDetailHeaderDTO
    {
        public Guid AdministratorId { get; set; }
        public string AdministratorName { get; set; } = string.Empty;
        public string AdministratorEmail { get; set; } = string.Empty;
        public List<ModelAllDTO> Models { get; set; } = new List<ModelAllDTO>();
        public List<RoleDetailDTO> Roles { get; set; } = new List<RoleDetailDTO>();
    }
}
