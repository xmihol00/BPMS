using System.Text.Json;
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
        public async Task<IActionResult> ShareImport([FromBody] JsonElement json)
        {
            return Ok(await _communicationFacade.ShareImport(json, Request.Headers.Authorization));
        }
    }
}
