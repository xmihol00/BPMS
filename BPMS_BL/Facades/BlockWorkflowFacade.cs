using AutoMapper;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.BlockWorkflow.IConfigTypes;
using BPMS_DTOs.Task;

namespace BPMS_BL.Facades
{
    public class BlockWorkflowFacade
    {
        private readonly BlockWorkflowRepository _blockWorkflowRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly IMapper _mapper;

        public BlockWorkflowFacade(BlockWorkflowRepository blockWorkflowRepository, BlockModelRepository blockModelRepository,
                                   IMapper mapper)
        {
            _blockWorkflowRepository = blockWorkflowRepository;
            _blockModelRepository = blockModelRepository;
            _mapper = mapper;
        }
        
        public async Task<BlockWorkflowConfigDTO> Config(Guid id)
        {
            BlockWorkflowEntity entity = await _blockWorkflowRepository.BareWorkflow(id);
            BlockWorkflowConfigDTO dto = _mapper.Map<BlockWorkflowConfigDTO>(entity);

            if (dto is IServiceTaskWorkflowConfigDTO)
            {
                IServiceTaskWorkflowConfigDTO taskConfig = dto as IServiceTaskWorkflowConfigDTO;
                taskConfig.CurrentUserId = await _blockWorkflowRepository.ServiceTaskUserId(id);
                taskConfig.ServiceName = await _blockModelRepository.ServiceName(entity.BlockModelId);
                taskConfig.UserIdNames = await _blockModelRepository.UserIdNamesService(entity.BlockModelId, entity.Workflow.AgendaId);
            }

            if (dto is IUserTaskWorkflowConfigDTO)
            {
                IUserTaskWorkflowConfigDTO taskConfig = dto as IUserTaskWorkflowConfigDTO;
                taskConfig.CurrentUserId = await _blockWorkflowRepository.UserTaskUserId(id);
                taskConfig.UserIdNames = await _blockModelRepository.UserIdNamesUser(entity.BlockModelId, entity.Workflow.AgendaId);
            }

            if (dto is ITaskWorkflowConfigDTO)
            {
                ITaskWorkflowConfigDTO taskConfig = dto as ITaskWorkflowConfigDTO;
            }

            return dto;
        }
    }
}
