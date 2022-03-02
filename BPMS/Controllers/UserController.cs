using BPMS_BL.Facades;
using BPMS_DTOs.Header;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly UserFacade _userFacade;

        public UserController(UserFacade userFacade)
        {
            _userFacade = userFacade;
        }

        public async Task<IActionResult> Overview()
        {
            return View("UserOverview", await _userFacade.Overview());
        }
    }
}
