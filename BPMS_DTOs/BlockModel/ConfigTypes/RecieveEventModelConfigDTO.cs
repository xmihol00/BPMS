using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.BlockModel.IConfigTypes;
using BPMS_DTOs.ServiceDataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.ConfigTypes
{
    public class RecieveEventModelConfigDTO : BlockModelConfigDTO, IRecievedMessageConfig, IAttributesConfig
    {
        public RecievedMessageDTO Message { get; set; } = new RecievedMessageDTO();
        public List<BlockAttributeDTO> Attributes { get; set; } = new List<BlockAttributeDTO>();
    }
}
