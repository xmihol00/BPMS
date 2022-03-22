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

        public Task<WorkflowEntity> Waiting(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id && x.State == WorkflowStateEnum.Waiting);
        }

        public Task<List<WorkflowActiveBlocksDTO>> ActiveBlocks()
        {
            return _dbSet.Include(x => x.Blocks)
                         .Where(x => x.State == WorkflowStateEnum.Active)
                         .Select(x => new WorkflowActiveBlocksDTO
                         {
                             Id = x.Id,
                             BlockIds = x.Blocks
                                         .Where(y => y.State == BlockWorkflowStateEnum.Active)
                                         .Select(y => y.BlockModelId)
                                         .ToList()
                         })
                         .ToListAsync();
        }

        public Task<WorkflowActiveBlocksDTO> ActiveBlocks(Guid id)
        {
            return _dbSet.Include(x => x.Blocks)
                         .Where(x => x.Id == id)
                         .Select(x => new WorkflowActiveBlocksDTO
                         {
                             BlockIds = x.Blocks
                                         .Where(y => y.State == BlockWorkflowStateEnum.Active)
                                         .Select(y => y.BlockModelId)
                                         .ToList()
                         })
                         .FirstAsync();
        }

        public Task<List<TaskDataEntity>> OutputUserTasks(Guid id)
        {
            return _dbSet.Include(x => x.Blocks)
                            .ThenInclude(x => x.OutputData)
                                .ThenInclude(x => x.Attribute)
                         .Include(x => x.Blocks)
                            .ThenInclude(x => x.OutputData)
                                .ThenInclude(x => x.OutputTask)
                                    .ThenInclude(x => x.BlockModel)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.Blocks)
                         .SelectMany(x => x.OutputData)
                         .Where(x => x.AttributeId != null)
                         .ToListAsync();
        }

        public Task<List<TaskDataEntity>> MappedServiceTasks(Guid id)
        {
            return _dbSet.Include(x => x.Blocks)
                            .ThenInclude(x => x.OutputData)
                                .ThenInclude(x => x.Attribute)
                         .Include(x => x.Blocks)
                            .ThenInclude(x => x.OutputData)
                                .ThenInclude(x => x.OutputTask)
                                    .ThenInclude(x => x.BlockModel)
                         .Include(x => x.Blocks)
                            .ThenInclude(x => x.InputData)
                         .Include(x => x.Blocks)
                            .ThenInclude(x => x.OutputData)
                                .ThenInclude(x => x.Schema)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.Blocks)
                         .SelectMany(x => x.OutputData)
                         .Where(x => x.SchemaId != null && x.Schema.StaticData == null)
                         .ToListAsync();
        }

        public Task<bool> IsKeeper(Guid id)
        {
            return _dbSet.AnyAsync(x => x.Id == id && x.AdministratorId == UserId);
        }

        public Task<Guid?> Keeper(Guid id)
        {
            return _dbSet.Where(x => x.Id == id)
                         .Select(x => x.AdministratorId)
                         .FirstAsync();
        }

        public Task<WorkflowDetailDTO> Detail(Guid id)
        {
            return _dbSet.Include(x => x.Model)
                         .Include(x => x.Agenda)
                         .Include(x => x.Administrator)
                         .Include(x => x.Blocks)
                         .Select(x => new WorkflowDetailDTO
                         {
                             AgendaId = x.AgendaId,
                             AgendaName = x.Agenda.Name,
                             Description = x.Description,
                             Id = x.Id,
                             ModelId = x.ModelId,
                             ModelName = x.Model.Name,
                             Name = x.Name,
                             State = x.State,
                             SVG = x.Model.SVG,
                             ExpectedEnd = x.ExpectedEnd,
                             AdministratorEmail = x.Administrator.Email,
                             AdministratorName = $"{x.Administrator.Title} {x.Administrator.Name} {x.Administrator.Surname}",
                         })
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<WorkflowAllDTO>> All(Guid? id = null)
        {
            IQueryable<WorkflowEntity> query = _dbSet.Include(x => x.Model)
                                                     .Include(x => x.Agenda)
                                                     .Include(x => x.Administrator)
                                                     .Where(x => x.Id != id);

            if (Filters != null)
            {
                if (Filters[((int)FilterTypeEnum.WorkflowKeeper)])
                {
                    query = query.Where(x => x.AdministratorId == UserId);
                }

                if (Filters[((int)FilterTypeEnum.WorkflowActive)] || Filters[((int)FilterTypeEnum.WorkflowPaused)] ||
                    Filters[((int)FilterTypeEnum.WorkflowFinished)] || Filters[((int)FilterTypeEnum.WorkflowCanceled)])
                {
                    query = query.Where(x => (Filters[((int)FilterTypeEnum.WorkflowActive)] && x.State == WorkflowStateEnum.Active) ||
                                             (Filters[((int)FilterTypeEnum.WorkflowPaused)] && (x.State == WorkflowStateEnum.Paused || x.State == WorkflowStateEnum.Waiting)) ||
                                             (Filters[((int)FilterTypeEnum.WorkflowFinished)] && x.State == WorkflowStateEnum.Finished) ||
                                             (Filters[((int)FilterTypeEnum.WorkflowCanceled)] && x.State == WorkflowStateEnum.Canceled));
                }
            }

            return query.Select(x => new WorkflowAllDTO
                        {
                            AgendaId = x.AgendaId,
                            AgendaName = x.Agenda.Name,
                            Description = x.Description,
                            Id = x.Id,
                            ModelId = x.ModelId,
                            ModelName = x.Model.Name,
                            Name = x.Name,
                            State = x.State,
                            SVG = x.Model.SVG,
                            AdministratorEmail = x.Administrator.Email,
                            AdministratorName = $"{x.Administrator.Title} {x.Administrator.Name} {x.Administrator.Surname}"
                        })
                        .ToListAsync();
        }

        public Task<WorkflowInfoCardDTO> InfoCard(Guid id)
        {
            return _dbSet.Include(x => x.Model)
                         .Include(x => x.Agenda)
                         .Include(x => x.Administrator)
                         .Where(x => x.Id == id)
                         .Select(x => new WorkflowInfoCardDTO
                         {
                            AdministratorEmail = x.Administrator.Email,
                            AdministratorName = $"{x.Administrator.Title} {x.Administrator.Name} {x.Administrator.Surname}",
                            Description = x.Description,
                            Id = x.Id,
                            Name = x.Name,
                            State = x.State,
                            ExpectedEnd = x.ExpectedEnd,
                            SelectedWorkflow = new WorkflowAllDTO
                            {
                                AgendaId = x.AgendaId,
                                AgendaName = x.Agenda.Name,
                                Description = x.Description,
                                Id = x.Id,
                                ModelId = x.ModelId,
                                ModelName = x.Model.Name,
                                Name = x.Name,
                                State = x.State,
                                SVG = x.Model.SVG,
                                AdministratorEmail = x.Administrator.Email,
                                AdministratorName = $"{x.Administrator.Title} {x.Administrator.Name} {x.Administrator.Surname}"
                            }                           
                         })
                         .FirstAsync();
        }

        public Task<WorkflowAllDTO> Selected(Guid id)
        {
            return _dbSet.Include(x => x.Model)
                         .Include(x => x.Agenda)
                         .Include(x => x.Administrator)
                         .Where(x => x.Id == id)
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
                             SVG = x.Model.SVG,
                             AdministratorEmail = x.Administrator.Email,
                             AdministratorName = $"{x.Administrator.Title} {x.Administrator.Name} {x.Administrator.Surname}"
                         })
                         .FirstAsync();
        }

        public Task<WorkflowEntity> Bare(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<WorkflowEntity?> WaitingOrDefault(Guid modelId)
        {
            return _dbSet.FirstOrDefaultAsync(x => x.ModelId == modelId && x.State == WorkflowStateEnum.Waiting);
        }

        public Task<bool> Any(Guid id)
        {
            return _dbSet.AnyAsync(x => x.Id == id);
        }
    }
}
