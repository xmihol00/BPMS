using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_BL.Hubs;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.BlockWorkflow.IConfigTypes;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Task;
using BPMS_DTOs.Workflow;

namespace BPMS_BL.Facades
{
    public class WorkflowFacade : BaseFacade
    {
        private readonly WorkflowRepository _workflowRepository;
        private readonly BlockWorkflowRepository _blockWorkflowRepository;
        private readonly UserRepository _userRepository;
        private readonly NotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public WorkflowFacade(WorkflowRepository workflowRepository, BlockWorkflowRepository blockWorkflowRepository, UserRepository userRepository,
                              NotificationRepository notificationRepository, FilterRepository filterRepository, IMapper mapper)
        : base(filterRepository)
        {
            _workflowRepository = workflowRepository;
            _blockWorkflowRepository = blockWorkflowRepository;
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public void SetFilters(bool[] filters)
        {
            _workflowRepository.Filters = filters;
            _workflowRepository.UserId = UserId;
            _userRepository.UserId = UserId;
        }

        public async Task<WorkflowOverviewDTO> Overview()
        {
            WorkflowOverviewDTO overview = new WorkflowOverviewDTO()
            {
                Workflows = await _workflowRepository.All(),
            };

            if (_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowActive)] ||
                (!_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowActive)] &&
                 !_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowPaused)] &&
                 !_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowFinished)] &&
                 !_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowCanceled)]))
            {
                overview.ActiveBlocks = await _workflowRepository.ActiveBlocks();
            }

            return overview;
        }

        public async Task<WorkflowDetailDTO> DetailPartial(Guid id)
        {
            WorkflowDetailDTO detail = await _workflowRepository.Detail(id);
            detail.ActiveBlock = await _workflowRepository.ActiveBlocks(id);

            List<TaskDataDTO> outputData = new List<TaskDataDTO>();
            foreach (TaskDataEntity data in await _workflowRepository.OutputUserTasks(id))
            {
                outputData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
            }
            detail.OutputData = outputData.GroupBy(x => x.BlockName);

            List<TaskDataDTO> inputServiceData = new List<TaskDataDTO>();
            List<TaskDataDTO> outputServiceData = new List<TaskDataDTO>();
            foreach (TaskDataEntity data in await _workflowRepository.MappedServiceTasks(id))
            {
                if (data.Schema.Direction == DirectionEnum.Input)
                {
                    inputServiceData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
                }
                else
                {
                    outputServiceData.Add(_mapper.Map(data, data.GetType(), typeof(TaskDataDTO)) as TaskDataDTO);
                }
            }
            detail.InputServiceData = inputServiceData.GroupBy(x => x.BlockName);
            detail.OutputServiceData = outputServiceData.GroupBy(x => x.BlockName);
            detail.Editable = await _workflowRepository.IsKeeper(id);

            return detail;
        }

        public async Task<WorkflowAdminChangeDTO> Keepers(Guid id)
        {
            return new WorkflowAdminChangeDTO
            {
                CurrentAdminId = await _workflowRepository.Keeper(id),
                OtherAdmins = await _userRepository.Keepers(SystemRoleEnum.WorkflowKeeper)
            };
        }

        public async Task<WorkflowOverviewDTO> Filter(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, UserId);
            _workflowRepository.Filters[((int)dto.Filter)] = !dto.Removed;
            WorkflowOverviewDTO overview = new WorkflowOverviewDTO()
            {
                Workflows = await _workflowRepository.All(),
            };

            if (_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowActive)] ||
                (!_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowActive)] &&
                 !_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowPaused)] &&
                 !_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowFinished)] &&
                 !_workflowRepository.Filters[((int)FilterTypeEnum.WorkflowCanceled)]))
            {
                overview.ActiveBlocks = await _workflowRepository.ActiveBlocks();
            }

            return overview;
        }

        public async Task<WorkflowInfoCardDTO> Edit(WorkflowEditDTO dto)
        {
            WorkflowEntity workflow = await _workflowRepository.Bare(dto.Id);
            if (workflow.State == WorkflowStateEnum.Active && dto.State != WorkflowStateEnum.Active)
            {
                BlockWorkflowStateEnum state = dto.State == WorkflowStateEnum.Paused ? BlockWorkflowStateEnum.Paused : BlockWorkflowStateEnum.Canceled;
                foreach (BlockWorkflowEntity block in await _blockWorkflowRepository.AllOfState(dto.Id, BlockWorkflowStateEnum.Active))
                {
                    block.State = state;
                }
            }
            else if (workflow.State == WorkflowStateEnum.Paused && dto.State != WorkflowStateEnum.Paused)
            {
                BlockWorkflowStateEnum state = dto.State == WorkflowStateEnum.Active ? BlockWorkflowStateEnum.Active : BlockWorkflowStateEnum.Canceled;
                foreach (BlockWorkflowEntity block in await _blockWorkflowRepository.AllOfState(dto.Id, BlockWorkflowStateEnum.Paused))
                {
                    block.State = state;
                }
            }

            if (dto.AdministratorId != null && dto.AdministratorId != workflow.AdministratorId)
            {
                workflow.AdministratorId = dto.AdministratorId;
                await NotificationHub.CreateSendNotifications(_notificationRepository, dto.Id, NotificationTypeEnum.NewWorkflow, workflow.Name, 
                                                              UserId, workflow.AdministratorId.Value);
            }

            workflow.Description = dto.Description;
            workflow.Name = dto.Name;
            workflow.State = dto.State;
            workflow.ExpectedEnd = dto.ExpectedEnd;
            await _workflowRepository.Save();

            return await _workflowRepository.InfoCard(workflow.Id);
        }

        public async Task<WorkflowDetailDTO> Detail(Guid id)
        {
            WorkflowDetailDTO detail = await DetailPartial(id);
            detail.OtherWorkflows = await _workflowRepository.All(id);
            detail.SelectedWorkflow = await _workflowRepository.Selected(id);
            detail.ActiveBlocks = await _workflowRepository.ActiveBlocks();
            
            return detail;
        }
    }
}
