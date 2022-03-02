using BPMS_BL.Facades;
using BPMS_DTOs.ServiceDataSchema;
using BPMS_DTOs.BlockWorkflow;
using Microsoft.AspNetCore.Mvc;
using BPMS_DTOs.BlockAttribute;
using Microsoft.AspNetCore.Authorization;

namespace BPMS.Controllers
{
    [Authorize]
    public class BlockWorkflowController : BaseController
    {
        private readonly BlockWorkflowFacade _blockWorkflowFacade;

        public BlockWorkflowController(BlockWorkflowFacade blockWorkflowFacade)
        {
            _blockWorkflowFacade = blockWorkflowFacade;
        }

        [HttpGet]
        public async Task<IActionResult> Config(Guid id)
        {
            return PartialView("Partial/_BlockWorkflowConfig", await _blockWorkflowFacade.Config(id));
        }
    }
}
