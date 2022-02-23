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

        [HttpPost]
        public async Task<IActionResult> CreateEditAttribute(AttributeCreateEditDTO dto)
        {
            return PartialView("Partial/_BlockModelConfig", await _blockModelFacade.CreateEdit(dto));
        }

        [HttpPost]
        [Route("/BlockModel/ToggleTaskMap/{blockId}/{attributeId}")]
        public async Task<IActionResult> ToggleTaskMap(Guid blockId, Guid attributeId)
        {
            await _blockModelFacade.ToggleTaskMap(blockId, attributeId);
            return Ok();
        }

        [HttpPost]
        [Route("/BlockModel/ToggleServiceMap/{blockId}/{dataSchemaId}")]
        public async Task<IActionResult> ToggleServicekMap(Guid blockId, Guid dataSchemaId)
        {
            await _blockModelFacade.ToggleServiceMap(blockId, dataSchemaId);
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
