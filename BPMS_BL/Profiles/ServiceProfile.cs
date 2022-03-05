using AutoMapper;
using BPMS_DAL.Entities;
using BPMS_DTOs.Header;
using BPMS_DTOs.Service;
using BPMS_DTOs.DataSchema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_BL.Profiles
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<DataSchemaCreateEditDTO, DataSchemaEntity>()
                .ForMember(dst => dst.Alias, opt => opt.MapFrom(src => src.Alias == null ? "" : src.Alias))
                .ForMember(dst => dst.Compulsory, opt => opt.MapFrom(src => src.Compulsory != null))
                .ForMember(dst => dst.StaticData, opt => opt.MapFrom(src => String.IsNullOrEmpty(src.DataToggle) ? null : src.StaticData));
            
            CreateMap<DataSchemaCreateEditDTO, DataSchemaNodeDTO>();

            CreateMap<ServiceCreateEditDTO, ServiceEntity>()
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Description == null ? "" : src.Description))
                .ForMember(dst => dst.URL, opt => opt.MapFrom(src => src.URL.ToString()));

            CreateMap<HeaderCreateEditDTO, ServiceHeaderEntity>();
            CreateMap<ServiceEntity, ServiceInfoCardDTO>();
        }
    }
}
