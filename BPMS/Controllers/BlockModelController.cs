using BPMS_BL.Facades;
using BPMS_DTOs.BlockModel;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    public class BlockModelController : Controller
    {
        private readonly BlockModelFacade _blockModelFacade;

        public BlockModelController(BlockModelFacade blockModelFacade)
        {
            _blockModelFacade = blockModelFacade;
        }

        [HttpGet]
        public async Task<IActionResult> Config(Guid id)
        {
            return PartialView("Partial/_BlockModelConfig", await _blockModelFacade.Config(id));
        }
    }
}
