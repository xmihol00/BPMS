using AutoMapper;
using BPMS_DAL.Entities;
using BPMS_DTOs.Header;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.System;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_BL.Profiles
{
    public class SystemProfile : Profile
    {
        public SystemProfile()
        {
            CreateMap<SystemEditDTO, SystemEntity>();
        }
    }
}
