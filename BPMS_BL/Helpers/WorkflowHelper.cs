
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
            Guid agendaId = model.AgendaId ?? Guid.Empty;
            BlockModelEntity startEvent = model.Pools
                                               .First(x => x.SystemId == StaticData.ThisSystemId)
                                               .Blocks
                                               .First(x => x.GetType() == typeof(StartEventModelEntity));
            
            List<BlockWorkflowEntity> blocks = await CreateBlocks(model.Agenda.AdministratorId, agendaId, startEvent,
                                                                  agendaRoleRepository, blockAttributeRepository,
                                                                  serviceDataSchemaRepository);
            blocks[1].Active = true;

            WorkflowEntity workflow = await workflowRepository.Waiting(model.Id);
            workflow.State = WorkflowStateEnum.Active;
            workflow.Blocks = blocks;
        }

        private static async Task<List<BlockWorkflowEntity>> CreateBlocks(Guid agendaAdminId, Guid agendaId, BlockModelEntity blockModel,
                                                                          AgendaRoleRepository agendaRoleRepository,
                                                                          BlockAttributeRepository blockAttributeRepository,
                                                                          ServiceDataSchemaRepository serviceDataSchemaRepository)
        {
            List<BlockWorkflowEntity> blocks = new List<BlockWorkflowEntity>();
            blocks.Add(await CreateBlock(agendaAdminId, agendaId, blockModel, agendaRoleRepository, 
                                         blockAttributeRepository, serviceDataSchemaRepository));
            foreach (FlowEntity outFlows in blockModel.OutFlows)
            {
                blocks.AddRange(await CreateBlocks(agendaAdminId, agendaId, outFlows.InBlock, agendaRoleRepository, 
                                                   blockAttributeRepository, serviceDataSchemaRepository));
            }

            return blocks;
        }

        private static async Task<BlockWorkflowEntity> CreateBlock(Guid agendaAdminId, Guid agendaId, BlockModelEntity blockModel,
                                                                   AgendaRoleRepository agendaRoleRepository,
                                                                   BlockAttributeRepository blockAttributeRepository,
                                                                   ServiceDataSchemaRepository serviceDataSchemaRepository)
        {
            Guid roleId;
            BlockWorkflowEntity blockWorkflow;
            switch (blockModel)
            {
                case IUserTaskModelEntity userTask:
                    roleId = userTask.RoleId ?? Guid.Empty;
                    blockWorkflow = new TaskWorkflowEntity()
                    {
                        SolveDate = DateTime.Now.AddDays(userTask.Difficulty.TotalDays),
                        UserId = await agendaRoleRepository.LeastBussyUser(roleId) ?? agendaAdminId,
                        Priority = TaskPriorityEnum.Medium
                    };

                    ITaskWorkflowEntity uTask = blockWorkflow as ITaskWorkflowEntity;
                    uTask.Data = CrateUserTaskData(await blockAttributeRepository.All(blockModel.Id));
                    break;

                case IServiceTaskModelEntity serviceTask:
                    roleId = serviceTask.RoleId ?? Guid.Empty;
                    blockWorkflow = new ServiceWorkflowEntity()
                    {
                        UserId = await agendaRoleRepository.LeastBussyUser(roleId) ?? agendaAdminId
                    };

                    IServiceWorkflowEntity sTask = blockWorkflow as IServiceWorkflowEntity;
                    sTask.Data = CrateServiceTaskData(await serviceDataSchemaRepository.All(serviceTask.ServiceId));
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
                
                case AttributeTypeEnum.Selection:
                    return new SelectionDataEntity();
                
                default:
                    return new TaskDataEntity();
            }
        }
    }
}
