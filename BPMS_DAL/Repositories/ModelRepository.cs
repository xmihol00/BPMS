using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using BPMS_DAL.Entities;
using BPMS_DTOs.Model;
using BPMS_DTOs.Pool;
using BPMS_DAL.Sharing;
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DTOs.User;
using BPMS_DTOs.Workflow;

namespace BPMS_DAL.Repositories
{
    public class ModelRepository : BaseRepository<ModelEntity>
    {
        public ModelRepository(BpmsDbContext context) : base(context) {}

        public Task<List<ModelAllDTO>> OfAgenda(Guid id)
        {
            return _dbSet.Where(x => x.AgendaId == id)
                         .Select(x => new ModelAllDTO
                         {
                             Id = x.Id,
                             Name = x.Name,
                             SVG = x.SVG
                         })
                         .ToListAsync();
        }

        public Task<ModelDetailDTO> Detail(Guid id)
        {
            return _dbSet.Select(x => new ModelDetailDTO
                         {
                             Id = x.Id,
                             Description = x.Description,
                             Name = x.Name,
                             SVG = x.SVG,
                             State = x.State,
                             Workflow = x.Workflows.Select(y => new WorkflowRunDTO 
                                                   {
                                                       Description = x.Description,
                                                       Id = x.Id,
                                                       Name = x.Name
                                                   })
                                                   .FirstOrDefault()
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<ModelEntity> DetailRaw(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<ModelEntity> DetailDeep(Guid id)
        {
            return _dbSet.Include(x => x.Pools)
                            .ThenInclude(x => x.Blocks)
                                .ThenInclude(x => x.InFlows)
                         .Include(x => x.Pools)
                            .ThenInclude(x => x.Blocks)
                                .ThenInclude(x => x.OutFlows)
                         .Include(x => x.Pools)
                            .ThenInclude(x => x.Blocks)
                                .ThenInclude(x => x.Attributes)
                         .Include(x => x.Pools)
                            .ThenInclude(x => x.Blocks)
                                .ThenInclude(x => x.MappedAttributes)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<ModelDetailShare> Share(Guid id)
        {
            return _dbSet.Select(x => new ModelDetailShare
                         {
                             Description = x.Description,
                             Id = x.Id,
                             Name = x.Name,
                             State = x.State,
                             SVG = x.SVG,
                             SenderURL = StaticData.ThisSystemURL
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<bool> Any(Guid id)
        {
            return _dbSet.AnyAsync(x => x.Id == id);
        }

        public Task<ModelHeaderDTO> Header(Guid id)
        {
            return _dbSet.Include(x => x.Workflows)
                         .Select(x => new ModelHeaderDTO
                         {
                             Id = x.Id,
                             Description = x.Description,
                             Name = x.Name,
                             State = x.State,
                             Workflow = x.Workflows.Select(y => new WorkflowRunDTO 
                                                   {
                                                       Description = x.Description,
                                                       Id = x.Id,
                                                       Name = x.Name
                                                   })
                                                   .FirstOrDefault()
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public void ChangeState(Guid id, ModelStateEnum state)
        {
            _dbSet.Attach(new ModelEntity 
                  {
                      Id = id,
                      State = state
                  })
                  .Property(x => x.State)
                  .IsModified = true;
        }

        public Task<bool> CheckState(Guid modelId, ModelStateEnum state)
        {
            return _dbSet.AnyAsync(x => x.Id == modelId && x.State == state);
        }

        public Task<ModelEntity> DetailToCreateWF(Guid id)
        {
            return _dbSet.Include(x => x.Pools)
                            .ThenInclude(x => x.Blocks)
                                .ThenInclude(x => x.InFlows)
                         .Include(x => x.Pools)
                            .ThenInclude(x => x.Blocks)
                                .ThenInclude(x => x.OutFlows)
                         .Include(x => x.Pools)
                            .ThenInclude(x => x.Blocks)
                                .ThenInclude(x => x.Attributes)
                         .Include(x => x.Pools)
                            .ThenInclude(x => x.Blocks)
                                .ThenInclude(x => x.MappedAttributes)
                         .Include(x => x.Pools)
                            .ThenInclude(x => x.Blocks)
                                .ThenInclude(x => x.DataSchemas)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<UserIdNameDTO>> WorflowKeepers(Guid id)
        {
            return _dbSet.Include(x => x.Agenda)
                            .ThenInclude(x => x.AgendaRoles)
                                .ThenInclude(x => x.UserRoles)
                                    .ThenInclude(x => x.User)
                                        .ThenInclude(x => x.SystemRoles)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.Agenda.AgendaRoles)
                         .SelectMany(x => x.UserRoles)
                         .Where(x => x.User.SystemRoles.Any(y => y.Role == SystemRoleEnum.WorkflowKeeper))
                         .Select(x => new UserIdNameDTO
                         {
                             FullName = $"{x.User.Name} {x.User.Surname}",
                             Id = x.User.Id
                         })
                         .ToListAsync();
        }

        public Task<ModelEntity> StateAgendaId(Guid id)
        {
            return _dbSet.Where(x => x.Id == id)
                         .Select(x => new ModelEntity
                         {
                             AgendaId = x.AgendaId,
                             State = x.State
                         })
                         .FirstAsync();
        }

        public Task<Guid?> AgendaId(Guid id)
        {
            return _dbSet.Where(x => x.Id == id)
                         .Select(x => x.AgendaId)
                         .FirstAsync();
        }
    }
}
