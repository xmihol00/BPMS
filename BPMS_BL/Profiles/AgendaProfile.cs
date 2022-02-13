using AutoMapper;
using BPMS_DAL.Entities;
using BPMS_DTOs.Agenda;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_BL.Profiles
{
    public class AgendaProfile : Profile
    {
        public AgendaProfile()
        {
            CreateMap<AgendaCreateDTO, AgendaEntity>();

            CreateMap<AgendaEntity, AgendaDetailPartialDTO>()
                .ForMember(dst => dst.AdministratorName, opt => opt.MapFrom(src => $"{src.Administrator.Name} {src.Administrator.Surname}"))
                .ForMember(dst => dst.AdministratorEmail, opt => opt.MapFrom(src => src.Administrator.Email));
        }
    }
}
