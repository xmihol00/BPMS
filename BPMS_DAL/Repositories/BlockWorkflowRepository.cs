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

namespace BPMS_DAL.Repositories
{
    public class BlockWorkflowRepository : BaseRepository<BlockWorkflowEntity>
    {
        private readonly DbSet<UserTaskWorkflowEntity> _userTasks;
        private readonly DbSet<ServiceTaskWorkflowEntity> _serviceTasks;
        public BlockWorkflowRepository(BpmsDbContext context) : base(context) 
        {
            _userTasks = context.Set<UserTaskWorkflowEntity>();
            _serviceTasks = context.Set<ServiceTaskWorkflowEntity>();
        }

        public async Task<List<TaskAllDTO>> Overview(Guid userId)
        {
            List<TaskAllDTO> tasks = await _serviceTasks.Include(x => x.BlockModel)
                                                        .Include(x => x.Workflow)
                                                           .ThenInclude(x => x.Agenda)
                                                        .Where(x => x.UserId == userId && x.Active == true)
                                                        .Select(x => new TaskAllDTO
                                                        {
                                                           AgendaId = x.Workflow.AgendaId,
                                                           AgendaName = x.Workflow.Agenda.Name,
                                                           Description = x.BlockModel.Description,
                                                           SolveDate = DateTime.MinValue,
                                                           Id = x.Id,
                                                           Priority = TaskPriorityEnum.Urgent,
                                                           TaskName = x.BlockModel.Name,
                                                           WorkflowId = x.WorkflowId,
                                                           WorkflowName = x.Workflow.Name,
                                                           Type = TaskTypeEnum.UserTask
                                                        })
                                                        .ToListAsync();
                                                     
            tasks.AddRange(await _userTasks.Include(x => x.BlockModel)
                                           .Include(x => x.Workflow)
                                              .ThenInclude(x => x.Agenda)
                                           .Where(x => x.UserId == userId && x.Active == true)
                                           .Select(x => new TaskAllDTO
                                           {
                                              AgendaId = x.Workflow.AgendaId,
                                              AgendaName = x.Workflow.Agenda.Name,
                                              Description = x.BlockModel.Description,
                                              SolveDate = x.SolveDate,
                                              Id = x.Id,
                                              Priority = x.Priority,
                                              TaskName = x.BlockModel.Name,
                                              WorkflowId = x.WorkflowId,
                                              WorkflowName = x.Workflow.Name,
                                              Type = TaskTypeEnum.UserTask
                                           })
                                           .OrderBy(x => x.Priority)
                                                .ThenBy(x => x.SolveDate)
                                           .ToListAsync());
            return tasks;
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
                                .ThenInclude(x => x.MappedAttributes)
                                    .ThenInclude(x => x.Attribute)
                                        .ThenInclude(x => x.Data)
                         .FirstAsync(x => x.Id == id);
        }

        public Task<UserTaskDetailDTO> UserDetail(Guid id, Guid userId)
        {
            return _userTasks.Include(x => x.Workflow)
                                .ThenInclude(x => x.Agenda)
                             .Include(x => x.BlockModel)
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
                             })
                             .FirstAsync(x => x.Id == id);
        }
    }
}
