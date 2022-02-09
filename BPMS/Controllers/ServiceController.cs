using BPMS_BL.Facades;
using BPMS_DTOs.ServiceDataSchema;
using Microsoft.AspNetCore.Mvc;

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
        [Route("/Service/Edit")]
        public async Task<IActionResult> CreateEdit(Guid id)
        {
            return View("ServiceCreateEdit", await _serviceFacade.CreateEdit(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditSchema(ServiceDataSchemaCreateEditDTO dto)
        {
            return PartialView("Partial/_BlockModelConfig", await _serviceFacade.CreateEditSchema(dto));
        }
    }
}
