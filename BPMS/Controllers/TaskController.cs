using BPMS_BL.Facades;
using BPMS_Common.Enums;
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
        private Guid _userId;

        public TaskController(TaskFacade taskFacade)
        {
            _taskFacade = taskFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _userId = ViewBag.Id;
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("TaskOverview", await _taskFacade.Overview(_userId));
        }

        [HttpGet]
        public async Task<IActionResult> UserDetail(Guid id)
        {
            return View("UserTaskDetail", await _taskFacade.UserDetail(id, _userId));
        }

        [HttpGet]
        public async Task<IActionResult> ServiceDetail(Guid id)
        {
            return View("ServiceTaskDetail", await _taskFacade.ServiceDetail(id, _userId));
        }

        public async Task<IActionResult> SaveData(IFormCollection data)
        {
            await _taskFacade.SaveData(data);
            return Ok();
        }
    }
}
