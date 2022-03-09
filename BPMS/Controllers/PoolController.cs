using BPMS_BL.Facades;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.Pool;
using Microsoft.AspNetCore.Mvc;
using BPMS_DTOs.Attribute;
using Microsoft.AspNetCore.Authorization;

namespace BPMS.Controllers
{
    [Authorize(Roles = "Admin, ModelKeeper")]
    public class PoolController : BaseController
    {
        private readonly PoolFacade _PoolFacade;

        public PoolController(PoolFacade PoolFacade)
        : base(PoolFacade)
        {
            _PoolFacade = PoolFacade;
        }

        [HttpGet]
        public async Task<IActionResult> Config(Guid id)
        {
            
            return PartialView("Partial/_PoolConfig", await _PoolFacade.Config(id));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PoolEditDTO dto)
        {
            return PartialView("../Model/Partial/_ModelSvg", await _PoolFacade.Edit(dto));
        }
    }
}
