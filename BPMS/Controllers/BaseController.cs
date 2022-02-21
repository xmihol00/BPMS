using System.Security.Claims;
using BPMS_Common.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BPMS.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
            ClaimsPrincipal user = HttpContext.User;

            ViewBag.Controller = this.GetType().Name;

            ViewBag.Signed = user.Identity.IsAuthenticated;
            ViewBag.Name = user.Identity.Name;
            ViewBag.Id = Guid.Empty;

            ViewBag.Admin = false;
            ViewBag.AgendaKeeper = false;
            ViewBag.WorkflowKeeper = false;
            ViewBag.ServiceKeeper = false;
            ViewBag.TaskSolver = false;

            List<Claim> claims = user.Claims?.ToList();
            if (claims != null)
            {
                foreach(Claim claim in claims)
                {
                    if (claim.Type == ClaimTypes.Role)
                    {
                        switch (Enum.Parse(typeof(SystemRoleEnum), claim.Value))
                        {
                            case SystemRoleEnum.Admin:
                                ViewBag.Admin = true;
                                break;
                            
                            case SystemRoleEnum.AgendaKeeper:
                                ViewBag.AgendaKeeper = true;
                                break;
                            
                            case SystemRoleEnum.WorkflowKeeper:
                                ViewBag.WorkflowKeeper = true;
                                break;
                            
                            case SystemRoleEnum.ServiceKeeper:
                                ViewBag.ServiceKeeper = true;
                                break;
                            
                            case SystemRoleEnum.TaskSolver:
                                ViewBag.TaskSolver = true;
                                break;
                        }
                    }
                    else if (claim.Type == ClaimTypes.NameIdentifier)
                    {
                        ViewBag.Id = Guid.Parse(claim.Value);
                    }                    
                }
            }
        }
    }
}
