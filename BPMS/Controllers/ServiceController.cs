using BPMS_BL.Facades;
using BPMS_DTOs.Service;
using BPMS_DTOs.ServiceDataSchema;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    public class ServiceController : Controller
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

        [HttpPost]
        public async Task<IActionResult> CreateEdit(ServiceCreateEditDTO dto)
        {
            return Redirect($"/Service/Edit/{await _serviceFacade.CreateEdit(dto)}");
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditSchema(ServiceDataSchemaCreateEditDTO dto)
        {
            return PartialView("Partial/_ServiceDataSchema", (await _serviceFacade.CreateEditSchema(dto), dto.Direction));
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
            return PartialView("Partial/_ServiceSendRequest", await _serviceFacade.SendRequest(data));
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
    }
}
