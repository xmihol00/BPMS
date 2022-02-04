using BPMS_BL.Facades;
using BPMS_DTOs.BlockDataSchema;
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

        [HttpPost]
        public async Task<IActionResult> CreateEditSchema(BlockDataSchemaCreateEditDTO dto)
        {
            return PartialView("Partial/_BlockModelConfig", await _blockModelFacade.CreateEditSchema(dto));
        }
    }
}
