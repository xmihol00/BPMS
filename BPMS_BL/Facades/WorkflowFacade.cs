using AutoMapper;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Task;
using BPMS_DTOs.Workflow;

namespace BPMS_BL.Facades
{
    public class WorkflowFacade
    {
        private readonly WorkflowRepository _workflowRepository;
        private readonly IMapper _mapper;

        public WorkflowFacade(WorkflowRepository workflowRepository, IMapper mapper)
        {
            _workflowRepository = workflowRepository;
            _mapper = mapper;
        }

        public async Task<WorkflowOverviewDTO> Overview()
        {
            return new WorkflowOverviewDTO()
            {
                Workflows = await _workflowRepository.All(),
                ActiveBlocks = await _workflowRepository.ActiveBlocks()
            };
        }

        public async Task<WorkflowDetailDTO> DetailPartial(Guid id)
        {
            WorkflowDetailDTO detail = await _workflowRepository.Detail(id);
            detail.ActiveBlocks = await _workflowRepository.ActiveBlocks(id);

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

            return detail;
        }

        public async Task<WorkflowInfoCardDTO> Edit(WorkflowEditDTO dto)
        {
            WorkflowEntity workflow = await _workflowRepository.BareAdmin(dto.Id);
            workflow.Description = dto.Description ?? "";
            workflow.Name = dto.Name;
            workflow.State = dto.State;
            await _workflowRepository.Save();

            return new WorkflowInfoCardDTO()
            {
                AdministratorEmail = workflow.Administrator.Email,
                AdministratorName = $"{workflow.Administrator.Name} {workflow.Administrator.Surname}",
                Description = workflow.Description,
                Id = workflow.Id,
                Name = workflow.Name,
                State = workflow.State,
                SelectedWorkflow = await _workflowRepository.Selected(dto.Id)
            };
        }

        public async Task<WorkflowDetailDTO> Detail(Guid id)
        {
            WorkflowDetailDTO detail = await DetailPartial(id);
            detail.OtherWorkflows = await _workflowRepository.All(id);
            detail.SelectedWorkflow = await _workflowRepository.Selected(id);
            
            return detail;
        }
    }
}
