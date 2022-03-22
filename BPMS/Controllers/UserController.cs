using BPMS_BL.Facades;
using BPMS_BL.Helpers;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Header;
using BPMS_DTOs.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;

namespace BPMS.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        private readonly UserFacade _userFacade;

        public UserController(UserFacade userFacade)
        : base(userFacade)
        {
            _userFacade = userFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _userFacade.SetFilters(_filters);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
            return PartialView("Partial/_UserOverview", await _userFacade.FilterUsers(dto));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Overview()
        {
            return View("UserOverview", await _userFacade.Overview());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> OverviewPartial()
        {
            return Ok(new
            {
                header = await this.RenderViewAsync("Partial/_UserOverviewHeader", true),
                filters = await this.RenderViewAsync("Partial/_OverviewFilters", "User", true)
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("UserDetail", await _userFacade.Detail(id));
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            UserDetailPartialDTO dto = await _userFacade.DetailPartial(id);

            return Ok(new
            {
                detail = await this.RenderViewAsync("Partial/_UserDetail", dto, true),
                header = await this.RenderViewAsync("Partial/_UserDetailHeader", dto, true),
                activeBlocks = dto.ActiveBlocks
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(UserCreateEditDTO dto)
        {
            UserInfoCardDTO infoCard = await _userFacade.Edit(dto);
            return Ok(new
            {
                info = await this.RenderViewAsync("Partial/_UserDetailInfo", infoCard, true),
                card = await this.RenderViewAsync("Partial/_UserCard", (infoCard.SelectedUser, true), true),
            });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(UserCreateEditDTO dto)
        {
            return Redirect($"/User/Detail/{await _userFacade.Create(dto)}");
        }

        [HttpGet]
        public async Task<IActionResult> MyDetail()
        {
            return View("UserMyDetail", await _userFacade.MyDetail());
        }

        [HttpPost]
        public async Task<IActionResult> PwdChange(UserPasswordChangeDTO dto)
        {
            await _userFacade.ChangePassword(dto);
            return Ok();
        }
    }
}
