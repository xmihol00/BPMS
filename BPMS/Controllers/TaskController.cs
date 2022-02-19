using BPMS_BL.Facades;
using BPMS_DTOs.Header;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class TaskController : BaseController
    {
        private readonly TaskFacade _taskFacade;

        public TaskController(TaskFacade taskFacade)
        {
            _taskFacade = taskFacade;
        }
    }
}
