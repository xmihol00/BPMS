using System.Text.Json;
using BPMS_BL.Facades;
using BPMS_DAL.Entities;
using BPMS_DAL.Sharing;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace BPMS.Controllers
{
    public class CommunicationController : Controller
    {
        private readonly CommunicationFacade _communicationFacade;

        public CommunicationController(CommunicationFacade communicationFacade)
        {
            _communicationFacade = communicationFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);
            
             _communicationFacade.AuthorizeSystem(Request.Headers.Authorization);
        }

        [HttpPost]
        public async Task<IActionResult> ShareImport([FromBody] ModelDetailShare dto)
        {
            return Ok(await _communicationFacade.ShareImport(dto));
        }

        [HttpPost]
        public async Task<IActionResult> IsModelRunable([FromBody] ModelIdDTO dto)
        {
            return Ok(await _communicationFacade.IsModelRunable(dto));
        }

        [HttpPost]
        public async Task<IActionResult> RunModel([FromBody] ModelIdDTO dto)
        {
            return Ok(await _communicationFacade.RunModel(dto));
        }
    }
}