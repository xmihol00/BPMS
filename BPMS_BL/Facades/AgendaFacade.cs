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
        private readonly IMapper _mapper;

        public async Task<AgendaOverviewDTO> Overview()
        {
            return new AgendaOverviewDTO()
            {
                Agendas = await _agendaRepository.All()
            };
        }

        public AgendaFacade(AgendaRepository agendaRepository, UserRepository userRepository, IMapper mapper)
        {
            _agendaRepository = agendaRepository;
            _userRepository = userRepository;
            _mapper = mapper;
        }

        public Task<List<UserIdNameDTO>> CreateModal() => _userRepository.CreateModal();

        public async Task Create(AgendaCreateDTO dto)
        {
            await _agendaRepository.Create(_mapper.Map<AgendaEntity>(dto));
            await _agendaRepository.Save();
        }

        public Task<AgendaDetailDTO> Detail(Guid id)
        {
            return _agendaRepository.Detail(id);
        }
    }
}
