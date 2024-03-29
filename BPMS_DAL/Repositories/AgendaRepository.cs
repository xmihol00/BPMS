using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Agenda;
using BPMS_Common.Enums;
using BPMS_DTOs.System;
using BPMS_DTOs.Model;
using BPMS_DTOs.User;
using BPMS_DTOs.Role;
using BPMS_DTOs.Workflow;

namespace BPMS_DAL.Repositories
{
    public class AgendaRepository : BaseRepository<AgendaEntity>
    {
        public AgendaRepository(BpmsDbContext context) : base(context) {} 

        public Task<List<AgendaAllDTO>> All(Guid? id = null)
        {
            IQueryable<AgendaEntity> query = _dbSet.Include(x => x.Models)
                                                   .Include(x => x.Systems)
                                                   .Include(x => x.Workflows)
                                                   .Include(x => x.AgendaRoles)
                                                      .ThenInclude(x => x.UserRoles)
                                                   .Where(x => x.Id != id);
            if (Filters != null)
            {
                if (Filters[((int)FilterTypeEnum.AgendaKeeper)])
                {
                    query = query.Where(x => x.AdministratorId == UserId);
                }

                if (Filters[((int)FilterTypeEnum.AgendaRole)])
                {
                    query = query.Where(x => x.AgendaRoles.Any(y => y.UserRoles.Any(z => z.UserId == UserId)));
                }

                if (Filters[((int)FilterTypeEnum.AgendaPausedWF)] || Filters[((int)FilterTypeEnum.AgendaActiveWF)] ||
                    Filters[((int)FilterTypeEnum.AgendaFinishedWF)] || Filters[((int)FilterTypeEnum.AgendaCanceledWF)])
                {
                    query = query.Where(x => (Filters[((int)FilterTypeEnum.AgendaPausedWF)] && x.Workflows.Any(y => y.State == WorkflowStateEnum.Paused || y.State == WorkflowStateEnum.Waiting)) ||
                                             (Filters[((int)FilterTypeEnum.AgendaActiveWF)] && x.Workflows.Any(y => y.State == WorkflowStateEnum.Active)) ||
                                             (Filters[((int)FilterTypeEnum.AgendaFinishedWF)] && x.Workflows.Any(y => y.State == WorkflowStateEnum.Finished)) ||
                                             (Filters[((int)FilterTypeEnum.AgendaCanceledWF)] && x.Workflows.Any(y => y.State == WorkflowStateEnum.Canceled)));
                }
            }

            return query.Select(x => new AgendaAllDTO 
                        {
                            Id = x.Id,
                            Name = x.Name,
                            ActiveWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Active).Count(),
                            PausedWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Paused || y.State == WorkflowStateEnum.Waiting).Count(),
                            FinishedWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Finished).Count(),
                            CanceledWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Canceled).Count(),
                            ModelsCount = x.Models.Count(),
                            SystemsCount = x.Systems.Count(),
                            UserCount = x.AgendaRoles.SelectMany(x => x.UserRoles).Count(),
                            MissingRolesCount = x.AgendaRoles.Where(y => y.UserRoles.Count == 0).Count(),
                        })
                        .ToListAsync();
        }

        public Task<AgendaDetailDTO> Detail(Guid id)
        {
            return _dbSet.Include(x => x.Administrator)
                         .Include(x => x.Models)
                         .Include(x => x.Workflows)
                            .ThenInclude(x => x.Model)
                         .Include(x => x.Workflows)
                            .ThenInclude(x => x.Administrator)
                         .Include(x => x.Workflows)
                            .ThenInclude(x => x.Blocks)
                         .Include(x => x.Systems)
                         .Include(x => x.AgendaRoles)
                            .ThenInclude(x => x.Role)
                         .Include(x => x.AgendaRoles)
                            .ThenInclude(x => x.UserRoles)
                                .ThenInclude(x => x.User)
                         .Include(x => x.AgendaRoles)
                            .ThenInclude(x => x.Role)
                         .Select(x => new AgendaDetailDTO 
                         {
                             AdministratorId = x.AdministratorId,
                             AdministratorName = $"{x.Administrator.Title} {x.Administrator.Name} {x.Administrator.Surname}",
                             AdministratorEmail = x.Administrator.Email,
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description,
                             Models = x.Models
                                       .Select(y => new ModelAllAgendaDTO
                                       {
                                           Id = y.Id,
                                           Name = y.Name,
                                           SVG = y.SVG,
                                           State = y.State
                                       })
                                       .ToList(),
                            Workflows = x.Workflows
                                         .Where(y => y.State == WorkflowStateEnum.Active)
                                         .Select(y => new WorkflowAllAgendaDTO
                                         {
                                             Description = y.Description,
                                             Id = y.Id,
                                             Name = y.Name,
                                             State = y.State,
                                             SVG = y.Model.SVG,
                                             AdministratorEmail = y.Administrator.Email,
                                             AdministratorName = $"{x.Administrator.Title} {y.Administrator.Name} {y.Administrator.Surname}",
                                             ModelId = y.ModelId,
                                             ModelName = y.Model.Name
                                         })
                                         .ToList(),
                            Roles = x.AgendaRoles
                                     .Select(y => new RoleDetailDTO
                                     {
                                         Description = y.Role.Description,
                                         Id = y.Id,
                                         Name = y.Role.Name,
                                         Users = y.UserRoles.Select(z => new UserIdNameDTO
                                                            {
                                                                Id = z.UserId,
                                                                FullName = $"{z.User.Title} {z.User.Name} {z.User.Surname}",
                                                            })
                                                            .ToList()
                                     })
                                     .ToList(),
                            ActiveBlocks = x.Workflows
                                            .Where(x => x.State == WorkflowStateEnum.Active)
                                            .Select(y => new WorkflowActiveBlocksDTO
                                            {
                                                Id = y.Id,
                                                BlockIds = y.Blocks
                                                            .Where(z => z.State == BlockWorkflowStateEnum.Active)
                                                            .Select(z => z.BlockModelId)
                                                            .ToList()
                                            })
                                            .ToList(),
                            Systems = x.Systems.Select(y => y.System)
                                               .Select(y => new SystemAllDTO
                                                {
                                                    Id = y.Id,
                                                    Name = y.Name,
                                                    URL = y.URL,
                                                    State = y.State
                                                })
                                                .ToList()
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<AgendaInfoCardDTO> InfoCard(Guid id)
        {
            return _dbSet.Include(x => x.Models)
                         .Include(x => x.Systems)
                         .Include(x => x.Workflows)
                         .Include(x => x.AgendaRoles)
                            .ThenInclude(x => x.UserRoles)
                         .Select(x => new AgendaInfoCardDTO
                         {
                             SelectedAgenda = new AgendaAllDTO 
                                              {
                                                  Id = x.Id,
                                                  Name = x.Name,
                                                  ActiveWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Active).Count(),
                                                  PausedWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Paused || y.State == WorkflowStateEnum.Waiting).Count(),
                                                  FinishedWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Finished).Count(),
                                                  CanceledWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Canceled).Count(),
                                                  ModelsCount = x.Models.Count(),
                                                  SystemsCount = x.Systems.Count(),
                                                  UserCount = x.AgendaRoles.SelectMany(x => x.UserRoles).Count(),
                                                  MissingRolesCount = x.AgendaRoles.Where(y => y.UserRoles.Count == 0).Count(),
                                              },
                            AdministratorId = x.AdministratorId,
                            AdministratorName = $"{x.Administrator.Title} {x.Administrator.Name} {x.Administrator.Surname}",
                            AdministratorEmail = x.Administrator.Email,
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<Guid> RoleByName(string roleName, Guid agendaId)
        {
            return _dbSet.Include(x => x.AgendaRoles)
                            .ThenInclude(x => x.Role)
                         .Where(x => x.Id == agendaId)
                         .SelectMany(x => x.AgendaRoles)
                         .Select(x => x.Role)
                         .Where(x => x.Name == roleName)
                         .Select(x => x.Id)
                         .FirstOrDefaultAsync();
        }

        public Task<Guid> CurrentAdmin(Guid agendaId)
        {
            return _dbSet.Where(x => x.Id == agendaId)
                         .Select(x => x.AdministratorId)
                         .FirstAsync();
        }

        public Task<bool> Keeper(Guid id)
        {
            return _dbSet.AnyAsync(x => x.Id == id && x.AdministratorId == UserId);
        }

        public Task<List<AgendaIdNameDTO>> AdminOfAgendas()
        {
            return _dbSet.Where(x => x.AdministratorId == UserId)
                         .Select(x => new AgendaIdNameDTO
                         {
                             Id = x.Id,
                             Name = x.Name
                         })
                         .ToListAsync();
        }

        public Task<List<AgendaIdNameDTO>> Agendas()
        {
            return _dbSet.Select(x => new AgendaIdNameDTO
                         {
                             Id = x.Id,
                             Name = x.Name
                         })
                         .ToListAsync();
        }

        public Task<List<AgendaIdNameDTO>> AgendasSystem(Guid systemId)
        {
            return _dbSet.Include(x => x.Systems)
                         .Where(x => x.Systems.Any(x => x.SystemId == systemId))
                         .Select(x => new AgendaIdNameDTO
                         {
                             Id = x.Id,
                             Name = x.Name
                         })
                         .ToListAsync();
        }

        public Task<AgendaAllDTO> Selected(Guid id)
        {
            return _dbSet.Include(x => x.Models)
                         .Include(x => x.Systems)
                         .Include(x => x.Workflows)
                         .Include(x => x.AgendaRoles)
                            .ThenInclude(x => x.UserRoles)
                         .Select(x => new AgendaAllDTO 
                         {
                             Id = x.Id,
                             Name = x.Name,
                             ActiveWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Active).Count(),
                             PausedWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Paused || y.State == WorkflowStateEnum.Waiting).Count(),
                             FinishedWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Finished).Count(),
                             CanceledWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Canceled).Count(),
                             ModelsCount = x.Models.Count(),
                             SystemsCount = x.Systems.Count(),
                             UserCount = x.AgendaRoles.SelectMany(x => x.UserRoles).Count(),
                             MissingRolesCount = x.AgendaRoles.Where(y => y.UserRoles.Count == 0).Count(),
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<AgendaEntity> Bare(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<List<SystemAllDTO>> Systems(Guid id)
        {
            return _dbSet.Include(x => x.Systems)
                            .ThenInclude(x => x.System)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.Systems)
                         .Select(x => new SystemAllDTO
                         {
                             Id = x.System.Id,
                             Name = x.System.Name,
                             URL = x.System.URL,
                             State = x.System.State
                         })
                         .ToListAsync();
        }
    }
}
