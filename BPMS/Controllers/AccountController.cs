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

namespace BPMS.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserFacade _userFacade;

        public AccountController(UserFacade userFacade)
        {
            _userFacade = userFacade;
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
                (ClaimsPrincipal principal, AuthenticationProperties authProperties) = await _userFacade.Authenticate(model.UserName, model.Password);
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
    }
}
