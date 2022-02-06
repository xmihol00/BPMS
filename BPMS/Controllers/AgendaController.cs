﻿using BPMS_BL.Facades;
using BPMS_DTOs.Agenda;
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
                    header = await this.RenderViewAsync("Partial/_AgendaDetailHeader", dto.Id, true),
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
    }
}
