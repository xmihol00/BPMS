using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockAttribute
{
    public class RecievedMessageDTO
    {
        public bool Editable { get; set; }
        public Guid? CurrentSystemId { get; set; }
        public List<SystemPickerDTO> Systems { get; set; } = new List<SystemPickerDTO>();
    }
}
