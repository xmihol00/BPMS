﻿using System.Security.Claims;
using BPMS_BL.Facades;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BPMS.Controllers
{
    public class BaseController : Controller
    {
        protected Guid _userId;
        protected readonly BaseFacade _baseFacade;
        protected bool[] _filters = new bool[Enum.GetValues<FilterTypeEnum>().Count()];

        public BaseController(BaseFacade baseFacade)
        {
            _baseFacade = baseFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            ClaimsPrincipal user = HttpContext.User;
            ViewBag.Signed = user.Identity.IsAuthenticated;
            if (!ViewBag.Signed)
            {
                return;
            }

            ViewBag.Controller = this.GetType().Name;

            ViewBag.UserName = user.Identity.Name;
            ViewBag.Id = Guid.Empty;

            ViewData[SystemRoleEnum.Admin.ToString()] = false;
            ViewData[SystemRoleEnum.AgendaKeeper.ToString()] = false;
            ViewData[SystemRoleEnum.WorkflowKeeper.ToString()] = false;
            ViewData[SystemRoleEnum.ServiceKeeper.ToString()] = false;
            ViewData[SystemRoleEnum.ModelKeeper.ToString()] = false;
            
            foreach(Claim claim in user.Claims)
            {
                if (claim.Type == ClaimTypes.Role)
                {
                    ViewData[claim.Value] = true;
                }
                else if (claim.Type == ClaimTypes.NameIdentifier)
                {
                    _userId = Guid.Parse(claim.Value);
                    _baseFacade.UserId = _userId;
                    ViewBag.Id = _userId;
                }                    
            }

            foreach (FilterTypeEnum value in Enum.GetValues<FilterTypeEnum>())
            {
                _filters[((int)value)] = context.HttpContext.Request.Cookies[value.ToString()] != null;
            }

            ViewBag.Filters = _filters;
        }
    }
}
