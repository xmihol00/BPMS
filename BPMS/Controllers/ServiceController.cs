using BPMS_BL.Facades;
using BPMS_DTOs.Header;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class ServiceController : BaseController
    {
        private readonly ServiceFacade _serviceFacade;

        public ServiceController(ServiceFacade serviceFacade)
        {
            _serviceFacade = serviceFacade;
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("ServiceOverview", await _serviceFacade.Overview());
        }

        [HttpGet]
        [Route("/Service/Create")]
        [Route("/Service/Edit/{id}")]
        public async Task<IActionResult> CreateEdit(Guid id)
        {
            return View("ServiceCreateEdit", await _serviceFacade.CreateEdit(id));
        }

        [HttpGet]
        public async Task<IActionResult> EditPartial(Guid id)
        {
            ServiceEditPagePartialDTO dto = await _serviceFacade.EditParial(id);
            return Ok(new
            {
                detail = await this.RenderViewAsync("Partial/_ServiceCreateEdit", dto, true),
                header = await this.RenderViewAsync("Partial/_ServiceEditHeader", dto, true),
            });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            return View("AgendaCreateEdit", await _serviceFacade.Edit(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEdit(ServiceCreateEditDTO dto)
        {
            return Redirect($"/Service/Edit/{await _serviceFacade.CreateEdit(dto)}");
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
        public async Task<IActionResult> GenerateAttributes(ServiceTestResultDTO dto)
        {
            return PartialView("Partial/_OutputDataSchema", await _serviceFacade.GenerateAttributes(dto)); 
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditHeader(HeaderCreateEditDTO dto)
        {
            return PartialView("Partial/_HeaderAll", await _serviceFacade.CreateEditHeader(dto));
        }

        [HttpPost]
        public async Task<IActionResult> RemoveHeader(Guid id)
        {
            await _serviceFacade.RemoveHeader(id);
            return Ok();
        }
    }
}
