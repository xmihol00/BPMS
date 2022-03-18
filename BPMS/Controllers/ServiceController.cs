using BPMS_BL.Facades;
using BPMS_DTOs.Header;
using BPMS_DTOs.Service;
using BPMS_DTOs.DataSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Microsoft.AspNetCore.Mvc.Filters;
using BPMS_DTOs.Filter;
using BPMS_BL.Helpers;

namespace BPMS.Controllers
{
    [Authorize(Roles = "Admin, ServiceKeeper")]
    public class ServiceController : BaseController
    {
        private readonly ServiceFacade _serviceFacade;

        public ServiceController(ServiceFacade serviceFacade)
        : base(serviceFacade)
        {
            _serviceFacade = serviceFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _serviceFacade.SetFilters(_filters);
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
            return PartialView("Partial/_ServiceOverview", await _serviceFacade.Filter(dto));
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("ServiceOverview", await _serviceFacade.Overview());
        }

        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            return Ok(new
            {
                header = await this.RenderViewAsync("Partial/_ServiceOverviewHeader", true),
                filters = await this.RenderViewAsync("Partial/_OverviewFilters", "Service", true)
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("ServiceDetail", await _serviceFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            ServiceDetailPartialDTO dto = await _serviceFacade.DetailPartial(id);
            return Ok(new
            {
                detail = await this.RenderViewAsync("Partial/_ServiceDetail", dto, true),
                header = await this.RenderViewAsync("Partial/_ServiceDetailHeader", dto, true),
            });
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceCreateEditDTO dto)
        {
            return Redirect($"/Service/Detail/{await _serviceFacade.Create(dto)}");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceCreateEditDTO dto)
        {
            ServiceInfoCardDTO infoCard = await _serviceFacade.Edit(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_ServiceDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_ServiceCard", (infoCard.SelectedService, true), true),
            });
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditSchema(DataSchemaCreateEditDTO dto)
        {
            return PartialView($"Partial/_{dto.Direction}DataSchema", await _serviceFacade.CreateEditSchema(dto));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSchema(Guid id)
        {
            await _serviceFacade.RemoveSchema(id);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Test(IFormCollection data)
        {
            return PartialView("Partial/_ServiceSentRequest", await _serviceFacade.SendRequest(data));
        }

        [HttpPost]
        public async Task<IActionResult> Generate(IFormCollection data)
        {
            return PartialView("Partial/_ServiceGeneratedRequest", await _serviceFacade.GenerateRequest(data));
        }

        [HttpGet]
        public async Task<IActionResult> Test(Guid id)
        {
            return PartialView("Partial/_ServiceTestInput", await _serviceFacade.Test(id));
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAttributes(ServiceCallResultDTO dto)
        {
            return PartialView("Partial/_OutputDataSchema", await _serviceFacade.GenerateAttributes(dto)); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditHeader(HeaderCreateEditDTO dto)
        {
            return PartialView("Partial/_ServiceHeaders", await _serviceFacade.CreateEditHeader(dto));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveHeader(Guid id)
        {
            await _serviceFacade.RemoveHeader(id);
            return Ok();
        }
    }
}
