using AutoMapper;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DTOs.Header;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.Task;
using BPMS_DTOs.Task.DataTypes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_BL.Profiles
{
    public class TaskDataProfile : Profile
    {
        public TaskDataProfile()
        {
            CreateMap<TaskDataEntity, TaskDataDTO>()
                .ForMember(dst => dst.Compulsory, opt => opt.MapFrom(src => src.Attribute.Compulsory))
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.Attribute.Name))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.Attribute.Description))
                .IncludeAllDerived();

            CreateMap<StringDataEntity, TaskStringDTO>();
            CreateMap<BoolDataEntity, TaskBoolDTO>();
            CreateMap<NumberDataEntity, TaskNumberDTO>();
            CreateMap<SelectDataEntity, TaskSelectDTO>()
                .ForMember(dst => dst.Options, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<string>>(src.Attribute.Specification)));
            CreateMap<FileDataEntity, TaskFileDTO>()
                .ForMember(dst => dst.FileFormats, opt => opt.MapFrom(src => JsonConvert.DeserializeObject<List<string>>(src.Attribute.Specification)));
            CreateMap<TextDataEntity, TaskTextDTO>();
            CreateMap<DateDataEntity, TaskDateDTO>();
        }
    }
}
