using BPMS_BL.Facades;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.Pool;
using Microsoft.AspNetCore.Mvc;
using BPMS_DTOs.BlockAttribute;

namespace BPMS.Controllers
{
    public class PoolController : Controller
    {
        private readonly PoolFacade _PoolFacade;

        public PoolController(PoolFacade PoolFacade)
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
            await _PoolFacade.Edit(dto); // TODO
            return Ok();
        }
    }
}
