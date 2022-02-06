﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Model;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class AgendaFacade
    {
        private readonly AgendaRepository _agendaRepository;
        private readonly UserRepository _userRepository;
        private readonly ModelRepository _modelRepository;
        private readonly IMapper _mapper;

        public AgendaFacade(AgendaRepository agendaRepository, UserRepository userRepository, ModelRepository modelRepository, 
                            IMapper mapper)
        {
            _agendaRepository = agendaRepository;
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _mapper = mapper;
        }

        public async Task<AgendaDetailDTO> Detail(Guid id)
        {
            AgendaDetailDTO dto = await _agendaRepository.Detail(id);
            dto.Models = await _modelRepository.OfAgenda(id);
            dto.AllAgendas = await _agendaRepository.All();
            return dto;
        }

        public async Task<AgendaOverviewDTO> Overview()
        {
            return new AgendaOverviewDTO()
            {
                Agendas = await _agendaRepository.All()
            };
        }

        public Task<List<UserIdNameDTO>> CreateModal() => _userRepository.CreateModal();

        public async Task Create(AgendaCreateDTO dto)
        {
            await _agendaRepository.Create(_mapper.Map<AgendaEntity>(dto));
            await _agendaRepository.Save();
        }

        public async Task<AgendaDetailPartialDTO> DetailPartial(Guid id)
        {
            AgendaDetailPartialDTO detail = await _agendaRepository.DetailPartial(id);
            detail.Models = await _modelRepository.OfAgenda(id);

            return detail;
        }
    }
}
