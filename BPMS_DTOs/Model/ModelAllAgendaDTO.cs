using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Model
{
    public class ModelAllAgendaDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string SVG { get; set; } = string.Empty;
        public ModelStateEnum State { get; set; }
    }
}
