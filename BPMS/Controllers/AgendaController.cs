using BPMS_BL.Facades;
using BPMS_BL.Helpers;
using BPMS_Common.Enums;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Model;
using BPMS_DTOs.Role;
using BPMS_DTOs.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;

namespace BPMS.Controllers
{
    [Authorize]
    public class AgendaController : BaseController
    {
        private readonly AgendaFacade _agendaFacade;

        public AgendaController(AgendaFacade agendaFacade)
        : base(agendaFacade)
        {
            _agendaFacade = agendaFacade;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            _agendaFacade.SetFilters(_filters);
        }

        [HttpGet]
        public async Task<IActionResult> Overview()
        {
            return View("AgendaOverview", await _agendaFacade.Overview());
        }

        [HttpGet]
        public async Task<IActionResult> OverviewPartial()
        {
            Task<string> header = this.RenderViewAsync("Partial/_AgendaOverviewHeader", true);
            Task<string> filters = this.RenderViewAsync("Partial/_OverviewFilters", "Agenda", true);
            return Ok(new { header = await header, filters = await filters });
        }

        [HttpPost]
        public async Task<IActionResult> Filter(FilterDTO dto)
        {
            try
            {
                CookieHelper.SetCookie(dto.Filter, dto.Removed, HttpContext.Response);
                return PartialView("Partial/_AgendaOverview", await _agendaFacade.Filter(dto));
            }
            catch
            {
                return BadRequest("Filtrování selhalo.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(Guid id)
        {
            return View("AgendaDetail", await _agendaFacade.Detail(id));
        }

        [HttpGet]
        public async Task<IActionResult> DetailPartial(Guid id)
        {
            try
            {
                AgendaDetailPartialDTO dto = await _agendaFacade.DetailPartial(id);
                Task<string> detail = this.RenderViewAsync("Partial/_AgendaDetail", dto, true);
                Task<string> header = this.RenderViewAsync("Partial/_AgendaDetailHeader", dto, true);
                return Ok(new { detail = await detail, header = await header, activeBlocks = dto.ActiveBlocks });
            }
            catch
            {
                return BadRequest("Agendu se nepodařilo nalézt.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create()
        {
            return PartialView("Partial/_AgendaCreate", await _agendaFacade.Create());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(AgendaCreateDTO dto)
        {
            return Redirect($"/Agenda/Detail/{await _agendaFacade.Create(dto)}");
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> Edit(AgendaEditDTO dto)
        {
            try
            {
                AgendaInfoCardDTO infoCard = await _agendaFacade.Edit(dto);
                Task<string> info = this.RenderViewAsync("Partial/_AgendaDetailInfo", infoCard, true);
                Task<string> card = this.RenderViewAsync("Partial/_AgendaCard", (infoCard.SelectedAgenda, true), true);
                return Ok(new { info = await info, card = await card });
            }
            catch
            {
                return BadRequest("Editace agendy selhala.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> AddRole(Guid id)
        {
            return PartialView("Partial/_AgendaAddRole", await _agendaFacade.AddRole(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> AddRole(RoleAddDTO dto)
        {
            try
            {
                return PartialView("Partial/_AgendaRoles", await _agendaFacade.AddRole(dto));
            }
            catch
            {
                return BadRequest("Přidání role do agendy selhalo.");
            }
        }

        [HttpGet]
        [Route("/Agenda/MissingInRole/{agendaId}/{roleId}")]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> MissingInRole(Guid agendaId, Guid roleId)
        {
            return PartialView("Partial/_AgendaRoleNewUser", await _agendaFacade.MissingInRole(agendaId, roleId));
        }

        [HttpPost]
        [Route("/Agenda/AddUserRole/{userId}/{agendaRoleId}")]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> AddUserRole(Guid userId, Guid agendaRoleId)
        {
            try
            {
                await _agendaFacade.AddUserRole(userId, agendaRoleId);
                return Ok();
            }
            catch
            {
                return BadRequest("Přiřazení řešitelské roli uživateli selhalo.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> RemoveAgendaRole(Guid id)
        {
            try
            {
                await _agendaFacade.RemoveAgendaRole(id);
                return Ok();
            }
            catch
            {
                return BadRequest("Odebrání role z agendy selhalo.");
            }
        }

        [HttpPost]
        [Route("/Agenda/RemoveUserRole/{userId}/{agendaRoleId}")]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> RemoveUserRole(Guid userId, Guid agendaRoleId)
        {
            try
            {
                await _agendaFacade.RemoveUserRole(userId, agendaRoleId);
                return Ok();
            }
            catch
            {
                return BadRequest("Odebrání řešitelské role uživateli selhalo.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> AddSystem(Guid id)
        {            
            return PartialView("Partial/_AgendaAddSystem", await _agendaFacade.MissingSystems(id));
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> AddSystem(SystemAddDTO dto)
        {            
            try
            {
                return PartialView("Partial/_AgendaSystems", await _agendaFacade.AddSystem(dto));
            }
            catch
            {
                return BadRequest("Přidání spolupracujícího systému do agendy selhalo.");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        [Route("/Agenda/RemoveSystem/{agendaId}/{systemId}")]
        public async Task<IActionResult> RemoveSystem(Guid agendaId, Guid systemId)
        {    
            try
            {
                await _agendaFacade.RemoveSystem(agendaId, systemId);
                return Ok();
            }
            catch
            {
                return BadRequest("Odebrání spolupracujícího systému z agendy selhalo.");
            }
        }


        [HttpGet]
        [Authorize(Roles = "Admin, AgendaKeeper")]
        public async Task<IActionResult> UploadModel()
        {
            return PartialView("Partial/_AgendaSelect", await _agendaFacade.UploadModel());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Keepers(Guid id)
        {
            return PartialView("Partial/_AgendaAdminChange", await _agendaFacade.Keepers(id));
        }
    }
}
