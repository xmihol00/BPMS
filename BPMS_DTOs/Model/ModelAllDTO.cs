using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelAllDTO : ModelAllAgendaDTO
    {
        public Guid? AgendaId { get; set; }
        public string AgendaName { get; set; } = string.Empty;
        public ModelStateEnum State { get; set; }
    }
}
