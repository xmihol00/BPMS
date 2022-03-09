using BPMS_BL.Facades;
using BPMS_DTOs.Task;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System;
using BPMS_Common;
using BPMS_DTOs.Filter;
using BPMS_BL.Helpers;
using BPMS_Common.Enums;

namespace BPMS.Controllers
{
    [Authorize("Admin, TaskSolver")]
    public class TaskController : BaseController
    {
        private readonly TaskFacade _taskFacade;
        

        public TaskController(TaskFacade taskFacade)
        : base(taskFacade)
        {
            _taskFacade = taskFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _taskFacade.SetFilters(_filters, _userId);
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
            return PartialView("Partial/_TaskOverview", await _taskFacade.Filter(dto));
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
        public async Task<IActionResult> UserDetailPartial(Guid id)
        {
            UserTaskDetailPartialDTO dto = await _taskFacade.UserTaskDetailPartial(id, _userId);
            return Ok(new
            {
                detail = await this.RenderViewAsync("Partial/_UserTaskDetail", dto, true),
                header = await this.RenderViewAsync("Partial/_UserTaskHeader", dto, true),
            });
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
            return File(file.Data ?? new byte[1], file.MIMEType ?? string.Empty, file.FileName);
        }

        [HttpPost]
        [Route("/Task/AddToArray/{taskDataId}/{type}")]
        public async Task<IActionResult> AddToArray(Guid taskDataId, DataTypeEnum type)
        {
            return PartialView($"Partial/_TaskData{type}", await _taskFacade.AddToArray(taskDataId, type));
        }
    }
}
