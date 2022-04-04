using BPMS_BL.Facades;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.BlockWorkflow;
using Microsoft.AspNetCore.Mvc;
using BPMS_DTOs.Attribute;
using Microsoft.AspNetCore.Authorization;
using BPMS_DTOs.BlockWorkflow.EditTypes;

namespace BPMS.Controllers
{
    [Authorize(Roles = "Admin, WorkflowKeeper")]
    public class BlockWorkflowController : BaseController
    {
        private readonly BlockWorkflowFacade _blockWorkflowFacade;

        public BlockWorkflowController(BlockWorkflowFacade blockWorkflowFacade)
        : base(blockWorkflowFacade)
        {
            _blockWorkflowFacade = blockWorkflowFacade;
        }

        [HttpGet]
        [Route("/BlockWorkflow/Config/{blockId}/{workflowId}")]
        public async Task<IActionResult> Config(Guid blockId, Guid workflowId)
        {
            return PartialView("Partial/_BlockWorkflowConfig", await _blockWorkflowFacade.Config(blockId, workflowId));
        }

        [HttpPost]
        public async Task<IActionResult> EditUserTask(UserTaskEditDTO dto)
        {
            try
            {
                await _blockWorkflowFacade.EditUserTask(dto);
                return Ok();
            }
            catch
            {
                return BadRequest("Editace bloku selhala.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditServiceTask(ServiceTaskEditDTO dto)
        {
            try
            {
                await _blockWorkflowFacade.EditServiceTask(dto);
                return Ok();
            }
            catch
            {
                return BadRequest("Editace bloku selhala.");
            }
        }
    }
}
