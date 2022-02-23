using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.Role;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.ConfigTypes
{
    public class UserTaskConfigDTO : BlockModelConfigDTO, IAttributesConfig, IInputAttributesConfig, IServiceInputAttributes, IServiceOutputAttributes, IRoleConfig
    {
        public List<BlockAttributeDTO> Attributes { get; set; } = new List<BlockAttributeDTO>();
        public List<IGrouping<string, InputBlockAttributeDTO>> InputAttributes { get; set; } = new List<IGrouping<string, InputBlockAttributeDTO>>();
        public List<ServiceTaskDataSchemaDTO>? ServiceOutputAttributes { get; set; }
        public List<ServiceTaskDataSchemaDTO>? ServiceInputAttributes { get; set; }
        public List<RoleAllDTO> Roles { get; set; } = new List<RoleAllDTO>();
        public Guid? CurrentRole { get; set; }
    }
}
