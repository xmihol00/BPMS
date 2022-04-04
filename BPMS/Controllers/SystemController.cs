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
            try
            {
                CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
                return PartialView("Partial/_SystemOverview", await _systemFacade.Filter(dto));
            }
            catch
            {
                return BadRequest("Filtrování selhalo.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("SystemOverview", await _systemFacade.Overview());
        }

        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            Task<string> header = this.RenderViewAsync("Partial/_SystemOverviewHeader", true);
            Task<string> filters = this.RenderViewAsync("Partial/_OverviewFilters", "System", true);
            return Ok(new { header = await header, filters = await filters });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("SystemDetail", await _systemFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            try
            {
                SystemDetailPartialDTO dto = await _systemFacade.DetailPartial(id);
                Task<string> detail = this.RenderViewAsync("Partial/_SystemDetail", dto, true);
                Task<string> header = this.RenderViewAsync("Partial/_SystemDetailHeader", dto, true);
                return Ok(new { detail = await detail, header = await header });
            }
            catch
            {
                return BadRequest("Systém nenalezen.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(SystemCreateDTO dto)
        {
            return Redirect($"/System/Detail/{await _systemFacade.Create(dto)}");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SystemEditDTO dto)
        {
            try
            {
                SystemInfoCardDTO infoCard = await _systemFacade.Edit(dto);
                Task<string> info = this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true);
                Task<string> card = this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true);
                return Ok(new { info = await info, card = await card });
            }
            catch
            {
                return BadRequest("Editace systému selhala.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddActivate(SystemActivateDTO dto)
        {
            try
            {
                SystemInfoCardDTO infoCard = await _systemFacade.Activate(dto);
                Task<string> info = this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true);
                Task<string> card = this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true);
                return Ok(new { info = await info, card = await card });
            }
            catch
            {
                return BadRequest("Aktivace systému selhala");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ConnectionRequest(Guid id)
        {
            return PartialView("Partial/_ConnectionRequest", await _systemFacade.ConnectionRequest(id));
        }

        [HttpPost]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            try
            {
                SystemInfoCardDTO infoCard = await _systemFacade.Deactivate(id);
                Task<string> info = this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true);
                Task<string> card = this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true);
                return Ok(new { info = await info, card = await card });
            }
            catch
            {
                return BadRequest("Deaktivace systému selhala.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Reactivate(SystemReactivateDTO dto)
        {
            try
            {
                SystemInfoCardDTO infoCard = await _systemFacade.Reactivate(dto);
                Task<string> info = this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true);
                Task<string> card = this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true);
                return Ok(new { info = await info, card = await card });
            }
            catch
            {
                return BadRequest("Reaktivace systmu selhala.");
            }
        }
    }
}
