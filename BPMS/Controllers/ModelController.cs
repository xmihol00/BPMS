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
        public async Task<IActionResult> Transition(Guid id)
        {
            ModelHeaderDTO header = await _modelFacade.Header(id);
            return Ok(new
            {
                header = await this.RenderViewAsync("Partial/_ModelHeader", header, true),
                info = await this.RenderViewAsync("Partial/_ModelInfo", header, true),
            });
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("ModelDetail", (await _modelFacade.Detail(id), ""));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(ModelEditDTO dto)
        {
            return PartialView("Partial/_ModelInfo", await _modelFacade.Edit(dto));
        }

        [HttpPost]
        public async Task<IActionResult> Share(Guid id)
        {
            return Ok(await _modelFacade.Share(id));
        }

        [HttpPost]
        public async Task<IActionResult> Run(Guid id)
        {
            return Ok(await _modelFacade.Run(id));
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid id)
        {
            return Ok(await _modelFacade.Remove(id));
        }
    }
}
