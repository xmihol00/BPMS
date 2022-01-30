using BPMS_BL.Facades;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    public class ModelUploadController : Controller
    {
        private readonly ModelUploadFacade _modelUploadFacade;

        public ModelUploadController(ModelUploadFacade modelUploadFacade)
        {
            _modelUploadFacade = modelUploadFacade;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(CreateModelDTO dto)
        {
            await _modelUploadFacade.Upload(dto);
            return View();
        }
    }
}
