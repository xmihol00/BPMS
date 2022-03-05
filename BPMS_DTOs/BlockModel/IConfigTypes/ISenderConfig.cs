using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Attribute;
using BPMS_DTOs.Service;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.IConfigTypes
{
    public interface ISenderConfig
    {
        List<SenderRecieverConfigDTO> Recievers { get; set; }
    }
}
