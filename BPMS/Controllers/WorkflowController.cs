using BPMS_BL.Facades;
using BPMS_DTOs.Header;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class WorkflowController : BaseController
    {
        private readonly WorkflowFacade _workflowFacade;

        public WorkflowController(WorkflowFacade workflowFacade)
        {
            _workflowFacade = workflowFacade;
        }
    }
}