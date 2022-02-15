using BPMS_BL.Facades;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    public class CommunicationController : Controller
    {
        private readonly CommunicationFacade _communicationFacade;

        public CommunicationController(CommunicationFacade communicationFacade)
        {
            _communicationFacade = communicationFacade;
        }


        [HttpPost]
        public async Task<IActionResult> ShareImport()
        {
            using var reader = new StreamReader(Request.Body);
            string body = reader.ReadToEnd();
            string data = "";

            return Ok(await _communicationFacade.ShareImport(data, HttpContext.Request.Headers.Authorization));
        }
    }
}
