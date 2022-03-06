using BPMS_BL.Facades;
using BPMS_DTOs.Header;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class SystemController : BaseController
    {
        private readonly SystemFacade _systemFacade;

        public SystemController(SystemFacade systemFacade)
        : base(systemFacade)
        {
            _systemFacade = systemFacade;
        }

        public async Task<IActionResult> Overview()
        {
            return View("SystemOverview", await _systemFacade.Overview());
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            return View("SystemDetail", await _systemFacade.Detail(id));
        }

        public async Task<IActionResult> DetailPartial(Guid id)
        {
            SystemDetailPartialDTO dto = await _systemFacade.DetailPartial(id);

            return Ok(new
            {
                detail = await this.RenderViewAsync("Partial/_SystemDetail", dto, true),
                header = await this.RenderViewAsync("Partial/_SystemDetailHeader", dto, true),
            });
        }

        public async Task<IActionResult> Create(SystemCreateDTO dto)
        {
            return Redirect($"/System/Detail/{await _systemFacade.Create(dto)}");
        }

        public async Task<IActionResult> Edit(SystemEditDTO dto)
        {
            SystemInfoCardDTO infoCard = await _systemFacade.Edit(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true),
            });
        }

        public async Task<IActionResult> AddActivate(SystemActivateDTO dto)
        {
            SystemInfoCardDTO infoCard = await _systemFacade.Activate(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_SystemDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_SystemCard", (infoCard.SelectedSystem, true), true),
            });
        }
    }
}
