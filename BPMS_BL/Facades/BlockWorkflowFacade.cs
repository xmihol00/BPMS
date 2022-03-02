using AutoMapper;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Interfaces.WorkflowBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.BlockWorkflow.EditTypes;
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
        
        public async Task<BlockWorkflowConfigDTO> Config(Guid blockId, Guid workflowId)
        {
            BlockWorkflowEntity entity = await _blockWorkflowRepository.BareWorkflow(blockId, workflowId);
            BlockWorkflowConfigDTO dto = _mapper.Map<BlockWorkflowConfigDTO>(entity);

            if (dto is IServiceTaskWorkflowConfigDTO)
            {
                IServiceTaskWorkflowConfigDTO taskConfig = dto as IServiceTaskWorkflowConfigDTO;
                taskConfig.CurrentUserId = await _blockWorkflowRepository.ServiceTaskUserId(entity.Id);
                taskConfig.ServiceName = await _blockModelRepository.ServiceName(blockId);
                taskConfig.UserIdNames = await _blockModelRepository.UserIdNamesService(blockId, entity.Workflow.AgendaId);
            }

            if (dto is IUserTaskWorkflowConfigDTO)
            {
                IUserTaskWorkflowConfigDTO taskConfig = dto as IUserTaskWorkflowConfigDTO;
                taskConfig.CurrentUserId = await _blockWorkflowRepository.UserTaskUserId(entity.Id);
                taskConfig.UserIdNames = await _blockModelRepository.UserIdNamesUser(blockId, entity.Workflow.AgendaId);
            }

            return dto;
        }

        public async Task EditServiceTask(ServiceTaskEditDTO dto)
        {
            IServiceTaskWorkflowEntity entity = await _blockWorkflowRepository.Bare(dto.Id) as IServiceTaskWorkflowEntity;
            entity.UserId = dto.UserId;

            await _blockWorkflowRepository.Save();
        }

        public async Task EditUserTask(UserTaskEditDTO dto)
        {
            IUserTaskWorkflowEntity entity = await _blockWorkflowRepository.Bare(dto.Id) as IUserTaskWorkflowEntity;
            entity.Priority = dto.Priority;
            entity.SolveDate = dto.SolveDate;
            entity.UserId = dto.UserId;

            await _blockWorkflowRepository.Save();
        }
    }
}
