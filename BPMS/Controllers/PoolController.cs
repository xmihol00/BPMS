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
            try
            {
                ModelDetailDTO detail = await _PoolFacade.Edit(dto);
                Task<string> info = this.RenderViewAsync("../Model/Partial/_ModelDetailInfo", detail, true);
                Task<string> model = this.RenderViewAsync("../Model/Partial/_ModelSvg", detail.SVG, true);
                Task<string> card = this.RenderViewAsync("../Model/Partial/_ModelCard", (detail.SelectedModel, true), true);
                Task<string> header = this.RenderViewAsync("../Model/Partial/_ModelDetailHeader", detail, true);
                return Ok(new
                {
                    info = await info,
                    model = await model,
                    card = await card,
                    header = await header
                });
            }
            catch
            {
                return BadRequest("Editace bazénu selhala.");
            }
        }
    }
}
