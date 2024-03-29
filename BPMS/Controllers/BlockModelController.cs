using BPMS_BL.Facades;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.BlockModel;
using Microsoft.AspNetCore.Mvc;
using BPMS_DTOs.Attribute;
using Microsoft.AspNetCore.Authorization;

namespace BPMS.Controllers
{
    [Authorize(Roles = "Admin, ModelKeeper")]
    public class BlockModelController : BaseController
    {
        private readonly BlockModelFacade _blockModelFacade;

        public BlockModelController(BlockModelFacade blockModelFacade)
        : base(blockModelFacade)
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
            try
            {
                await _blockModelFacade.ToggleTaskMap(blockId, attributeId);
                return Ok();
            }
            catch
            {
                return BadRequest("Namapování atributu úkolu selhalo.");
            }
        }

        [HttpPost]
        [Route("/BlockModel/ToggleSendMap/{blockId}/{attributeId}")]
        public async Task<IActionResult> ToggleSendMap(Guid blockId, Guid attributeId)
        {
            return Ok(await _blockModelFacade.ToggleSendMap(blockId, attributeId));
        }

        [HttpPost]
        [Route("/BlockModel/ToggleServiceMap/{blockId}/{dataSchemaId}/{serviceTaskId}")]
        public async Task<IActionResult> ToggleServicekMap(Guid blockId, Guid dataSchemaId, Guid serviceTaskId)
        {
            try
            {
                await _blockModelFacade.ToggleServiceMap(blockId, dataSchemaId, serviceTaskId);
                return Ok();
            }
            catch
            {
                return BadRequest("Namapování atributu služby selhalo.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BlockModelEditDTO dto)
        {
            try
            {
                await _blockModelFacade.Edit(dto);
                return Ok();
            }
            catch
            {
                return BadRequest("Editace bloku selhala.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAttribute(Guid id)
        {
            await _blockModelFacade.RemoveAttribute(id);
            return Ok();
        }

        [HttpPost]
        [Route("/BlockModel/ChangeSender/{modelId}/{blockId}")]
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
        [Route("/BlockModel/Models/{systemId}/{agendaId}")]
        public async Task<IActionResult> Models(Guid systemId, Guid agendaId)
        {
            return PartialView("Partial/_ModelPicker", await _blockModelFacade.Models(systemId, agendaId));
        }

        [HttpPost]
        [Route("/BlockModel/Pools/{systemId}/{modelId}")]
        public async Task<IActionResult> Pools(Guid systemId, Guid modelId)
        {
            return PartialView("Partial/_PoolPicker", await _blockModelFacade.Pools(systemId, modelId));
        }

        [HttpPost]
        [Route("/BlockModel/SenderBlocks/{systemId}/{poolId}")]
        public async Task<IActionResult> SenderBlocks(Guid systemId, Guid poolId)
        {
            return PartialView("Partial/_BlockPicker", await _blockModelFacade.SenderBlocks(systemId, poolId));
        }

        [HttpPost]
        public async Task<IActionResult> SenderChange(SenderChangeDTO dto)
        {
            return PartialView("Partial/_AttributesConfig", await _blockModelFacade.SenderChange(dto));
        }

        [HttpPost]
        [Route("/BlockModel/AddMap/{serviceTaskId}/{sourceId}/{targetId}")]
        public async Task<IActionResult> AddMap(Guid serviceTaskId, Guid sourceId, Guid targetId)
        {
            return PartialView("Partial/_ServiceMapConfig", await _blockModelFacade.AddMap(serviceTaskId, sourceId, targetId));
        }

        [HttpPost]
        [Route("/BlockModel/RemoveMap/{serviceTaskId}/{sourceId}/{targetId}")]
        public async Task<IActionResult> RemoveMap(Guid serviceTaskId, Guid sourceId, Guid targetId)
        {
            return PartialView("Partial/_ServiceMapConfig", await _blockModelFacade.RemoveMap(serviceTaskId, sourceId, targetId));
        }
    }
}
