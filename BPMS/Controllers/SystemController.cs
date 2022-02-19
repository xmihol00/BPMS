using BPMS_BL.Facades;
using BPMS_DTOs.Header;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class SystemController : BaseController
    {
        private readonly SystemFacade _systemFacade;

        public SystemController(SystemFacade systemFacade)
        {
            _systemFacade = systemFacade;
        }
    }
}
