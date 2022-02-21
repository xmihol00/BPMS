using BPMS_BL.Facades;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Model;
using BPMS_DTOs.Role;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BPMS.Controllers
{
    [Authorize]
    public class AgendaController : BaseController
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
        [Route("/Agenda/AddUserRole/{userId}/{agendaRoleId}")]
        public async Task<IActionResult> AddUserRole(Guid userId, Guid agendaRoleId)
        {
            await _agendaFacade.AddUserRole(userId, agendaRoleId);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAgendaRole(Guid id)
        {
            await _agendaFacade.RemoveAgendaRole(id);
            return Ok();
        }

        [HttpPost]
        [Route("/Agenda/RemoveUserRole/{userId}/{agendaRoleId}")]
        public async Task<IActionResult> RemoveUserRole(Guid userId, Guid agendaRoleId)
        {
            await _agendaFacade.RemoveUserRole(userId, agendaRoleId);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> AddSystem(Guid id)
        {            
            return PartialView("Partial/_AgendaAddSystem", await _agendaFacade.MissingSystems(id));
        }

        [HttpPost]
        public async Task<IActionResult> AddSystem(SystemAddDTO dto)
        {            
            return PartialView("Partial/_AgendaSystems", await _agendaFacade.AddSystem(dto));
        }

        [HttpPost]
        [Route("/Agenda/RemoveSystem/{agendaId}/{systemId}")]
        public async Task<IActionResult> RemoveSystem(Guid agendaId, Guid systemId)
        {    
            await _agendaFacade.RemoveSystem(agendaId, systemId);
            return Ok();
        }
    }
}
