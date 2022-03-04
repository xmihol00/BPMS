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
    public class RecieveEventModelConfigDTO : BlockModelConfigDTO, IOutputAttributesConfig, IRecieverConfig
    {
        public List<BlockAttributeDTO> OutputAttributes { get; set; } = new List<BlockAttributeDTO>();
        public SenderRecieverConfigDTO? Sender { get; set; }
    }
}
