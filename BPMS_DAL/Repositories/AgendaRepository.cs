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
            #pragma warning disable CS8602
            return _dbSet.Include(x => x.Administrator)
                         .Select(x => new AgendaDetailDTO 
                         {
                             AdministratorId = x.AdministratorId,
                             AdministratorName = $"{x.Administrator.Name} {x.Administrator.Surname}",
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description
                         })
                         .FirstAsync(x => x.Id == id);
            #pragma warning restore CS8602
        }

        public Task<AgendaDetailPartialDTO> DetailPartial(Guid id)
        {
            #pragma warning disable CS8602
            return _dbSet.Include(x => x.Administrator)
                         .Select(x => new AgendaDetailPartialDTO 
                         {
                             AdministratorId = x.AdministratorId,
                             AdministratorName = $"{x.Administrator.Name} {x.Administrator.Surname}",
                             Id = x.Id,
                             Name = x.Name,
                             Description = x.Description
                         })
                         .FirstAsync(x => x.Id == id);
            #pragma warning restore CS8602
        }
    }
}
