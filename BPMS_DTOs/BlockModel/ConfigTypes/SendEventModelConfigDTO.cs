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
    public class SendEventModelConfigDTO : BlockModelConfigDTO, IInputAttributesConfig, ISenderConfig
    {
        public List<IGrouping<string, InputAttributeDTO>> InputAttributes { get; set; } = new List<IGrouping<string, InputAttributeDTO>>();
        public List<SenderRecieverConfigDTO> Recievers { get; set; } = new List<SenderRecieverConfigDTO>();
    }
}
