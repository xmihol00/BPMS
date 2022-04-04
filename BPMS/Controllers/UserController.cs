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
            try
            {
                CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
                return PartialView("Partial/_UserOverview", await _userFacade.FilterUsers(dto));
            }
            catch
            {
                return BadRequest("Filtrování selhalo.");
            }
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
            Task<string> header = this.RenderViewAsync("Partial/_UserOverviewHeader", true);
            Task<string> filters = this.RenderViewAsync("Partial/_OverviewFilters", "User", true);
            return Ok(new { header = await header, filters = await filters });
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
            try
            {
                UserDetailPartialDTO dto = await _userFacade.DetailPartial(id);
                Task<string> detail = this.RenderViewAsync("Partial/_UserDetail", dto, true);
                Task<string> header = this.RenderViewAsync("Partial/_UserDetailHeader", dto, true);
                return Ok(new { detail = await detail, header = await header, activeBlocks = dto.ActiveBlocks });
            }
            catch
            {
                return BadRequest("Uživatel nebyl nalezen.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(UserCreateEditDTO dto)
        {
            try
            {
                UserInfoCardDTO infoCard = await _userFacade.Edit(dto);
                Task<string> info = this.RenderViewAsync("Partial/_UserDetailInfo", infoCard, true);
                Task<string> card = this.RenderViewAsync("Partial/_UserCard", (infoCard.SelectedUser, true), true);
                return Ok(new { info = await info, card = await card });
            }
            catch
            {
                return BadRequest("Editace uživatele selhala.");
            }
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
            try
            {
                await _userFacade.ChangePassword(dto);
                return Ok();
            }
            catch
            {
                return BadRequest("Změna hesla selhala.");
            }
        }
    }
}
