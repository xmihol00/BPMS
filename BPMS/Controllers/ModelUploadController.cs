using BPMS_BL.Facades;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    [Authorize]
    public class ModelUploadController : BaseController
    {
        private readonly ModelUploadFacade _modelUploadFacade;

        public ModelUploadController(ModelUploadFacade modelUploadFacade)
        : base(modelUploadFacade)
        {
            _modelUploadFacade = modelUploadFacade;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(ModelCreateDTO dto)
        {
            await _modelUploadFacade.Upload(dto);
            return Redirect($"/Agenda/Detail/{dto.AgendaId}");
        }
    }
}
