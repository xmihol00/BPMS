using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.System;
using BPMS_Common;
using BPMS_DTOs.Agenda;
using BPMS_Common.Enums;
using BPMS_DTOs.Model;
using BPMS_DTOs.Account;
using BPMS_DTOs.Pool;

namespace BPMS_DAL.Repositories
{
    public class SystemRepository : BaseRepository<SystemEntity>
    {
        public SystemRepository(BpmsDbContext context) : base(context) {} 

        public Task<List<SystemPickerDTO>> SystemsOfAgenda(Guid? agendaId)
        {
            return _dbSet.Include(x => x.Agendas)
                         .Where(x => x.Agendas.Any(y => y.AgendaId == agendaId))
                         .Select(x => new SystemPickerDTO
                         {
                             Id = x.Id,
                             Name = x.Name,
                             URL = x.URL
                         })
                         .ToListAsync();
        }

        public Task<List<SystemIdNameDTO>> IdNames()
        {
            return _dbSet.Select(x => new SystemIdNameDTO
                         {
                             Name = x.Name,
                             Id = x.Id
                         })
                         .ToListAsync();
        }

        public Task<List<SystemAllDTO>> All(Guid? id = null)
        {
            IQueryable<SystemEntity> query = _dbSet.Where(x => x.Id != id);

            if (Filters != null)
            {
                if (Filters[((int)FilterTypeEnum.SystemActive)] || Filters[((int)FilterTypeEnum.SystemInactive)] ||
                    Filters[((int)FilterTypeEnum.SystemWaiting)] || Filters[((int)FilterTypeEnum.SystemDeactivated)] ||
                    Filters[((int)FilterTypeEnum.SystemThisSystem)])
                {
                    query = query.Where(x => (Filters[((int)FilterTypeEnum.SystemInactive)] && x.State == SystemStateEnum.Inactive) ||
                                             (Filters[((int)FilterTypeEnum.SystemActive)] && x.State == SystemStateEnum.Active) ||
                                             (Filters[((int)FilterTypeEnum.SystemDeactivated)] && x.State == SystemStateEnum.Deactivated) ||
                                             (Filters[((int)FilterTypeEnum.SystemThisSystem)] && x.State == SystemStateEnum.ThisSystem) ||
                                             (Filters[((int)FilterTypeEnum.SystemWaiting)] && x.State == SystemStateEnum.Waiting));
                }
            }             
                         
            return query.Select(x => new SystemAllDTO
                        {
                            Id = x.Id,
                            Name = x.Name,
                            URL = x.URL,
                            State = x.State
                        })
                        .ToListAsync();
        }

        public Task<SystemInfoCardDTO> InfoCard(Guid id)
        {
            return _dbSet.Select(x => new SystemInfoCardDTO
                         {
                             Id = x.Id,
                             Name = x.Name,
                             URL = x.URL,
                             Description = x.Description,
                             State = x.State,
                             SelectedSystem = new SystemAllDTO
                             {
                                 Id = x.Id,
                                 Name = x.Name,
                                 URL = x.URL,
                                 State = x.State
                             }
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<SystemAllDTO> Selected(Guid id)
        {
            return _dbSet.Select(x => new SystemAllDTO
                         {
                             Id = x.Id,
                             Name = x.Name,
                             URL = x.URL,
                             State = x.State
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<SystemDetailDTO> Detail(Guid id)
        {
            return _dbSet.Include(x => x.Agendas)
                            .ThenInclude(x => x.Agenda)
                                .ThenInclude(x => x.Models)
                         .Include(x => x.Agendas)
                            .ThenInclude(x => x.Agenda)
                                .ThenInclude(x => x.Systems)
                         .Include(x => x.Agendas)
                            .ThenInclude(x => x.Agenda)
                                .ThenInclude(x => x.Workflows)
                         .Include(x => x.Agendas)
                            .ThenInclude(x => x.Agenda)
                                .ThenInclude(x => x.AgendaRoles)
                                    .ThenInclude(x => x.UserRoles)
                         .Include(x => x.Pools)
                            .ThenInclude(x => x.Model)
                         .Select(x => new SystemDetailDTO
                         {
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             URL = x.URL,
                             State = x.State,
                             Agendas = x.Agendas.Select(y => y.Agenda)
                                                .Select(y => new AgendaAllDTO
                                                {
                                                    Id = y.Id,
                                                    Name = y.Name,
                                                    ActiveWorkflowsCount = y.Workflows.Where(y => y.State == WorkflowStateEnum.Active).Count(),
                                                    PausedWorkflowsCount = y.Workflows.Where(y => y.State == WorkflowStateEnum.Paused || y.State == WorkflowStateEnum.Waiting).Count(),
                                                    FinishedWorkflowsCount = y.Workflows.Where(y => y.State == WorkflowStateEnum.Finished).Count(),
                                                    CanceledWorkflowsCount = y.Workflows.Where(y => y.State == WorkflowStateEnum.Canceled).Count(),
                                                    ModelsCount = y.Models.Count(),
                                                    SystemsCount = y.Systems.Count(),
                                                    UserCount = y.AgendaRoles.SelectMany(x => x.UserRoles).Count(),
                                                    MissingRolesCount = y.AgendaRoles.Where(y => y.UserRoles.Count == 0).Count(),
                                                })
                                                .ToList()
                         })
                        .FirstAsync(x => x.Id == id);
        }

        public Task<List<SystemIdNameDTO>> ThisSystemIdName()
        {
            return _dbSet.Where(x => x.Id == StaticData.ThisSystemId)
                         .Select(x => new SystemIdNameDTO
                         {
                             Id = x.Id,
                             Name = x.Name
                         })
                         .ToListAsync();
        }

        public Task<DstAddressDTO> Address(Guid systemId)
        {
            return _dbSet.Select(x => new DstAddressDTO
                         {
                             DestinationURL = x.URL,
                             Key = x.Key,
                             SystemId = x.Id
                         })
                         .FirstAsync(x => x.SystemId == systemId);
        }

        public Task<Guid> IdFromUrl(string systemURL)
        {
            return _dbSet.Where(x => x.URL == systemURL)
                         .Select(x => x.Id)
                         .FirstOrDefaultAsync();
        }

        public Task<List<SystemAllDTO>> NotInAgenda(Guid agendaId)
        {
            return _dbSet.Include(x => x.Agendas)
                         .Where(x => x.Agendas.All(y => y.AgendaId != agendaId) && x.Id != StaticData.ThisSystemId)
                         .Select(x => new SystemAllDTO
                         {
                             Id = x.Id,
                             Name = x.Name,
                             URL = x.URL
                         })
                         .ToListAsync();
        }

        public Task<List<Guid>> Agendas(string senderURL)
        {
            return _dbSet.Include(x => x.Agendas)
                            .Where(x => x.URL == senderURL)
                         .SelectMany(x => x.Agendas)
                         .Select(x => x.AgendaId)
                         .ToListAsync();
        }

        public Task<SystemPickerDTO> ThisSystem()
        {
            return _dbSet.Where(x => x.Id == StaticData.ThisSystemId)
                         .Select(x => new SystemPickerDTO
                         {
                             Id = x.Id,
                             Name = x.Name,
                             URL = x.URL
                         })
                         .FirstAsync();
        }

        public SystemEntity Bare(Guid id)
        {
            return _dbSet.First(x => x.Id == id);
        }

        public Task<SystemEntity> BareAsync(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }
    }
}
