using BPMS_BL.Facades;
using BPMS_BL.Helpers;
using BPMS_DTOs.Filter;
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

            _modelFacade.SetFilters(_filters, _userId);
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
            return PartialView("Partial/_ModelOverview", await _modelFacade.Filter(dto));
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("ModelOverview", await _modelFacade.Overview());
        }


        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            return Ok(new
            {
                header = await this.RenderViewAsync("Partial/_ModelOverviewHeader", true),
                filters = await this.RenderViewAsync("Partial/_OverviewFilters", "Model", true)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("ModelDetail", await _modelFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            ModelDetailPartialDTO dto = await _modelFacade.DetailPartial(id);
            return Ok(new
            {
                detail = await this.RenderViewAsync("Partial/_ModelDetail", dto, true),
                header = await this.RenderViewAsync("Partial/_ModelDetailHeader", dto, true),
                activePools = dto.ActivePools
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, ModelKeeper")]
        public async Task<IActionResult> Edit(ModelEditDTO dto)
        {
            ModelInfoCardDTO infoCard = await _modelFacade.Edit(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_ModelDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_ModelCard", (infoCard.SelectedModel, true), true),
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> Share(Guid id)
        {
            ModelDetailDTO detail = await _modelFacade.Share(id);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_ModelDetailInfo", detail, true),
                model = await this.RenderViewAsync("Partial/_ModelSvg", detail.SVG, true),
                card = await this.RenderViewAsync("Partial/_ModelCard", (detail.SelectedModel, true), true),
                header = await this.RenderViewAsync("Partial/_ModelDetailHeader", detail, true)
            });
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
            (ModelDetailDTO? detail, Guid workflowId) = await _modelFacade.Run(dto);
            if (detail != null)
            {
                return Ok(new
                {
                    info = await this.RenderViewAsync("Partial/_ModelDetailInfo", detail, true),
                    card = await this.RenderViewAsync("Partial/_ModelCard", (detail.SelectedModel, true), true),
                    header = await this.RenderViewAsync("Partial/_ModelDetailHeader", detail, true)
                }); 
            }
            else
            {
                return Ok(workflowId);
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, ModelKeeper")]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _modelFacade.Remove(id);
            return Ok();
        }
    }
}
