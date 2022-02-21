using BPMS_BL.Facades;
using BPMS_DTOs.Header;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class TaskController : BaseController
    {
        private readonly TaskFacade _taskFacade;
        private Guid UserId;

        public TaskController(TaskFacade taskFacade)
        {
            _taskFacade = taskFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            UserId = ViewBag.Id;
        }

        public async Task<IActionResult> Overview()
        {
            return View("TaskOverview", await _taskFacade.Overview(UserId));
        }
    }
}
