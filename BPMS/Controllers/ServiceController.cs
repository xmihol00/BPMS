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


        [HttpPost]
        public async Task<IActionResult> CreateEditSchema(ServiceDataSchemaCreateEditDTO dto)
        {
            return PartialView("Partial/_BlockModelConfig", await _serviceFacade.CreateEditSchema(dto));
        }
    }
}
