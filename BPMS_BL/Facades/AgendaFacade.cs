using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using AutoMapper;
using BPMS_BL.Helpers;
using BPMS_BL.Hubs;
using BPMS_Common.Enums;
using BPMS_Common.Helpers;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.Agenda;
using BPMS_DTOs.Filter;
using BPMS_DTOs.Model;
using BPMS_DTOs.Role;
using BPMS_DTOs.System;
using BPMS_DTOs.User;
using Microsoft.EntityFrameworkCore.Storage;

namespace BPMS_BL.Facades
{
    public class AgendaFacade : BaseFacade
    {
        private readonly AgendaRepository _agendaRepository;
        private readonly UserRepository _userRepository;
        private readonly ModelRepository _modelRepository;
        private readonly SolvingRoleRepository _solvingRoleRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;
        private readonly BlockModelRepository _blockModelRepository;
        private readonly SystemRepository _systemRepository;
        private readonly SystemAgendaRepository _systemAgendaRepository;
        private readonly UserRoleRepository _userRoleRepository;
        private readonly NotificationRepository _notificationRepository;
        private readonly IMapper _mapper;

        public AgendaFacade(AgendaRepository agendaRepository, UserRepository userRepository, ModelRepository modelRepository, 
                            SolvingRoleRepository solvingRoleRepository, AgendaRoleRepository agendaRoleRepository, 
                            BlockModelRepository blockModelRepository, SystemRepository systemRepository, FilterRepository filterRepository,
                            SystemAgendaRepository systemAgendaRepository, UserRoleRepository userRoleRepository, 
                            NotificationRepository notificationRepository, IMapper mapper)
        : base(filterRepository)
        {
            _agendaRepository = agendaRepository;
            _userRepository = userRepository;
            _modelRepository = modelRepository;
            _solvingRoleRepository = solvingRoleRepository;
            _agendaRoleRepository = agendaRoleRepository;
            _blockModelRepository = blockModelRepository;
            _systemRepository = systemRepository;
            _systemAgendaRepository = systemAgendaRepository;
            _userRoleRepository = userRoleRepository;
            _notificationRepository = notificationRepository;
            _mapper = mapper;
        }

        public void SetFilters(bool[] filters, Guid userId)
        {
            _agendaRepository.Filters = filters;
            _agendaRepository.UserId = userId;
            _userId = userId;
        }

        public async Task<List<AgendaAllDTO>> Filter(FilterDTO dto)
        {
            await FilterHelper.ChnageFilterState(_filterRepository, dto, _userId);
            _agendaRepository.Filters[((int)dto.Filter)] = !dto.Removed;
            return await _agendaRepository.All();
        }

        public async Task<AgendaDetailDTO> Detail(Guid id)
        {
            AgendaDetailDTO dto = await _agendaRepository.Detail(id);
            
            dto.AllAgendas = await _agendaRepository.All(id);
            dto.SelectedAgenda = await _agendaRepository.Selected(id);
            
            return dto;
        }

        public async Task<AgendaOverviewDTO> Overview()
        {
            return new AgendaOverviewDTO()
            {
                Agendas = await _agendaRepository.All()
            };
        }

        public Task<List<UserIdNameDTO>> Create()
        {
            return _userRepository.Create();
        }

        public async Task<Guid> Create(AgendaCreateDTO dto)
        {
            AgendaEntity entity = _mapper.Map<AgendaEntity>(dto);
            await _agendaRepository.Create(entity);
            await NotificationHub.CreateSendNotifications(_notificationRepository, entity.Id, NotificationTypeEnum.NewAgenda, 
                                                          entity.Name, entity.AdministratorId);
            await _agendaRepository.Save();
            return entity.Id;
        }

        public async Task<AgendaInfoCardDTO> Edit(AgendaEditDTO dto)
        {
            AgendaEntity agenda = await _agendaRepository.BareAdmin(dto.Id);
            agenda.Name = dto.Name;
            agenda.Description = dto.Description ?? "";
            await _agendaRepository.Save();

            return new AgendaInfoCardDTO()
            {
                AdministratorEmail = agenda.Administrator.Email,
                AdministratorId = agenda.AdministratorId,
                AdministratorName = $"{agenda.Administrator.Name} {agenda.Administrator.Surname}",
                Description = agenda.Description,
                Id = agenda.Id,
                Name = agenda.Name,
                SelectedAgenda = await _agendaRepository.Selected(agenda.Id)
            };
        }

        public async Task AddUserRole(Guid userId, Guid agendaRoleId)
        {
            await _userRoleRepository.Create(new UserRoleEntity
            {
                AgendaRoleId = agendaRoleId,
                UserId = userId
            });

            AgendaIdNameDTO agenda = await _agendaRoleRepository.AgendaIdName(agendaRoleId);
            await NotificationHub.CreateSendNotifications(_notificationRepository, agenda.Id, NotificationTypeEnum.NewRole, 
                                                          agenda.Name, userId);

            await _agendaRoleRepository.Save();
        }

        public async Task RemoveUserRole(Guid userId, Guid agendaRoleId)
        {
            AgendaIdNameDTO agenda = await _agendaRoleRepository.AgendaIdName(agendaRoleId);
            _userRoleRepository.Remove(await _userRoleRepository.RoleForRemoval(userId, agendaRoleId));
            
            await NotificationHub.CreateSendNotifications(_notificationRepository, agenda.Id, NotificationTypeEnum.RemovedRole, 
                                                          agenda.Name, userId);

            await _userRoleRepository.Save();
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

        public async Task RemoveAgendaRole(Guid id)
        {
            AgendaRoleEntity agendaRole = await _agendaRoleRepository.RoleForRemoval(id); 

            foreach (UserTaskModelEntity task in await _blockModelRepository.RolesForRemovalUserTaks(agendaRole.RoleId))
            {
                task.RoleId = null;
            }

            foreach (ServiceTaskModelEntity task in await _blockModelRepository.RolesForRemovalServiceTaks(agendaRole.RoleId))
            {
                task.RoleId = null;
            }

            await NotificationHub.CreateSendNotifications(_notificationRepository, agendaRole.AgendaId, NotificationTypeEnum.RemovedRole, 
                                                          agendaRole.Agenda.Name, agendaRole.UserRoles.Select(x => x.UserId).ToArray());

            _agendaRoleRepository.Remove(agendaRole);
            await _agendaRoleRepository.Save();
        }

        public Task<List<AgendaIdNameDTO>> UploadModel()
        {
            return _agendaRepository.AdminOfAgendas();
        }

        public Task<List<UserIdNameDTO>> MissingInRole(Guid agendaId, Guid roleId)
        {
            return _userRepository.MissingInRole(agendaId, roleId);
        }

        public Task<AgendaDetailDTO> DetailPartial(Guid id)
        {
            return _agendaRepository.Detail(id);
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
            if (dto.Id == null)
            {
                SolvingRoleEntity role = new SolvingRoleEntity
                {
                    Name = dto.Name,
                    Description = dto.Description ?? "",
                    AgendaRoles = new List<AgendaRoleEntity>
                    {
                        new AgendaRoleEntity
                        {
                            AgendaId = dto.AgendaId,
                        }
                    }
                };

                await _solvingRoleRepository.Create(role);
            }
            else
            {
                await _agendaRoleRepository.Create(new AgendaRoleEntity
                {
                    AgendaId = dto.AgendaId,
                    RoleId = dto.Id.Value,
                });
            }
            

            await _agendaRoleRepository.Save();
            return await _agendaRoleRepository.Roles(dto.AgendaId);
        }
    }
}
