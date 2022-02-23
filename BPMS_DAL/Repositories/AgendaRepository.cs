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

        public Task<List<AgendaAllDTO>> All()
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
                         .ToListAsync();
        }

        public Task<AgendaDetailDTO> Detail(Guid id)
        {
            return _dbSet.Include(x => x.Administrator)
                         .Include(x => x.Models)
                         .Include(x => x.Workflows)
                            .ThenInclude(x => x.Model)
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
                             AdministratorName = $"{x.Administrator.Name} {x.Administrator.Surname}",
                             AdministratorEmail = x.Administrator.Email,
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description,
                             Models = x.Models
                                       .Select(y => new ModelAllDTO
                                       {
                                           Id = y.Id,
                                           Name = y.Name,
                                           SVG = y.SVG
                                       })
                                       .ToList(),
                            Workflows = x.Workflows
                                         .Select(y => new WorkflowAllAgendaDTO
                                         {
                                             Description = y.Description,
                                             Id = y.Id,
                                             Name = y.Name,
                                             State = y.State,
                                             SVG = y.Model.SVG
                                         })
                                         .ToList(),
                            Roles = x.AgendaRoles.Select(x => new RoleDetailDTO
                                     {
                                         Description = x.Role.Description,
                                         Id = x.Id,
                                         Name = x.Role.Name,
                                         Users = x.UserRoles.Select(y => new UserIdNameDTO
                                                            {
                                                                Id = y.UserId,
                                                                FullName = $"{y.User.Name} {y.User.Surname}",
                                                            })
                                                            .ToList()
                                     })
                                     .ToList()
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<AgendaEntity> DetailBase(Guid id)
        {
            return _dbSet.Include(x => x.Administrator)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<SystemAllDTO>> Systems(Guid id)
        {
            return _dbSet.Include(x => x.Systems)
                            .ThenInclude(x => x.System)
                         .SelectMany(x => x.Systems)
                         .Select(x => new SystemAllDTO
                         {
                             Id = x.System.Id,
                             Name = x.System.Name,
                             URL = x.System.URL
                         })
                         .ToListAsync();
        }
    }
}
