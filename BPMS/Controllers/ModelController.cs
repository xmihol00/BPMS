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

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("ModelDetail", await _modelFacade.Detail(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ModelEditDTO dto)
        {
            return View("ModelDetail", await _modelFacade.Edit(dto));
        }
    }
}
