using AutoMapper;
using BPMS_DAL.Entities;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Pool;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_BL.Profiles
{
    public class CommunicationProfile : Profile
    {
        public CommunicationProfile()
        {
            CreateMap<ModelDetailShare, ModelEntity>()
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => String.IsNullOrEmpty(src.Description) ? "" : src.Description));

            CreateMap<PoolShareDTO, PoolEntity>();
        }
    }
}
