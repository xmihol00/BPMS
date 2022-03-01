using BPMS_BL.Facades;
using BPMS_DTOs.Header;
using BPMS_DTOs.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class WorkflowController : BaseController
    {
        private readonly WorkflowFacade _workflowFacade;

        public WorkflowController(WorkflowFacade workflowFacade)
        {
            _workflowFacade = workflowFacade;
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("WorkflowOverview", await _workflowFacade.Overview());
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("WorkflowDetail", await _workflowFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            WorkflowDetailPartialDTO dto = await _workflowFacade.DetailPartial(id);
            return Ok(new
            {
                detail = await this.RenderViewAsync("Partial/_WorkflowDetail", dto, true),
                header = await this.RenderViewAsync("Partial/_WorkflowDetailHeader", dto, true),
                activeBlocks = dto.ActiveBlocks
            });
        }
    }
}
