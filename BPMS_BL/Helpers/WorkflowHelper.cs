
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DAL.Interfaces.ModelBlocks;
using BPMS_DAL.Interfaces.WorkflowBlocks;
using BPMS_DAL.Repositories;
using BPMS_DTOs.BlockAttribute;

namespace BPMS_BL.Helpers
{
    public static class WorkflowHelper
    {
        public static async Task CreateWorkflow(ModelEntity model, WorkflowRepository workflowRepository,
                                                AgendaRoleUserRepository agendaRoleUserRepository,
                                                BlockAttributeRepository blockAttributeRepository)
        {
            Guid agendaId = model.AgendaId ?? Guid.Empty;
            BlockModelEntity startEvent = model.Pools
                                               .First(x => x.SystemId == StaticData.ThisSystemId)
                                               .Blocks
                                               .First(x => x.GetType() == typeof(StartEventModelEntity));



            BlockWorkflowEntity startEventWorkflow = new BlockWorkflowEntity()
            {
                Active = false,
                BlockModelId = startEvent.Id,
                SolvedDate = DateTime.Now,
            };

            BlockModelEntity nextBlock = startEvent.OutFlows.First().InBlock;
            BlockWorkflowEntity blockWorkflow = await CreateBlock(model.Agenda.AdministratorId, agendaId, nextBlock,
                                                                  agendaRoleUserRepository, blockAttributeRepository);

            WorkflowEntity workflow = new WorkflowEntity()
            {
                Name = $"NEW: {model.Name} {DateTime.Now.ToString("dd. MM. yyyy HH:mm")}",
                AgendaId = agendaId,
                ModelId = model.Id,
                State = WorkflowStateEnum.Active,
                Blocks = new List<BlockWorkflowEntity>()
                {
                    startEventWorkflow,
                    blockWorkflow
                }
            };

            await workflowRepository.Create(workflow);
            await workflowRepository.Save();
        }

        private static async Task<BlockWorkflowEntity> CreateBlock(Guid agendaAdminId, Guid agendaId, BlockModelEntity nextBlock,
                                                                   AgendaRoleUserRepository agendaRoleUserRepository,
                                                                   BlockAttributeRepository blockAttributeRepository)
        {
            BlockWorkflowEntity blockWorkflow;
            switch (nextBlock)
            {
                case IUserTaskModelEntity userTask:
                    Guid roleId = userTask.RoleId ?? Guid.Empty;
                    blockWorkflow = new TaskWorkflowEntity()
                    {
                        SolveDate = DateTime.Now.AddDays(userTask.Difficulty.TotalDays),
                        UserId = await agendaRoleUserRepository.LeastBussyUser(agendaId, roleId)
                    };

                    ITaskWorkflowEntity uTask = blockWorkflow as ITaskWorkflowEntity;
                    if (uTask.UserId == Guid.Empty)
                    {
                        uTask.Id = agendaAdminId;
                    }

                    uTask.Data = await CrateBlockData(await blockAttributeRepository.All(nextBlock.Id));
                    break;

                case IServiceTaskModelEntity serviceTask:
                    blockWorkflow = new BlockWorkflowEntity(); // TODO
                    break;

                default:
                    blockWorkflow = new BlockWorkflowEntity();
                    break;
            }

            blockWorkflow.Active = true;
            blockWorkflow.BlockModelId = nextBlock.Id;

            return blockWorkflow;
        }

        private static async Task<List<TaskDataEntity>> CrateBlockData(List<BlockAttributeAllDTO> blockAttributes)
        {
            throw new NotImplementedException();
        }
    }
}
