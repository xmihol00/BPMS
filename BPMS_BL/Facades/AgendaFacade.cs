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
using BPMS_DTOs.System;
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
        private readonly BlockModelRepository _blockModelRepository;
        private readonly SystemRepository _systemRepository;
        private readonly SystemAgendaRepository _systemAgendaRepository;
        private readonly IMapper _mapper;

        public AgendaFacade(AgendaRepository agendaRepository, UserRepository userRepository, ModelRepository modelRepository, 
                            SolvingRoleRepository solvingRoleRepository, AgendaRoleUserRepository agendaRoleUserRepository, 
                            BlockModelRepository blockModelRepository, SystemRepository systemRepository,
                            SystemAgendaRepository systemAgendaRepository, IMapper mapper)
        {
            _agendaRepository = agendaRepository;
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _solvingRoleRepository = solvingRoleRepository;
            _agendaRoleUserRepository = agendaRoleUserRepository;
            _blockModelRepository = blockModelRepository;
            _systemRepository = systemRepository;
            _systemAgendaRepository = systemAgendaRepository;
            _mapper = mapper;
        }

        public async Task<AgendaDetailDTO> Detail(Guid id)
        {
            AgendaDetailDTO dto = await _agendaRepository.Detail(id);
            dto.Models = await _modelRepository.OfAgenda(id);
            dto.AllAgendas = await _agendaRepository.All();
            dto.Roles = await _solvingRoleRepository.Roles(id);
            dto.Systems = await _agendaRepository.Systems(id);
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

        public async Task AddUserRole(Guid userId, Guid agendaId, Guid roleId)
        {
            await _agendaRoleUserRepository.Create(new AgendaRoleUserEntity
            {
                AgendaId = agendaId,
                RoleId = roleId,
                UserId = userId
            });

            await _agendaRoleUserRepository.Save();
        }

        public async Task RemoveUserRole(Guid userId, Guid agendaId, Guid roleId)
        {
            _agendaRoleUserRepository.Remove(await _agendaRoleUserRepository.RoleForRemoval(userId, agendaId, roleId));

            await _agendaRoleUserRepository.Save();
        }

        public async Task RemoveSystem(Guid agendaId, Guid systemId)
        {
            _systemAgendaRepository.Remove(new SystemAgendaEntity 
            {
                AgendaId = agendaId,
                SystemId = systemId
            });

            await _systemAgendaRepository.Save();
        }

        public Task<List<SystemAllDTO>> MissingSystems(Guid agendaId)
        {
            return _systemRepository.NotInAgenda(agendaId); 
        }

        public async Task<List<SystemAllDTO>> AddSystem(SystemAddDTO dto)
        {
            await _systemAgendaRepository.Create(new SystemAgendaEntity
            {
                AgendaId = dto.TargetId,
                SystemId = dto.SystemId
            });

            await _systemAgendaRepository.Save();

            return await _agendaRepository.Systems(dto.TargetId); 
        }

        public async Task RemoveRole(Guid agendaId, Guid roleId)
        {
            foreach (AgendaRoleUserEntity role in await _agendaRoleUserRepository.ForRemoval(agendaId, roleId))
            {
                _agendaRoleUserRepository.Remove(role);
            }

            foreach (UserTaskModelEntity task in await _blockModelRepository.RolesForRemovalUserTaks(roleId, agendaId))
            {
                task.RoleId = null;
            }

            foreach (ServiceTaskModelEntity task in await _blockModelRepository.RolesForRemovalServiceTaks(roleId, agendaId))
            {
                task.RoleId = null;
            }

            await _agendaRoleUserRepository.Save();
        }

        public Task<List<UserIdNameDTO>> MissingInRole(Guid agendaId, Guid roleId)
        {
            return _userRepository.MissingInRole(agendaId, roleId);
        }

        public async Task<AgendaDetailPartialDTO> DetailPartial(Guid id)
        {
            AgendaDetailPartialDTO detail = await _agendaRepository.DetailPartial(id);
            detail.Models = await _modelRepository.OfAgenda(id);
            detail.Roles = await _solvingRoleRepository.Roles(id);

            return detail;
        }

        public async Task<List<RoleAllDTO>> AddRole(Guid agendaId)
        {
            List<RoleAllDTO> roles = new List<RoleAllDTO>{
                new RoleAllDTO
                {
                    Id = null,
                    Name = "Nevybrána"
                }
            };
            roles.AddRange(await _solvingRoleRepository.AllNotInAgenda(agendaId));

            return roles;
        }

        public async Task<List<RoleDetailDTO>> AddRole(RoleAddDTO dto)
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

            return await _solvingRoleRepository.Roles(dto.AgendaId);
        }
    }
}
