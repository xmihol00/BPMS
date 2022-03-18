using BPMS_BL.Facades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BPMS_DTOs.Filter;
using BPMS_BL.Helpers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BPMS.Controllers
{
    [Authorize]
    public class NotificationController : BaseController
    {
        private readonly NotificationFacade _notificationFacade;

        public NotificationController(NotificationFacade notificationFacade)
        : base(notificationFacade)
        {
            _notificationFacade = notificationFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _notificationFacade.SetFilters(_filters);
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
            return PartialView("Partial/_NotificationAll", await _notificationFacade.Filter(dto));
        }

        [HttpGet]
        public async Task<IActionResult> All()
        {
            return PartialView("Partial/_NotificationAll", await _notificationFacade.All());
        }

        [HttpPost]
        public async Task<IActionResult> Seen(Guid id)
        {
            return PartialView("Partial/_NotificationAll", await _notificationFacade.Seen(id));
        }

        [HttpPost]
        [Route("/Notification/Mark/{id}/{marked}")]
        public async Task<IActionResult> Mark(Guid id, bool marked)
        {
            await _notificationFacade.Mark(id, marked);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> Remove(Guid id)
        {
            await _notificationFacade.Remove(id);
            return Ok();
        }
    }
}
