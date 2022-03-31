using BPMS_BL.Facades;
using BPMS_BL.Helpers;
using BPMS_Common.Enums;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Header;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class SystemController : BaseController
    {
        private readonly SystemFacade _systemFacade;

        public SystemController(SystemFacade systemFacade)
        : base(systemFacade)
        {
            _systemFacade = systemFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _systemFacade.SetFilters(_filters, ViewBag.UserName);
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
            return PartialView("Partial/_SystemOverview", await _systemFacade.Filter(dto));
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("SystemOverview", await _systemFacade.Overview());
        }

        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            return Ok(new
            {
                header = await this.RenderViewAsync("Partial/_SystemOverviewHeader", true),
                filters = await this.RenderViewAsync("Partial/_OverviewFilters", "System", true)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("SystemDetail", await _systemFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            SystemDetailPartialDTO dto = await _systemFacade.DetailPartial(id);

            return Ok(new
            {
                detail = await this.RenderViewAsync("Partial/_SystemDetail", dto, true),
                header = await this.RenderViewAsync("Partial/_SystemDetailHeader", dto, true),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(SystemCreateDTO dto)
        {
            return Redirect($"/System/Detail/{await _systemFacade.Create(dto)}");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SystemEditDTO dto)
        {
            SystemInfoCardDTO infoCard = await _systemFacade.Edit(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true),
            });
        }

        [HttpPost]
        public async Task<IActionResult> AddActivate(SystemActivateDTO dto)
        {
            SystemInfoCardDTO infoCard = await _systemFacade.Activate(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true),
            });
        }

        [HttpGet]
        public async Task<IActionResult> ConnectionRequest(Guid id)
        {
            return PartialView("Partial/_ConnectionRequest", await _systemFacade.ConnectionRequest(id));
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            SystemInfoCardDTO infoCard = await _systemFacade.Deactivate(id);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Reactivate(SystemReactivateDTO dto)
        {
            SystemInfoCardDTO infoCard = await _systemFacade.Reactivate(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true),
            });
        }
    }
}
