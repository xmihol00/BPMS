using AutoMapper;
using BPMS_DAL.Entities;
using BPMS_DTOs.BlockAttribute;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_BL.Profiles
{
    public class BlockAttributeProfile : Profile
    {
        public BlockAttributeProfile()
        {
            CreateMap<AttributeCreateEditDTO, TaskAttributeEntity>()
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => String.IsNullOrEmpty(src.Description) ? "" : src.Description))
                .ForMember(dst => dst.Specification, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Specification)))
                .ForMember(dst => dst.Compulsory, opt => opt.MapFrom(src => src.Compulsory != null));
        }
    }
}
