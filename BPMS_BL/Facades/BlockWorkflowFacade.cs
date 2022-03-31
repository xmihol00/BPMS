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
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class BlockWorkflowFacade : BaseFacade
    {
        private readonly BlockWorkflowRepository _blockWorkflowRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly TaskDataRepository _taskDataRepository;
        private readonly IMapper _mapper;

        public BlockWorkflowFacade(BlockWorkflowRepository blockWorkflowRepository, BlockModelRepository blockModelRepository,
                                   TaskDataRepository taskDataRepository, FilterRepository filterRepository, IMapper mapper)
        : base(filterRepository)
        {
            _blockWorkflowRepository = blockWorkflowRepository;
            _blockModelRepository = blockModelRepository;
            _taskDataRepository = taskDataRepository;
            _mapper = mapper;
        }
        
        public async Task<BlockWorkflowConfigDTO> Config(Guid blockId, Guid workflowId)
        {
            BlockWorkflowEntity entity = await _blockWorkflowRepository.BareWorkflow(blockId, workflowId);
            BlockWorkflowConfigDTO dto = _mapper.Map<BlockWorkflowConfigDTO>(entity);
            UserIdNameDTO adminWF = await _blockWorkflowRepository.WorkflowAdmin(entity.Id);

            if (dto is ITaskWorkflowConfigDTO)
            {
                ITaskWorkflowConfigDTO taskConfig = dto as ITaskWorkflowConfigDTO;
                taskConfig.UserIdNames = await _blockModelRepository.UserIdNamesRole(blockId, entity.Workflow.AgendaId);
                if (!taskConfig.UserIdNames.Any(x => x.Id == adminWF.Id))
                {
                    taskConfig.UserIdNames.Add(adminWF);
                }
            }

            if (dto is IServiceTaskWorkflowConfigDTO)
            {
                IServiceTaskWorkflowConfigDTO taskConfig = dto as IServiceTaskWorkflowConfigDTO;
                taskConfig.CurrentUserId = await _blockWorkflowRepository.ServiceTaskUserId(entity.Id);
                taskConfig.ServiceName = await _blockModelRepository.ServiceName(blockId);
                
                foreach (TaskDataEntity data in await _taskDataRepository.ServiceTaskDataOutput(entity.Id))
                {
                    (dto as IOutputData).OutputData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
                }
            }
            else if (dto is IUserTaskWorkflowConfigDTO)
            {
                IUserTaskWorkflowConfigDTO taskConfig = dto as IUserTaskWorkflowConfigDTO;
                taskConfig.CurrentUserId = await _blockWorkflowRepository.UserTaskUserId(entity.Id);

                foreach (TaskDataEntity data in await _taskDataRepository.OutputUserTasks(entity.Id))
                {
                    (dto as IOutputData).OutputData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
                }
            }
            else if (dto is IOutputData)
            {
                foreach (TaskDataEntity data in await _taskDataRepository.OutputUserTasks(entity.Id))
                {
                    (dto as IOutputData).OutputData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
                }
            }

            return dto;
        }

        public async Task EditServiceTask(ServiceTaskEditDTO dto)
        {
            IServiceTaskWorkflowEntity entity = await _blockWorkflowRepository.Bare(dto.Id) as IServiceTaskWorkflowEntity;
            entity.UserId = dto.UserId;
            entity.State = dto.State;

            await _blockWorkflowRepository.Save();
        }

        public async Task EditUserTask(UserTaskEditDTO dto)
        {
            IUserTaskWorkflowEntity entity = await _blockWorkflowRepository.Bare(dto.Id) as IUserTaskWorkflowEntity;
            entity.Priority = dto.Priority;
            entity.SolveDate = dto.SolveDate;
            entity.UserId = dto.UserId;
            entity.State = dto.State;

            await _blockWorkflowRepository.Save();
        }
    }
}
