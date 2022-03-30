using AutoMapper;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.BlockWorkflow.ConfigTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_BL.Profiles
{
    public class BlockWorkflowProfile : Profile
    {
        public BlockWorkflowProfile()
        {
            CreateMap<BlockWorkflowEntity, BlockWorkflowConfigDTO>()
                .ForMember(dst => dst.Name, opt => opt.MapFrom(src => src.BlockModel.Name))
                .ForMember(dst => dst.Description, opt => opt.MapFrom(src => src.BlockModel.Description))
            .Include<UserTaskWorkflowEntity, UserTaskWorkflowConfigDTO>()
            .Include<ServiceTaskWorkflowEntity, ServiceTaskWorkflowConfigDTO>()
            .Include<RecieveMessageEventWorkflowEntity, RecieveEventWorkflowConfigDTO>();

            CreateMap<UserTaskWorkflowEntity, UserTaskWorkflowConfigDTO>();            
            CreateMap<ServiceTaskWorkflowEntity, ServiceTaskWorkflowConfigDTO>();
            CreateMap<RecieveMessageEventWorkflowEntity, RecieveEventWorkflowConfigDTO>();
        }
    }
}
