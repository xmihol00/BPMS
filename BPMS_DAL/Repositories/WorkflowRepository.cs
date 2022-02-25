using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_Common.Enums;
using BPMS_DTOs.Workflow;

namespace BPMS_DAL.Repositories
{
    public class WorkflowRepository : BaseRepository<WorkflowEntity>
    {
        public WorkflowRepository(BpmsDbContext context) : base(context) {}

        public Task<WorkflowEntity> Waiting(Guid modelId)
        {
            return _dbSet.FirstAsync(x => x.ModelId == modelId && x.State == WorkflowStateEnum.Waiting);
        }

        public Task<List<WorkflowAllDTO>> Overview()
        {
            return _dbSet.Include(x => x.Model)
                         .Include(x => x.Agenda)
                         .Select(x => new WorkflowAllDTO
                         {
                             AgendaId = x.AgendaId,
                             AgendaName = x.Agenda.Name,
                             Description = x.Description,
                             Id = x.Id,
                             ModelId = x.ModelId,
                             ModelName = x.Model.Name,
                             Name = x.Name,
                             State = x.State,
                             SVG = x.Model.SVG
                         })
                         .ToListAsync();
        }

        public Task<WorkflowEntity> Bare(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }
    }
}
