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
            try
            {
                CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
                return PartialView("Partial/_ServiceOverview", await _serviceFacade.Filter(dto));
            }
            catch
            {
                return BadRequest("Filtrování selhalo.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("ServiceOverview", await _serviceFacade.Overview());
        }

        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            Task<string> header = this.RenderViewAsync("Partial/_ServiceOverviewHeader", true);
            Task<string> filters = this.RenderViewAsync("Partial/_OverviewFilters", "Service", true);
            return Ok(new { header = await header, filters = await filters });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("ServiceDetail", await _serviceFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            try
            {
                ServiceDetailPartialDTO dto = await _serviceFacade.DetailPartial(id);
                Task<string> detail = this.RenderViewAsync("Partial/_ServiceDetail", dto, true);
                Task<string> header = this.RenderViewAsync("Partial/_ServiceDetailHeader", dto, true);
                return Ok(new { detail = await detail, header = await header });
            }
            catch
            {
                return BadRequest("Webovou službu se nepodařilo nalézt");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(ServiceCreateEditDTO dto)
        {
            return Redirect($"/Service/Detail/{await _serviceFacade.Create(dto)}");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ServiceCreateEditDTO dto)
        {
            try
            {
                ServiceInfoCardDTO infoCard = await _serviceFacade.Edit(dto);
                Task<string> info = this.RenderViewAsync("Partial/_ServiceDetailInfo", infoCard, true);
                Task<string> card = this.RenderViewAsync("Partial/_ServiceCard", (infoCard.SelectedService, true), true);
                return Ok(new { info = await info, card = await card });
            }
            catch
            {
                return BadRequest("Editace webové služby selhala.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditSchema(DataSchemaCreateEditDTO dto)
        {
            try
            {
                return PartialView($"Partial/_{dto.Direction}DataSchema", await _serviceFacade.CreateEditSchema(dto));
            }
            catch
            {
                return BadRequest("Vytvoření schématu služby selhalo.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveSchema(Guid id)
        {
            try
            {
                await _serviceFacade.RemoveSchema(id);
                return Ok();
            }
            catch
            {
                return BadRequest("Odtranění schématu služby selhalo.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Test(IFormCollection data)
        {
            try
            {
                return PartialView("Partial/_ServiceSentRequest", await _serviceFacade.SendRequest(data));
            }
            catch
            {
                return BadRequest("Testovací volání služby selhalo.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Generate(IFormCollection data)
        {
            try
            {
                return PartialView("Partial/_ServiceGeneratedRequest", await _serviceFacade.GenerateRequest(data));
            }
            catch
            {
                return BadRequest("Vygenerování výstupních schémat služby selhalo.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Test(Guid id)
        {
            return PartialView("Partial/_ServiceTestInput", await _serviceFacade.Test(id));
        }

        [HttpPost]
        public async Task<IActionResult> GenerateAttributes(ServiceCallResultDTO dto)
        {
            try
            {
                return PartialView("Partial/_OutputDataSchema", await _serviceFacade.GenerateAttributes(dto)); 
            }
            catch
            {
                return BadRequest("Vygenerování výstupních schémat služby selhalo.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditHeader(HeaderCreateEditDTO dto)
        {
            try
            {
                return PartialView("Partial/_ServiceHeaders", await _serviceFacade.CreateEditHeader(dto));
            }
            catch
            {
                return BadRequest("Vytvoření nebo editace hlavičky webové služby selhala.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveHeader(Guid id)
        {
            try
            {
                await _serviceFacade.RemoveHeader(id);
                return Ok();
            }
            catch
            {
                return BadRequest("Odstranění hlavičky webové služby selhalo.");
            }
        }
    }
}
