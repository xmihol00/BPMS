using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BPMS_DTOs.BlockAttribute;
using BPMS_DTOs.BlockModel.IConfigTypes;
using BPMS_DTOs.Role;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using Microsoft.AspNetCore.Http;

namespace BPMS_DTOs.BlockModel.ConfigTypes
{
    public class ServiceTaskModelConfigDTO : BlockModelConfigDTO, IServiceConfig, IRoleConfig
    {
        public List<ServiceIdNameDTO> Services { get; set; } = new List<ServiceIdNameDTO>();
        public Guid? CurrentService { get; set; }
        public List<RoleAllDTO> Roles { get; set; } = new List<RoleAllDTO>();
        public Guid? CurrentRole { get; set; }
    }
}
