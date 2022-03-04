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
    public class SendEventModelConfigDTO : BlockModelConfigDTO, IInputAttributesConfig, ISenderConfig
    {
        public List<IGrouping<string, InputBlockAttributeDTO>> InputAttributes { get; set; } = new List<IGrouping<string, InputBlockAttributeDTO>>();
        public List<SenderRecieverConfigDTO> Senders { get; set; } = new List<SenderRecieverConfigDTO>();
    }
}
