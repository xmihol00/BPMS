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

namespace BPMS_DAL.Repositories
{
    public class BlockWorkflowRepository : BaseRepository<BlockWorkflowEntity>
    {
        private readonly DbSet<TaskWorkflowEntity> _userTasks;
        private readonly DbSet<ServiceWorkflowEntity> _serviceTasks;
        public BlockWorkflowRepository(BpmsDbContext context) : base(context) 
        {
            _userTasks = context.Set<TaskWorkflowEntity>();
            _serviceTasks = context.Set<ServiceWorkflowEntity>();
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
    }
}
