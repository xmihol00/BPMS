using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_Common.Enums;
using BPMS_DTOs.Header;
using BPMS_DTOs.ServiceDataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.Service
{
    public class ServiceInfoCardDTO : ServiceDetailHeaderDTO
    {
        public ServiceAllDTO SelectedService { get; set; } = new ServiceAllDTO();
    }
}
