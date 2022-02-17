using System.Text.Json;
using BPMS_BL.Facades;
using BPMS_DAL.Entities;
using BPMS_DAL.Sharing;
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
        public async Task<IActionResult> ShareImport(ModelDetailShare dto)
        {
            return Ok(await _communicationFacade.ShareImport(dto, Request.Headers.Authorization));
        }
    }
}