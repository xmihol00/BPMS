using BPMS_BL.Facades;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System;
using BPMS_Common;

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
            return View("UserTaskDetail", await _taskFacade.UserTaskDetail(id, _userId));
        }

        [HttpGet]
        public async Task<IActionResult> ServiceDetail(Guid id)
        {
            return View("ServiceTaskDetail", await _taskFacade.ServiceTaskDetail(id, _userId));
        }

        [HttpPost]
        public async Task<IActionResult> SaveData(IFormCollection data)
        {
            return PartialView("Partial/_UserTaskDetail", await _taskFacade.SaveData(data, Request.Form.Files, _userId));
        }

        [HttpPost]
        public async Task<IActionResult> SolveUserTask(IFormCollection data)
        {
            await _taskFacade.SolveUserTask(data, Request.Form.Files);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            FileDownloadDTO file = await _taskFacade.DownloadFile(id);
            return File(file.Data, file.MIMEType, file.FileName);
        }
    }
}
