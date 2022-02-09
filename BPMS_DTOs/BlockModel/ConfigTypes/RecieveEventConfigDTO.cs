using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.ServiceDataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.ConfigTypes
{
    public class RecieveEventConfigDTO : BlockModelConfigDTO, IRecievedMessageConfig
    {
        public RecievedMessageDTO Message { get; set; } = new RecievedMessageDTO();
    }
}
