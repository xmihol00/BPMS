
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
using BPMS_DTOs.ServiceDataSchema;

namespace BPMS_BL.Helpers
{
    public static class WorkflowHelper
    {
        public static async Task CreateWorkflow(ModelEntity model, WorkflowRepository workflowRepository,
                                                AgendaRoleRepository agendaRoleRepository,
                                                BlockAttributeRepository blockAttributeRepository,
                                                ServiceDataSchemaRepository serviceDataSchemaRepository)
        {
            model.State = ModelStateEnum.Executable;
            Guid agendaAdminId = model.Agenda.AdministratorId;
            BlockModelEntity startEvent = model.Pools
                                               .First(x => x.SystemId == StaticData.ThisSystemId)
                                               .Blocks
                                               .First(x => x.GetType() == typeof(StartEventModelEntity));
            
            List<BlockWorkflowEntity> blocks = await CreateBlocks(agendaAdminId, startEvent,
                                                                  blockAttributeRepository, serviceDataSchemaRepository);
            
            blocks[0].SolvedDate = DateTime.Now;
            blocks[1].Active = true;
            if (blocks[1] is IUserTaskWorkflowEntity)
            {
                IUserTaskModelEntity taskModel = startEvent.OutFlows[1].OutBlock as IUserTaskModelEntity;
                IUserTaskWorkflowEntity taskWorkflow = blocks[1] as IUserTaskWorkflowEntity;
                taskWorkflow.UserId = await agendaRoleRepository.LeastBussyUser(taskModel.RoleId ?? Guid.Empty) ?? agendaAdminId;
                taskWorkflow.SolveDate = DateTime.Now.AddDays(taskModel.Difficulty.TotalDays);
            }
            else if (blocks[1] is IUserTaskWorkflowEntity)
            {
                IServiceTaskModelEntity serviceModel = startEvent.OutFlows[1].OutBlock as IServiceTaskModelEntity;
                (blocks[1] as IServiceTaskWorkflowEntity).UserId = await agendaRoleRepository
                                                               .LeastBussyUser(serviceModel.RoleId ?? Guid.Empty) ?? agendaAdminId;
            }

            WorkflowEntity workflow = await workflowRepository.Waiting(model.Id);
            workflow.State = WorkflowStateEnum.Active;
            workflow.Blocks = blocks;
        }

        private static async Task<List<BlockWorkflowEntity>> CreateBlocks(Guid agendaAdminId, BlockModelEntity blockModel,
                                                                          BlockAttributeRepository blockAttributeRepository,
                                                                          ServiceDataSchemaRepository serviceDataSchemaRepository)
        {
            List<BlockWorkflowEntity> blocks = new List<BlockWorkflowEntity>();
            blocks.Add(await CreateBlock(agendaAdminId, blockModel, blockAttributeRepository, serviceDataSchemaRepository));
            foreach (FlowEntity outFlows in blockModel.OutFlows)
            {
                blocks.AddRange(await CreateBlocks(agendaAdminId, outFlows.InBlock, blockAttributeRepository, serviceDataSchemaRepository));
            }

            return blocks;
        }

        private static async Task<BlockWorkflowEntity> CreateBlock(Guid agendaAdminId, BlockModelEntity blockModel,
                                                                   BlockAttributeRepository blockAttributeRepository,
                                                                   ServiceDataSchemaRepository serviceDataSchemaRepository)
        {
            BlockWorkflowEntity blockWorkflow;
            switch (blockModel)
            {
                case IUserTaskModelEntity userTask:
                    blockWorkflow = new UserTaskWorkflowEntity()
                    {
                        UserId = agendaAdminId,
                        Priority = TaskPriorityEnum.Medium
                    };

                    IUserTaskWorkflowEntity uTask = blockWorkflow as IUserTaskWorkflowEntity;
                    uTask.OutputData = CrateUserTaskData(await blockAttributeRepository.All(blockModel.Id));
                    break;

                case IServiceTaskModelEntity serviceTask:
                    blockWorkflow = new ServiceTaskWorkflowEntity()
                    {
                        UserId = agendaAdminId
                    };

                    IServiceTaskWorkflowEntity sTask = blockWorkflow as IServiceTaskWorkflowEntity;
                    sTask.OutputData = CrateServiceTaskData(await serviceDataSchemaRepository.All(serviceTask.ServiceId));
                    break;

                default:
                    blockWorkflow = new BlockWorkflowEntity();
                    break;
            }

            blockWorkflow.Active = true;
            blockWorkflow.BlockModelId = blockModel.Id;

            return blockWorkflow;
        }

        private static List<TaskDataEntity> CrateServiceTaskData(List<DataSchemaBareDTO> dataSchemaEntities)
        {
            List<TaskDataEntity> data = new List<TaskDataEntity>();
            foreach(DataSchemaBareDTO attribute in dataSchemaEntities)
            {
                data.Add(CreateServiceTaskData(attribute.Type));
            }

            return data;
        }

        private static TaskDataEntity CreateServiceTaskData(DataTypeEnum type)
        {
            switch (type)
            {
                case DataTypeEnum.String:
                    return new StringDataEntity();

                case DataTypeEnum.Number:
                    return new NumberDataEntity();
                
                case DataTypeEnum.Bool:
                    return new BoolDataEntity();
                                
                default:
                    return new TaskDataEntity();
            }
        }

        private static List<TaskDataEntity> CrateUserTaskData(List<BlockAttributeAllDTO> blockAttributes)
        {
            List<TaskDataEntity> data = new List<TaskDataEntity>();
            foreach(BlockAttributeAllDTO attribute in blockAttributes)
            {
                data.Add(CreateUserTaskData(attribute.Type));
            }

            return data;
        }

        private static TaskDataEntity CreateUserTaskData(AttributeTypeEnum type)
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
                
                case AttributeTypeEnum.Select:
                    return new SelectDataEntity();
                
                case AttributeTypeEnum.Date:
                    return new DateDataEntity();
                
                default:
                    return new TaskDataEntity();
            }
        }
    }
}
