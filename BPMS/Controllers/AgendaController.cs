using BPMS_BL.Facades;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Model;
using BPMS_DTOs.Role;
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

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("AgendaOverview", await _agendaFacade.Overview());
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("AgendaDetail", await _agendaFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            AgendaDetailPartialDTO dto = await _agendaFacade.DetailPartial(id);

            return Ok(new
                {
                    detail = await this.RenderViewAsync("Partial/_AgendaDetail", dto, true),
                    header = await this.RenderViewAsync("Partial/_AgendaDetailHeader", dto, true),
                });
        }

        [HttpGet]
        public async Task<IActionResult> CreateModal()
        {
            return PartialView("Partial/_AgendaCreateModal", await _agendaFacade.CreateModal());
        }

        [HttpPost]
        public async Task<IActionResult> Create(AgendaCreateDTO dto)
        {
            await _agendaFacade.Create(dto);
            return Redirect("/Agenda/Overview");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AgendaEditDTO dto)
        {
            return PartialView("Partial/_AgendaInfo", await _agendaFacade.Edit(dto));
        }

        [HttpGet]
        public async Task<IActionResult> AddRole(Guid id)
        {
            return PartialView("Partial/_AgendaAddRole", await _agendaFacade.AddRole(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddRole(RoleAddDTO dto)
        {
            await _agendaFacade.AddRole(dto);
            return Ok(); // TODO
        }
    }
}
