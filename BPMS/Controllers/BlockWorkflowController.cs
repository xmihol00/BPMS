using BPMS_BL.Facades;
using BPMS_DTOs.DataSchema;
using BPMS_DTOs.BlockWorkflow;
using Microsoft.AspNetCore.Mvc;
using BPMS_DTOs.Attribute;
using Microsoft.AspNetCore.Authorization;
using BPMS_DTOs.BlockWorkflow.EditTypes;

namespace BPMS.Controllers
{
    [Authorize("Admin, WorkflowKeeper")]
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
            await _blockWorkflowFacade.EditUserTask(dto);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> EditServiceTask(ServiceTaskEditDTO dto)
        {
            await _blockWorkflowFacade.EditServiceTask(dto);
            return Ok();
        }
    }
}
