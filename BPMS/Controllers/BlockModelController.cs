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
            return PartialView("Partial/_AttributesConfig", await _blockModelFacade.CreateEdit(dto));
        }

        [HttpPost]
        [Route("/BlockModel/ToggleTaskMap/{blockId}/{attributeId}")]
        public async Task<IActionResult> ToggleTaskMap(Guid blockId, Guid attributeId)
        {
            await _blockModelFacade.ToggleTaskMap(blockId, attributeId);
            return Ok();
        }

        [HttpPost]
        [Route("/BlockModel/ToggleSendMap/{blockId}/{attributeId}")]
        public async Task<IActionResult> ToggleSendMap(Guid blockId, Guid attributeId)
        {
            await _blockModelFacade.ToggleSendMap(blockId, attributeId);
            return Ok();
        }

        [HttpPost]
        [Route("/BlockModel/ToggleServiceMap/{blockId}/{dataSchemaId}/{serviceTaskId}")]
        public async Task<IActionResult> ToggleServicekMap(Guid blockId, Guid dataSchemaId, Guid serviceTaskId)
        {
            await _blockModelFacade.ToggleServiceMap(blockId, dataSchemaId, serviceTaskId);
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

        [HttpPost]
        [Route("BlockModel/ChangeSender/{modelId}/{blockId}")]
        public async Task<IActionResult> ChangeSender(Guid modelId, Guid blockId)
        {
            return PartialView("Partial/_ChangeSender", (await _blockModelFacade.ChangeSender(modelId), blockId));
        }

        [HttpPost]
        public async Task<IActionResult> Agendas(Guid id)
        {
            return PartialView("Partial/_AgendaPicker", await _blockModelFacade.Agendas(id));
        }

        [HttpPost]
        [Route("BlockModel/Models/{systemId}/{agendaId}")]
        public async Task<IActionResult> Models(Guid systemId, Guid agendaId)
        {
            return PartialView("Partial/_ModelPicker", await _blockModelFacade.Models(systemId, agendaId));
        }

        [HttpPost]
        [Route("BlockModel/Pools/{systemId}/{modelId}")]
        public async Task<IActionResult> Pools(Guid systemId, Guid modelId)
        {
            return PartialView("Partial/_PoolPicker", await _blockModelFacade.Pools(systemId, modelId));
        }

        [HttpPost]
        [Route("BlockModel/SenderBlocks/{systemId}/{poolId}")]
        public async Task<IActionResult> SenderBlocks(Guid systemId, Guid poolId)
        {
            return PartialView("Partial/_BlockPicker", await _blockModelFacade.SenderBlocks(systemId, poolId));
        }

        [HttpPost]
        public async Task<IActionResult> SenderChange(SenderChangeDTO dto)
        {
            return PartialView("Partial/_AttributesConfig", await _blockModelFacade.SenderChange(dto));
        }
    }
}
