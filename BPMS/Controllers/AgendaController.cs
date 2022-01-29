using BPMS_BL.Facades;
using BPMS_DTOs.Model;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    public class AgendaController : Controller
    {
        private readonly AgendaFacade _agendaFacade;

        public AgendaController(AgendaFacade agendaFacade)
        {
            _agendaFacade = agendaFacade;
        }

        public async Task<IActionResult> Overview(CreateModelDTO dto)
        {
            return View("AgendaOverview");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(CreateModelDTO dto)
        {
            return View();
        }
    }
}
