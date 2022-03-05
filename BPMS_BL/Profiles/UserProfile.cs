using AutoMapper;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DTOs.Header;
using BPMS_DTOs.Service;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_BL.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserCreateEditDTO, UserEntity>()
                .ForMember(dst => dst.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber == null ? "" : src.PhoneNumber));
            
            CreateMap<UserEntity, UserAuthDTO>()
                .ForMember(dst => dst.Roles, opt => opt.MapFrom(src => src.SystemRoles.Select(x => x.Role).ToList()));
        }
    }
}
