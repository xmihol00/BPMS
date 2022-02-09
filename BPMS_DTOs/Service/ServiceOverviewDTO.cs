using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Service
{
    public class ServiceOverviewDTO
    {
        public List<ServiceAllDTO> Services { get; set; } = new List<ServiceAllDTO>();
    }
}
