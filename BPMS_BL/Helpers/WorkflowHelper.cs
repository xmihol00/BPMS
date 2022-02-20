
using BPMS_Common;
using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
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

                    uTask.Data = CrateTaskData(await blockAttributeRepository.All(nextBlock.Id));
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

        private static List<TaskDataEntity> CrateTaskData(List<BlockAttributeAllDTO> blockAttributes)
        {
            List<TaskDataEntity> data = new List<TaskDataEntity>();
            foreach(BlockAttributeAllDTO attribute in blockAttributes)
            {
                data.Add(CreateTaskData(attribute.Type));
            }

            return data;
        }

        private static TaskDataEntity CreateTaskData(AttributeTypeEnum type)
        {
            switch (type)
            {
                case AttributeTypeEnum.String:
                    return new StringDataEntity();

                case AttributeTypeEnum.Number:
                    return new NumberDataEntity();
                
                case AttributeTypeEnum.Text:
                    return new TextDataEntity();

                case AttributeTypeEnum.YesNo:
                    return new BoolDataEntity();
                
                case AttributeTypeEnum.File:
                    return new FileDataEntity();
                
                case AttributeTypeEnum.Selection:
                    return new SelectionDataEntity();
                
                default:
                    return new TaskDataEntity();
            }
        }
    }
}
