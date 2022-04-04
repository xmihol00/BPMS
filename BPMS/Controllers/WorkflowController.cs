using BPMS_BL.Facades;
using BPMS_BL.Helpers;
using BPMS_Common.Enums;
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

            _workflowFacade.SetFilters(_filters);
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("WorkflowOverview", await _workflowFacade.Overview());
        }

        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            Task<string> header = this.RenderViewAsync("Partial/_WorkflowOverviewHeader", true);
            Task<string> filters = this.RenderViewAsync("Partial/_OverviewFilters", "Workflow", true);
            return Ok(new { header = await header, filters = await filters });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("WorkflowDetail", await _workflowFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            try
            {
                WorkflowDetailPartialDTO dto = await _workflowFacade.DetailPartial(id);
                dto.Editable |= (bool)ViewData[SystemRoleEnum.Admin.ToString()];
                Task<string> detail = this.RenderViewAsync("Partial/_WorkflowDetail", dto, true);
                Task<string> header = this.RenderViewAsync("Partial/_WorkflowDetailHeader", dto, true);
                return Ok(new
                {
                    detail = await detail,
                    header = await header,
                    activeBlock = dto.ActiveBlock,
                    editable = dto.Editable
                });
            }
            catch
            {
                return BadRequest("Workflow nebylo nalezeno.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            try
            {
                CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
                WorkflowOverviewDTO overview = await _workflowFacade.Filter(dto);
                return Ok(new
                {
                    overview = await this.RenderViewAsync("Partial/_WorkflowOverview", overview.Workflows, true),
                    activeBlocks = overview.ActiveBlocks,
                });
            }
            catch
            {
                return BadRequest("Filtrování selhalo.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, WorkflowKeeper")]
        public async Task<IActionResult> Edit(WorkflowEditDTO dto)
        {
            try
            {
                WorkflowInfoCardDTO infoCard = await _workflowFacade.Edit(dto);
                Task<string> info = this.RenderViewAsync("Partial/_WorkflowDetailInfo", infoCard, true);
                Task<string> card = this.RenderViewAsync("Partial/_WorkflowCard", (infoCard.SelectedWorkflow, true), true);
                return Ok(new { info = await info, card = await card });
            }
            catch
            {
                return BadRequest("Editace workflow selhala.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Keepers(Guid id)
        {
            return PartialView("Partial/_WorkflowAdminChange", await _workflowFacade.Keepers(id));            
        }
    }
}
