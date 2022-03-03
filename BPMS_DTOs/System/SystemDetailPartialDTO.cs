using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.System
{
    public class SystemDetailPartialDTO : SystemDetailInfoDTO
    {
        public List<ModelAllDTO> Models { get; set; } = new List<ModelAllDTO>();
        public List<AgendaAllDTO> Agendas { get; set; } = new List<AgendaAllDTO>();
    }
}
