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
            return PartialView("Partial/_AgendaRoles", await _agendaFacade.AddRole(dto));
        }

        [HttpGet]
        [Route("/Agenda/MissingInRole/{agendaId}/{roleId}")]
        public async Task<IActionResult> MissingInRole(Guid agendaId, Guid roleId)
        {
            return PartialView("Partial/_AgendaRoleNewUser", await _agendaFacade.MissingInRole(agendaId, roleId));
        }

        [HttpPost]
        [Route("/Agenda/AddUserRole/{userId}/{agendaId}/{roleId}")]
        public async Task<IActionResult> AddUserRole(Guid userId, Guid agendaId, Guid roleId)
        {
            await _agendaFacade.AddUserRole(userId, agendaId, roleId);
            return Ok();
        }

        [HttpPost]
        [Route("/Agenda/RemoveRole/{agendaId}/{roleId}")]
        public async Task<IActionResult> RemoveRole(Guid agendaId, Guid roleId)
        {
            await _agendaFacade.RemoveRole(agendaId, roleId);
            return Ok();
        }

        [HttpPost]
        [Route("/Agenda/RemoveUserRole/{userId}/{agendaId}/{roleId}")]
        public async Task<IActionResult> RemoveUserRole(Guid userId, Guid agendaId, Guid roleId)
        {
            await _agendaFacade.RemoveUserRole(userId, agendaId, roleId);
            return Ok();
        }
    }
}
