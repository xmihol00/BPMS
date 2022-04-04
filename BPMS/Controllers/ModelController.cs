using BPMS_BL.Facades;
using BPMS_BL.Helpers;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Lane;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BPMS.Controllers
{
    [Authorize]
    public class ModelController : BaseController
    {
        private readonly ModelFacade _modelFacade;

        public ModelController(ModelFacade modelFacade)
        : base(modelFacade)
        {
            _modelFacade = modelFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _modelFacade.SetFilters(_filters);
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            try
            {
                CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
                return PartialView("Partial/_ModelOverview", await _modelFacade.Filter(dto));
            }
            catch
            {
                return BadRequest("Filtrování selhalo.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("ModelOverview", await _modelFacade.Overview());
        }


        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            Task<string> header = this.RenderViewAsync("Partial/_ModelOverviewHeader", true);
            Task<string> filters = this.RenderViewAsync("Partial/_OverviewFilters", "Model", true);
            return Ok(new { header = await header, filters = await filters });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("ModelDetail", await _modelFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            try
            {
                ModelDetailPartialDTO dto = await _modelFacade.DetailPartial(id);
                Task<string> detail = this.RenderViewAsync("Partial/_ModelDetail", dto, true);
                Task<string> header = this.RenderViewAsync("Partial/_ModelDetailHeader", dto, true);
                return Ok(new
                {
                    detail = await detail,
                    header = await header,
                    activePools = dto.ActivePools,
                    activeBlocks = dto.ActiveBlocks
                });
            }
            catch
            {
                return BadRequest("Model se nepodařilo nalézt.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, ModelKeeper")]
        public async Task<IActionResult> Edit(ModelEditDTO dto)
        {
            try
            {
                ModelInfoCardDTO infoCard = await _modelFacade.Edit(dto);
                Task<string> info = this.RenderViewAsync("Partial/_ModelDetailInfo", infoCard, true);
                Task<string> card = this.RenderViewAsync("Partial/_ModelCard", (infoCard.SelectedModel, true), true);
                return Ok(new { info = await info, card = await card });
            }
            catch
            {
                return BadRequest("Editace modelu selhala.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> Share(Guid id)
        {
            try
            {
                ModelDetailDTO detail = await _modelFacade.Share(id);
                Task<string> info = this.RenderViewAsync("Partial/_ModelDetailInfo", detail, true);
                Task<string> model = this.RenderViewAsync("Partial/_ModelSvg", detail.SVG, true);
                Task<string> card = this.RenderViewAsync("Partial/_ModelCard", (detail.SelectedModel, true), true);
                Task<string> header = this.RenderViewAsync("Partial/_ModelDetailHeader", detail, true);
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
                return BadRequest("Sdílení modelu selhalo.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> Run(Guid id)
        {
            return PartialView("Partial/_ModelRun", await _modelFacade.Run(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> Run(ModelRunDTO dto)
        {
            try
            {
                (ModelDetailDTO? detail, Guid workflowId) = await _modelFacade.Run(dto);
                if (detail != null)
                {
                    Task<string> info = this.RenderViewAsync("Partial/_ModelDetailInfo", detail, true);
                    Task<string> card = this.RenderViewAsync("Partial/_ModelCard", (detail.SelectedModel, true), true);
                    Task<string> header = this.RenderViewAsync("Partial/_ModelDetailHeader", detail, true);
                    return Ok(new { info = await info, card = await card, header = await header }); 
                }
                else
                {
                    return Ok(workflowId);
                }
            }
            catch
            {
                return BadRequest("Spuštění workflow selhalo.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, ModelKeeper")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _modelFacade.Remove(id);
            return Ok();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, ModelKeeper")]
        public async Task<IActionResult> LaneConfig(Guid id)
        {
            return PartialView("Partial/_LaneConfig", await _modelFacade.LaneConfig(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, ModelKeeper")]
        public async Task<IActionResult> LaneEdit(LaneEditDTO dto)
        {
            try
            {
                await _modelFacade.LaneEdit(dto);
                return Ok();
            }
            catch
            {
                return BadRequest("Editace dráhy modelu selhala.");
            }
        }
    }
}
