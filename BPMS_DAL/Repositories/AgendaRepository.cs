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
                         .Include(x => x.UserRoles)
                         .Select(x => new AgendaAllDTO 
                         {
                             Id = x.Id,
                             Name = x.Name,
                             ActiveWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Active).Count(),
                             PausedWorkflowsCount = x.Workflows.Where(y => y.State == WorkflowStateEnum.Paused).Count(),
                             ModelsCount = x.Models.Count(),
                             SystemsCount = x.Systems.Count(),
                             UserCount = x.UserRoles.Where(y => y.UserId != Guid.Empty).Count(),
                             MissingRolesCount = x.UserRoles.Where(y => y.UserId == Guid.Empty).Count(),
                         })
                         .ToListAsync();
        }

        public Task<AgendaDetailDTO> Detail(Guid id)
        {
            return _dbSet.Include(x => x.Administrator)
                         .Select(x => new AgendaDetailDTO 
                         {
                             AdministratorId = x.AdministratorId,
                             AdministratorName = $"{x.Administrator.Name} {x.Administrator.Surname}",
                             AdministratorEmail = x.Administrator.Email,
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<AgendaEntity> DetailBase(Guid id)
        {
            return _dbSet.Include(x => x.Administrator)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<AgendaDetailPartialDTO> DetailPartial(Guid id)
        {
            return _dbSet.Include(x => x.Administrator)
                         .Include(x => x.Systems)
                            .ThenInclude(x => x.System)
                         .Select(x => new AgendaDetailPartialDTO 
                         {
                             AdministratorId = x.AdministratorId,
                             AdministratorName = $"{x.Administrator.Name} {x.Administrator.Surname}",
                             AdministratorEmail = x.Administrator.Email,
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description,
                             Systems = x.Systems.Select(y => new SystemAllDTO
                             {
                                 Id = y.System.Id,
                                 Name = y.System.Name,
                                 URL = y.System.URL
                             }).ToList()
                         })
                         .FirstAsync(x => x.Id == id);
        }
    }
}
