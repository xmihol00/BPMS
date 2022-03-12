using BPMS_BL.Facades;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    [Authorize(Roles = "Admin, AgendaKeeper")]
    public class ModelUploadController : BaseController
    {
        private readonly ModelUploadFacade _modelUploadFacade;

        public ModelUploadController(ModelUploadFacade modelUploadFacade)
        : base(modelUploadFacade)
        {
            _modelUploadFacade = modelUploadFacade;
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> Upload(ModelCreateDTO dto)
        {
            return Redirect($"/Model/Detail/{await _modelUploadFacade.Upload(dto)}");
        }
    }
}
