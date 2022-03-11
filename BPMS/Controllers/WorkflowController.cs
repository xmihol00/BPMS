using BPMS_BL.Facades;
using BPMS_BL.Helpers;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Header;
using BPMS_DTOs.Workflow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class WorkflowController : BaseController
    {
        private readonly WorkflowFacade _workflowFacade;

        public WorkflowController(WorkflowFacade workflowFacade)
        : base(workflowFacade)
        {
            _workflowFacade = workflowFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _workflowFacade.SetFilters(_filters, _userId);
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("WorkflowOverview", await _workflowFacade.Overview());
        }

        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            return Ok(new
            {
                header = await this.RenderViewAsync("Partial/_WorkflowOverviewHeader", true),
                filters = await this.RenderViewAsync("Partial/_WorkflowOverviewFilters", true)
            });
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
                activeBlock = dto.ActiveBlock,
            });
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
            WorkflowOverviewDTO overview = await _workflowFacade.Filter(dto);
            return Ok(new
            {
                overview = await this.RenderViewAsync("Partial/_WorkflowOverview", overview.Workflows, true),
                activeBlocks = overview.ActiveBlocks,
            });
        }

        [HttpPost("Admin, WorkflowKeeper")]
        public async Task<IActionResult> Edit(WorkflowEditDTO dto)
        {
            WorkflowInfoCardDTO infoCard = await _workflowFacade.Edit(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_WorkflowDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_WorkflowCard", (infoCard.SelectedWorkflow, true), true),
            });
        }
    }
}
