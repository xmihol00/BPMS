using AutoMapper;
using BPMS_DAL.Entities;
using BPMS_DTOs.ServiceDataSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_BL.Profiles
{
    public class ServiceDataSchemaProfile : Profile
    {
        public ServiceDataSchemaProfile()
        {
            CreateMap<ServiceDataSchemaCreateEditDTO, ServiceDataSchemaEntity>()
                .ForMember(dst => dst.Alias, opt => opt.MapFrom(src => String.IsNullOrEmpty(src.Alias) ? "" : src.Alias))
                .ForMember(dst => dst.Compulsory, opt => opt.MapFrom(src => src.Compulsory != null));
        }
    }
}
