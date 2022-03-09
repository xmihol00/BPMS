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
            });
        }

        [HttpPost]
        [Authorize("Admin, ModelKeeper")]
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
        [Authorize("Admin, AgendaKeeper")]
        public async Task<IActionResult> Share(Guid id)
        {
            return Ok(await _modelFacade.Share(id));
        }

        [HttpGet]
        [Authorize("Admin, AgendaKeeper")]
        public async Task<IActionResult> Run(Guid id)
        {
            return PartialView("Partial/_ModelRun", await _modelFacade.Run(id));
        }

        [HttpPost]
        [Authorize("Admin, AgendaKeeper")]
        public async Task<IActionResult> Run(ModelRunDTO dto)
        {
            return Ok(await _modelFacade.Run(dto));
        }

        [HttpPost]
        [Authorize("Admin, ModelKeeper")]
        public async Task<IActionResult> Remove(Guid id)
        {
            return Ok(await _modelFacade.Remove(id));
        }
    }
}
