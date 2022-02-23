using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.ConfigTypes
{
    public interface IServiceInputAttributes
    {
        public List<ServiceTaskDataSchemaDTO>? ServiceInputAttributes { get; set; }
    }
}
