using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Model;
using BPMS_DTOs.Role;
using BPMS_DTOs.User;
using Microsoft.EntityFrameworkCore.Storage;

namespace BPMS_BL.Facades
{
    public class AgendaFacade
    {
        private readonly AgendaRepository _agendaRepository;
        private readonly UserRepository _userRepository;
        private readonly ModelRepository _modelRepository;
        private readonly SolvingRoleRepository _solvingRoleRepository;
        private readonly AgendaRoleUserRepository _agendaRoleUserRepository;
        private readonly IMapper _mapper;

        public AgendaFacade(AgendaRepository agendaRepository, UserRepository userRepository, ModelRepository modelRepository, 
                            SolvingRoleRepository solvingRoleRepository, AgendaRoleUserRepository agendaRoleUserRepository, 
                            IMapper mapper)
        {
            _agendaRepository = agendaRepository;
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _solvingRoleRepository = solvingRoleRepository;
            _agendaRoleUserRepository = agendaRoleUserRepository;
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

        public async Task<AgendaDetailPartialDTO> Edit(AgendaEditDTO dto)
        {
            AgendaEntity agenda = await _agendaRepository.DetailBase(dto.Id);
            agenda.Name = dto.Name;
            agenda.Description = dto.Description ?? "";
            await _agendaRepository.Save();

            return _mapper.Map<AgendaDetailPartialDTO>(agenda);
        }

        public async Task<AgendaDetailPartialDTO> DetailPartial(Guid id)
        {
            AgendaDetailPartialDTO detail = await _agendaRepository.DetailPartial(id);
            detail.Models = await _modelRepository.OfAgenda(id);

            return detail;
        }

        public async Task<List<RoleAllDTO>> AddRole(Guid agendaId)
        {
            List<RoleAllDTO> roles = await _solvingRoleRepository.AllNotInAgenda(agendaId);
            roles.Add(new RoleAllDTO
            {
                Id = null,
                Name = "Nevybrána"
            });

            return roles;
        }

        public async Task AddRole(RoleAddDTO dto)
        {
            IDbContextTransaction transaction = await _agendaRepository.CreateTransaction();
            if (dto.Id == null)
            {
                SolvingRoleEntity role = new SolvingRoleEntity
                {
                    Name = dto.Name,
                    Description = dto.Description ?? "",
                };

                await _solvingRoleRepository.Create(role);
                await _solvingRoleRepository.Save();
                dto.Id = role.Id;
            }
            
            await _agendaRoleUserRepository.Create(new AgendaRoleUserEntity
            {
                AgendaId = dto.AgendaId,
                RoleId = dto.Id,
            });

            await _agendaRoleUserRepository.Save();
            await transaction.CommitAsync();
        }
    }
}
