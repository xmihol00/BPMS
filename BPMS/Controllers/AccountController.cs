using System.Security.Claims;
using BPMS_BL.Facades;
using BPMS_DTOs.Account;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Model;
using BPMS_DTOs.Role;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BPMS.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserFacade _userFacade;

        public AccountController(UserFacade userFacade)
        : base(userFacade)
        {
            _userFacade = userFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _userFacade.SetFilters(_filters, _userId);
        }

        [HttpGet]
        public IActionResult SignIn(string ReturnUrl, string UserName)
        {
            if (ViewBag.Signed)
            {
                return Redirect(ReturnUrl ?? "/");
            }

            return View(new SignInDTO() { ReturnURL = ReturnUrl, UserName = UserName });
        }


        [HttpGet]
        public IActionResult Create(string ReturnUrl, string UserName)
        {
            if (ViewBag.Signed)
            {
                return Redirect(ReturnUrl ?? "/");
            }

            return View("AccountCreate", new SignInDTO() { ReturnURL = ReturnUrl, UserName = UserName });
        }

        [HttpPost]
        public async Task<IActionResult> SignIn(SignInDTO model)
        {
            try
            {
                (ClaimsPrincipal principal, AuthenticationProperties authProperties) = 
                    await _userFacade.Authenticate(model.UserName, model.Password, HttpContext.Response);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return Redirect(model.ReturnURL ?? "/");
            }
            catch
            {
                return Redirect($"/Account/SignIn?ReturnUrl={model?.ReturnURL ?? ""}&UserName={model?.UserName ?? ""}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AccountCreateDTO dto)
        {
            if (ViewBag.Signed)
            {
                return Redirect(dto.ReturnURL ?? "/");
            }

            try
            {
                (ClaimsPrincipal principal, AuthenticationProperties authProperties) = await _userFacade.Create(dto);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProperties);

                return Redirect(dto.ReturnURL ?? "/");
            }
            catch
            {
                return Redirect($"/Account/Create?ReturnUrl={dto.ReturnURL}&UserName={dto.UserName}");
            }
        }

        [HttpGet]
        [Route("/Account/SignOut")]
        public async Task<IActionResult> Signout()
        {
            await HttpContext.SignOutAsync();

            return Redirect("/Account/SignIn");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Notifications()
        {
            return PartialView("Partial/_NotificationAll", await _userFacade.AllNotifications());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> NotificationSeen(Guid id)
        {
            await _userFacade.NotificationSeen(id);
            return Ok();
        }

        [HttpPost]
        [Authorize]
        [Route("/Account/NotificationMark/{id}/{marked}")]
        public async Task<IActionResult> NotificationMark(Guid id, bool marked)
        {
            await _userFacade.NotificationMark(id, marked);
            return Ok();
        }
    }
}
