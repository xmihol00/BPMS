using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.System
{
    public class SystemOverviewDTO
    {
        public List<SystemAllDTO> Systems { get; set; } = new List<SystemAllDTO>();
    }
}
