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
using BPMS_DTOs.Task;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DAL.Interfaces.WorkflowBlocks;
using BPMS_DTOs.BlockModel;
using BPMS_DTOs.BlockWorkflow;
using BPMS_DTOs.User;

namespace BPMS_DAL.Repositories
{
    public class BlockWorkflowRepository : BaseRepository<BlockWorkflowEntity>
    {
        private readonly DbSet<UserTaskWorkflowEntity> _userTasks;
        private readonly DbSet<ServiceTaskWorkflowEntity> _serviceTasks;
        private readonly DbSet<TaskWorkflowEntity> _tasks;
        
        public BlockWorkflowRepository(BpmsDbContext context) : base(context) 
        {
            _userTasks = context.Set<UserTaskWorkflowEntity>();
            _serviceTasks = context.Set<ServiceTaskWorkflowEntity>();
            _tasks = context.Set<TaskWorkflowEntity>();
        }

        public async Task<List<TaskAllDTO>> All(Guid? id = null)
        {
            
            IQueryable<ServiceTaskWorkflowEntity> serviceQuery = 
                _serviceTasks.Include(x => x.BlockModel)
                             .Include(x => x.Workflow)
                                .ThenInclude(x => x.Agenda)
                             .Where(x => x.UserId == UserId && x.Id != id);
            
            IQueryable<UserTaskWorkflowEntity> taskQuery = 
                _userTasks.Include(x => x.BlockModel)
                          .Include(x => x.Workflow)
                              .ThenInclude(x => x.Agenda)
                          .Where(x => x.UserId == UserId && x.Id != id);
                    
            
            if (Filters != null)
            {
                if (Filters[((int)FilterTypeEnum.TaskActive)] || Filters[((int)FilterTypeEnum.TaskPaused)] ||
                    Filters[((int)FilterTypeEnum.TaskSolved)] || Filters[((int)FilterTypeEnum.TaskCanceled)])
                {
                    serviceQuery = 
                        serviceQuery.Where(x => (Filters[((int)FilterTypeEnum.TaskActive)] && x.State == BlockWorkflowStateEnum.Active) ||
                                                (Filters[((int)FilterTypeEnum.TaskPaused)] && x.State == BlockWorkflowStateEnum.Paused) ||
                                                (Filters[((int)FilterTypeEnum.TaskSolved)] && x.State == BlockWorkflowStateEnum.SolvedByUser) ||
                                                (Filters[((int)FilterTypeEnum.TaskCanceled)] && x.State == BlockWorkflowStateEnum.Canceled));
                    taskQuery = 
                        taskQuery.Where(x => (Filters[((int)FilterTypeEnum.TaskActive)] && x.State == BlockWorkflowStateEnum.Active) ||
                                             (Filters[((int)FilterTypeEnum.TaskPaused)] && x.State == BlockWorkflowStateEnum.Paused) ||
                                             (Filters[((int)FilterTypeEnum.TaskSolved)] && x.State == BlockWorkflowStateEnum.Solved) ||
                                             (Filters[((int)FilterTypeEnum.TaskCanceled)] && x.State == BlockWorkflowStateEnum.Canceled));
                }
                else
                {
                    serviceQuery = serviceQuery.Where(x => x.State != BlockWorkflowStateEnum.Canceled);
                    taskQuery = taskQuery.Where(x => x.State != BlockWorkflowStateEnum.Canceled);
                }
            }

            List<TaskAllDTO> tasks = await serviceQuery.Select(x => new TaskAllDTO
                                                       {
                                                          AgendaId = x.Workflow.AgendaId,
                                                          AgendaName = x.Workflow.Agenda.Name,
                                                          Id = x.Id,
                                                          Priority = TaskPriorityEnum.Urgent,
                                                          TaskName = x.BlockModel.Name,
                                                          WorkflowId = x.WorkflowId,
                                                          WorkflowName = x.Workflow.Name,
                                                          Type = TaskTypeEnum.ServiceTask,
                                                          State = x.State,
                                                       })
                                                       .ToListAsync();
                                                     
            tasks.AddRange(await taskQuery.Select(x => new TaskAllDTO
                                          {
                                             AgendaId = x.Workflow.AgendaId,
                                             AgendaName = x.Workflow.Agenda.Name,
                                             SolveDate = x.SolveDate,
                                             Id = x.Id,
                                             Priority = x.Priority,
                                             TaskName = x.BlockModel.Name,
                                             WorkflowId = x.WorkflowId,
                                             WorkflowName = x.Workflow.Name,
                                             Type = TaskTypeEnum.UserTask,
                                             State = x.State,
                                          })
                                          .OrderBy(x => x.Priority)
                                               .ThenBy(x => x.SolveDate)
                                          .ToListAsync());
            return tasks;
        }

        public Task<BlockWorkflowEntity> SendEventForResend(Guid id)
        {
            return _dbSet.Include(x => x.BlockModel)
                            .ThenInclude(x => x.Pool)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<UserIdNameDTO> WorkflowAdmin(Guid id)
        {
            return _dbSet.Include(x => x.Workflow)
                            .ThenInclude(x => x.Administrator)
                         .Where(x => x.Id == id)
                         .Select(x => x.Workflow.Administrator)
                         .Select(x => new UserIdNameDTO
                         {
                             FullName = $"{x.Title} {x.Name} {x.Surname}",
                             Id = x.Id
                         })
                         .FirstAsync();
        }

        public Task<List<BlockWorkflowEntity>> AllOfState(Guid workflowId, BlockWorkflowStateEnum state)
        {
            return _dbSet.Where(x => x.WorkflowId == workflowId && x.State == state)
                         .ToListAsync();
        }

        public Task<BlockWorkflowEntity> Bare(Guid id)
        {
            return _dbSet.FirstAsync(x => x.Id == id);
        }

        public Task<Guid?> TaskUserId<T>(Guid id, DbSet<T> set) where T : TaskWorkflowEntity
        {
            return set.Where(x => x.Id == id)
                      .Select(x => x.UserId)
                      .FirstOrDefaultAsync();
        }

        public Task<Guid?> ServiceTaskUserId(Guid id)
        {
            return TaskUserId(id, _serviceTasks);
        }

        public Task<Guid?> UserTaskUserId(Guid id)
        {
            return TaskUserId(id, _userTasks);
        }

        public async Task<TaskAllDTO> Selected(Guid id)
        {
            if (await _serviceTasks.AnyAsync(x => x.Id == id))
            {
                return await _serviceTasks.Include(x => x.BlockModel)
                                          .Include(x => x.Workflow)
                                             .ThenInclude(x => x.Agenda)
                                          .Where(x => x.UserId == UserId && x.Id == id)
                                          .Select(x => new TaskAllDTO
                                          {
                                             AgendaId = x.Workflow.AgendaId,
                                             AgendaName = x.Workflow.Agenda.Name,
                                             Id = x.Id,
                                             Priority = TaskPriorityEnum.Urgent,
                                             TaskName = x.BlockModel.Name,
                                             WorkflowId = x.WorkflowId,
                                             WorkflowName = x.Workflow.Name,
                                             Type = TaskTypeEnum.ServiceTask,
                                             State = x.State,
                                          })
                                          .FirstAsync();
            }
            else
            {
                return await _userTasks.Include(x => x.BlockModel)
                                       .Include(x => x.Workflow)
                                          .ThenInclude(x => x.Agenda)
                                       .Where(x => x.UserId == UserId && x.Id == id)
                                       .Select(x => new TaskAllDTO
                                       {
                                          AgendaId = x.Workflow.AgendaId,
                                          AgendaName = x.Workflow.Agenda.Name,
                                          SolveDate = x.SolveDate,
                                          Id = x.Id,
                                          Priority = x.Priority,
                                          TaskName = x.BlockModel.Name,
                                          WorkflowId = x.WorkflowId,
                                          WorkflowName = x.Workflow.Name,
                                          Type = TaskTypeEnum.UserTask,
                                          State = x.State
                                       })
                                       .FirstAsync();
            }
        }

        public Task<BlockWorkflowEntity> Bare(Guid workflowId, Guid blockModelId)
        {
            return _dbSet.FirstAsync(x => x.WorkflowId == workflowId && x.BlockModelId == blockModelId);
        }

        public Task<BlockWorkflowEntity> BareWorkflow(Guid blockId, Guid workflowId)
        {
            return _dbSet.Include(x => x.Workflow)
                         .Include(x => x.BlockModel)
                         .FirstAsync(x => x.BlockModelId == blockId && x.WorkflowId == workflowId);
        }

        public Task<bool> Any(Guid workflowId, Guid blockModelId)
        {
            return _dbSet.AnyAsync(x => x.WorkflowId == workflowId && x.BlockModelId == blockModelId);
        }

        public Task<List<RecieveMessageEventWorkflowEntity>> RecieveMessageEvents(Guid blockId)
        {
            return _context.Set<RecieveMessageEventWorkflowEntity>()
                           .Include(x => x.Workflow)
                           .Include(x => x.BlockModel)
                                .ThenInclude(x => x.Pool)
                           .Where(x => x.Workflow.State == WorkflowStateEnum.Active && x.BlockModelId == blockId)
                           .ToListAsync();
        }

        public Task<List<RecieveSignalEventWorkflowEntity>> RecieveSignalEvents(Guid blockId)
        {
            return _context.Set<RecieveSignalEventWorkflowEntity>()
                           .Include(x => x.Workflow)
                           .Include(x => x.BlockModel)
                                .ThenInclude(x => x.Pool)
                           .Where(x => x.Workflow.State == WorkflowStateEnum.Active && x.BlockModelId == blockId)
                           .ToListAsync();
        }

        public Task<List<RecieveMessageEventWorkflowEntity>> RecieveMessageEvents(Guid workflowId, Guid blockId)
        {
            return _context.Set<RecieveMessageEventWorkflowEntity>()
                           .Include(x => x.BlockModel)
                                .ThenInclude(x => x.Pool)
                           .Where(x => x.WorkflowId == workflowId && x.BlockModelId == blockId)
                           .ToListAsync();
        }

        public Task<BlockWorkflowEntity> TaskForSolving(Guid id)
        {
            return _dbSet.Include(x => x.BlockModel)
                            .ThenInclude(x => x.Pool)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<List<BlockWorkflowEntity>> NextBlocks(Guid id, Guid workflowId)
        {
            return _dbSet.Include(x => x.BlockModel)
                            .ThenInclude(x => x.OutFlows)
                                .ThenInclude(x => x.InBlock)
                                    .ThenInclude(x => x.BlockWorkflows)
                                        .ThenInclude(x => x.Workflow)
                         .Where(x => x.Id == id)
                         .SelectMany(x => x.BlockModel.OutFlows)
                         .Select(x => x.InBlock)
                         .SelectMany(x => x.BlockWorkflows)
                         .Where(x => x.WorkflowId == workflowId)
                         .ToListAsync();
        }

        public Task<ServiceTaskDetailDTO> ServiceDetail(Guid id)
        {
            return _serviceTasks.Include(x => x.Workflow)
                                .ThenInclude(x => x.Agenda)
                             .Include(x => x.BlockModel)
                             .Where(x => x.UserId == UserId)
                             .Select(x => new ServiceTaskDetailDTO
                             {
                                 AgendaId = x.Workflow.AgendaId,
                                 AgendaName = x.Workflow.Agenda.Name,
                                 Description = x.BlockModel.Description,
                                 Id = x.Id,
                                 WorkflowId = x.WorkflowId,
                                 WorkflowName = x.Workflow.Name,
                                 TaskName = x.BlockModel.Name,
                                 State = x.State,
                                 FailedResponse = x.FailedResponse
                             })
                             .FirstAsync(x => x.Id == id);
        }

        public Task<List<BlockWorkflowActivityDTO>> BlockActivity(Guid poolId, Guid workflowId)
        {
            return _dbSet.Include(x => x.BlockModel)
                         .Where(x => x.WorkflowId == workflowId && x.BlockModel.PoolId == poolId)
                         .Select(x => new BlockWorkflowActivityDTO
                         {
                             State = x.State,
                             BlockModelId = x.BlockModelId,
                             WorkflowId = x.WorkflowId
                         })
                         .ToListAsync();
        }

        public Task<UserTaskWorkflowEntity> Detail(Guid id)
        {
            return _userTasks.Include(x => x.InputData).ThenInclude(x => x.TaskData).FirstAsync(x => x.Id == id);
        }

        public Task<BlockWorkflowEntity> DataService(Guid id)
        {
            return _dbSet.Include(x => x.OutputData)
                            .ThenInclude(x => x.Schema)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<BlockWorkflowEntity> DataUser(Guid id)
        {
            return _dbSet.Include(x => x.OutputData)
                            .ThenInclude(x => x.Attribute)
                         .Include(x => x.BlockModel)
                            .ThenInclude(x => x.Attributes)
                                .ThenInclude(x => x.MappedBlocks)
                                    .ThenInclude(x => x.Attribute)
                                        .ThenInclude(x => x.Data)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<UserTaskDetailDTO> UserDetail(Guid id)
        {
            return _userTasks.Include(x => x.Workflow)
                                .ThenInclude(x => x.Agenda)
                             .Include(x => x.BlockModel)
                             .Where(x => x.UserId == UserId)
                             .Select(x => new UserTaskDetailDTO
                             {
                                 AgendaId = x.Workflow.AgendaId,
                                 AgendaName = x.Workflow.Agenda.Name,
                                 Description = x.BlockModel.Description,
                                 Id = x.Id,
                                 Priority = x.Priority,
                                 WorkflowId = x.WorkflowId,
                                 WorkflowName = x.Workflow.Name,
                                 SolveDate = x.SolveDate,
                                 TaskName = x.BlockModel.Name,
                                 State = x.State
                             })
                             .FirstAsync(x => x.Id == id);
        }

        public Task<bool> IsInModel(Guid modelId, Guid id)
        {
            return _dbSet.Include(x => x.BlockModel)
                            .ThenInclude(x => x.Pool)
                         .AnyAsync(x => x.Id == id && x.BlockModel.Pool.ModelId == modelId);
        }
    }
}
