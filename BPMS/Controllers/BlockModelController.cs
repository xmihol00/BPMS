using BPMS_BL.Facades;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.BlockModel;
using Microsoft.AspNetCore.Mvc;
using BPMS_DTOs.BlockAttribute;
using Microsoft.AspNetCore.Authorization;

namespace BPMS.Controllers
{
    [Authorize]
    public class BlockModelController : BaseController
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

        [HttpGet]
        public async Task<IActionResult> PoolConfig(Guid id)
        {
            return PartialView("Partial/_PoolConfig", await _blockModelFacade.PoolConfig(id));
        }

        [HttpPost]
        public async Task<IActionResult> CreateEditAttribute(AttributeCreateEditDTO dto)
        {
            return PartialView("Partial/_BlockModelConfig", await _blockModelFacade.CreateEdit(dto));
        }

        [HttpPost]
        [Route("/BlockModel/ToggleMap/{blockId}/{attributeId}")]
        public async Task<IActionResult> ToggleMap(Guid blockId, Guid attributeId)
        {
            await _blockModelFacade.ToggleMap(blockId, attributeId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BlockModelEditDTO dto)
        {
            await _blockModelFacade.Edit(dto);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _blockModelFacade.Remove(id);
            return Ok();
        }
    }
}
