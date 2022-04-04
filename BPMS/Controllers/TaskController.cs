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
    [Authorize]
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

            _taskFacade.SetFilters(_filters);
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            try
            {
                CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
                return PartialView("Partial/_TaskOverview", await _taskFacade.Filter(dto));
            }
            catch
            {
                return BadRequest("Filtrování selhalo.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("TaskOverview", await _taskFacade.Overview(_userId));
        }

        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            Task<string> header = this.RenderViewAsync("Partial/_TaskOverviewHeader", true);
            Task<string> filters = this.RenderViewAsync("Partial/_OverviewFilters", "Task", true);
            return Ok(new { header = await header, filters = await filters });
        }

        [HttpGet]
        public async Task<IActionResult> UserDetail(Guid id)
        {
            return View("UserTaskDetail", await _taskFacade.UserTaskDetail(id));
        }

        [HttpGet]
        public async Task<IActionResult> UserDetailPartial(Guid id)
        {
            try
            {
                UserTaskDetailPartialDTO dto = await _taskFacade.UserTaskDetailPartial(id);
                Task<string> detail = this.RenderViewAsync("Partial/_UserTaskDetail", dto, true);
                Task<string> header = this.RenderViewAsync("Partial/_UserTaskDetailHeader", dto, true);
                return Ok(new { detail = await detail, header = await header });
            }
            catch
            {
                return BadRequest("Úkol se nepodařilo najít.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ServiceDetail(Guid id)
        {
            return View("ServiceTaskDetail", await _taskFacade.ServiceTaskDetail(id));
        }

        [HttpGet]
        public async Task<IActionResult> ServiceDetailPartial(Guid id)
        {
            try
            {
                ServiceTaskDetailPartialDTO dto = await _taskFacade.ServiceTaskDetailPartial(id);
                Task<string> detail = this.RenderViewAsync("Partial/_ServiceTaskDetail", dto, true);
                Task<string> header = this.RenderViewAsync("Partial/_ServiceTaskDetailHeader", dto, true);
                return Ok(new { detail = await detail, header = await header });
            }
            catch
            {
                return BadRequest("Úkol se nepodařilo najít.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveUserTask(IFormCollection data)
        {
            return PartialView("Partial/_UserTaskDetail", await _taskFacade.SaveUserTask(data, Request.Form.Files));
        }

        [HttpPost]
        public async Task<IActionResult> SaveServiceTask(IFormCollection data)
        {
            return PartialView("Partial/_ServiceTaskDetail", await _taskFacade.SaveServiceTask(data, Request.Form.Files));
        }

        [HttpPost]
        public async Task<IActionResult> SolveUserTask(IFormCollection data)
        {
            try
            {
                await _taskFacade.SolveTask(data, Request.Form.Files);
                return Ok();
            }
            catch
            {
                return BadRequest("Úkol se nepodařilo vyřešit.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> SolveServiceTask(IFormCollection data)
        {
            await _taskFacade.SolveTask(data, Request.Form.Files, BlockWorkflowStateEnum.SolvedByUser);
            return Redirect("/Task/Overview");
        }

        [HttpPost]
        public async Task<IActionResult> CallService(IFormCollection data)
        {
            try
            {
                return PartialView("Partial/_ServiceTaskDetail", await _taskFacade.CallService(data, Request.Form.Files));
            }
            catch
            {
                return BadRequest("Volání webové služby selhalo.");
            }
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
            try
            {
                return PartialView($"Partial/_TaskData{type}", await _taskFacade.AddToArray(taskDataId, type));
            }
            catch
            {
                return BadRequest("Do pole se nepodařilo přidat další element.");
            }
        }


        [HttpPost]
        [Authorize(Roles = "Admin, WorkflowKeeper")]
        public async Task<IActionResult> Resend(Guid id)
        {
            try
            {
                await _taskFacade.Resend(id);
                return Ok();
            }
            catch
            {
                return BadRequest("Znovu odeslání zprávy selhalo.");
            }
        }
    }
}
