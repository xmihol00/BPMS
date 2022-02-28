using BPMS_DAL.Repositories;
using BPMS_DTOs.Workflow;

namespace BPMS_BL.Facades
{
    public class WorkflowFacade
    {
        private readonly WorkflowRepository _workflowRepository;

        public WorkflowFacade(WorkflowRepository workflowRepository)
        {
            _workflowRepository = workflowRepository;
        }

        public async Task<WorkflowOverviewDTO> Overview()
        {
            return new WorkflowOverviewDTO()
            {
                Workflows = await _workflowRepository.Overview()
            };
        }
    }
}
