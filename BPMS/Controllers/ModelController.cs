using BPMS_BL.Facades;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    public class ModelController : Controller
    {
        private readonly ModelFacade _modelFacade;

        public ModelController(ModelFacade modelFacade)
        {
            _modelFacade = modelFacade;
        }

        [HttpGet]
        public async Task<IActionResult> Header(Guid id)
        {
            return PartialView("Partial/_ModelHeader", await _modelFacade.Header(id));
        }

        public async Task<IActionResult> Detail(Guid id)
        {
            return View("ModelDetail", await _modelFacade.Detail(id));
        }
    }
}
