using BPMS_BL.Facades;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.Pool;
using Microsoft.AspNetCore.Mvc;
using BPMS_DTOs.Attribute;
using Microsoft.AspNetCore.Authorization;
using BPMS_DTOs.Model;

namespace BPMS.Controllers
{
    [Authorize(Roles = "Admin, ModelKeeper")]
    public class PoolController : BaseController
    {
        private readonly PoolFacade _PoolFacade;

        public PoolController(PoolFacade PoolFacade)
        : base(PoolFacade)
        {
            _PoolFacade = PoolFacade;
        }

        [HttpGet]
        public async Task<IActionResult> Config(Guid id)
        {
            
            return PartialView("Partial/_PoolConfig", await _PoolFacade.Config(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PoolEditDTO dto)
        {
            ModelDetailDTO detail = await _PoolFacade.Edit(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("../Model/Partial/_ModelDetailInfo", detail, true),
                card = await this.RenderViewAsync("../Model/Partial/_ModelCard", (detail.SelectedModel, true), true),
                header = await this.RenderViewAsync("../Model/Partial/_ModelDetailHeader", detail, true)
            });
        }
    }
}
