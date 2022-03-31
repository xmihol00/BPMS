using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Attribute;
using BPMS_DTOs.BlockModel.IConfigTypes;
using BPMS_DTOs.Role;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.ConfigTypes
{
    public class UserTaskModelConfigDTO : BlockModelConfigDTO, IOutputAttributesConfig, IInputAttributesConfig, IServiceInputAttributes, IServiceOutputAttributes, IDifficultyConfig
    {
        public List<AttributeDTO> OutputAttributes { get; set; } = new List<AttributeDTO>();
        public List<IGrouping<string, InputAttributeDTO>> InputAttributes { get; set; } = new List<IGrouping<string, InputAttributeDTO>>();
        public List<ServiceTaskDataSchemaDTO>? ServiceOutputAttributes { get; set; }
        public List<ServiceTaskDataSchemaDTO>? ServiceInputAttributes { get; set; }
        public Guid? CurrentRole { get; set; }
        public int Difficulty { get; set; }
    }
}
