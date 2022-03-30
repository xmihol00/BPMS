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
    public class SendMessageEventModelConfigDTO : BlockModelConfigDTO, IInputAttributesConfig, ISendMessageEventModelConfig
    {
        public List<IGrouping<string, InputAttributeDTO>> InputAttributes { get; set; } = new List<IGrouping<string, InputAttributeDTO>>();
        public SenderRecieverConfigDTO Reciever { get; set; } = new SenderRecieverConfigDTO();
    }
}
