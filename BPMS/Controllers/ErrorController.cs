using System.Threading.Tasks;
using BPMS_BL.Facades;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    public class ErrorController : BaseController
    {
        public ErrorController(BaseFacade baseFacade) : base(baseFacade)
        {
        }

        [HttpGet]
        [Route("/Error/{statusCode}")]
        public IActionResult Error(int statusCode)
        {
            switch (statusCode)
            {
                case 404:
                case 405:
                    return View("Error", "Tato stránka neexistuje.");
                
                default:
                    return View("Error", "Omlouváme se, došlo k chybě.");

            }
        }
    }
}
