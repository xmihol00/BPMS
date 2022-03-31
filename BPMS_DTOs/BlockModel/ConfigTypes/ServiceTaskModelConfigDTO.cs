using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.Attribute;
using BPMS_DTOs.BlockModel.IConfigTypes;
using BPMS_DTOs.Role;
using BPMS_DTOs.Service;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.ConfigTypes
{
    public class ServiceTaskModelConfigDTO : BlockModelConfigDTO, IServiceConfig
    {
        public List<ServiceIdNameDTO> Services { get; set; } = new List<ServiceIdNameDTO>();
        public Guid? CurrentServiceId { get; set; }
        public Guid? CurrentRole { get; set; }
        public List<DataSchemaSourceDTO> SourceSchemas { get; set; } = new List<DataSchemaSourceDTO>();
        public List<DataSchemaMapDTO> TargetSchemas { get; set; } = new List<DataSchemaMapDTO>();
        public List<DataSchemaMappedDTO> MappedSchemas { get; set; } = new List<DataSchemaMappedDTO>();
    }
}
