using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Agenda
{
    public class AgendaAdminChangeDTO
    {
        public Guid CurrentAdminId { get; set; }
        public List<UserIdNameDTO> OtherAdmins { get; set; } = new List<UserIdNameDTO>();
    }
}
