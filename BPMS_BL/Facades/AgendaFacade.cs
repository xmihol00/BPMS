using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Model;
using BPMS_DTOs.User;

namespace BPMS_BL.Facades
{
    public class AgendaFacade
    {
        private readonly AgendaRepository _agendaRepository;
        private readonly UserRepository _userRepository;

        public AgendaFacade(AgendaRepository agendaRepository, UserRepository userRepository)
        {
            _agendaRepository = agendaRepository;
            _userRepository = userRepository;
        }

        public Task<List<UserIdNameDTO>> CreateModal() => _userRepository.CreateModal();
    }
}
