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
    public class SendEventConfigDTO : BlockModelConfigDTO, IInputAttributesConfig
    {
        public List<IGrouping<string, InputBlockAttributeDTO>> InputAttributes { get; set; } = new List<IGrouping<string, InputBlockAttributeDTO>>();
    }
}