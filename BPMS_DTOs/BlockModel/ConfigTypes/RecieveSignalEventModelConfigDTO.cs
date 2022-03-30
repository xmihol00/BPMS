using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Attribute;
using BPMS_DTOs.BlockModel.IConfigTypes;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.ConfigTypes
{
    public class RecieveSignalEventModelConfigDTO : BlockModelConfigDTO, IOutputAttributesConfig, IRecieveSignalEventModelConfig
    {
        public List<AttributeDTO> OutputAttributes { get; set; } = new List<AttributeDTO>();
        public SenderRecieverConfigDTO? Sender { get; set; }
    }
}
