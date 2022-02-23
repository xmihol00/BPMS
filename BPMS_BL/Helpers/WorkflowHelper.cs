
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
    public class WorkflowHelper
    {
        private readonly ModelRepository _modelRepository;
        private readonly WorkflowRepository _workflowRepository;
        private readonly AgendaRoleRepository _agendaRoleRepository;
        private readonly BlockAttributeRepository _blockAttributeRepository;
        private readonly ServiceDataSchemaRepository _serviceDataSchemaRepository;
        private Dictionary<Guid, BlockWorkflowEntity> _createdUserTasks = new Dictionary<Guid, BlockWorkflowEntity>();
        private Dictionary<Guid, TaskDataEntity> _createdServiceData = new Dictionary<Guid, TaskDataEntity>();

        public WorkflowHelper(ModelRepository modelRepository, WorkflowRepository workflowRepository, AgendaRoleRepository agendaRoleRepository,
                              BlockAttributeRepository blockAttributeRepository, ServiceDataSchemaRepository serviceDataSchemaRepository)
        {
            _modelRepository = modelRepository;
            _workflowRepository = workflowRepository;
            _agendaRoleRepository = agendaRoleRepository;
            _blockAttributeRepository = blockAttributeRepository;
            _serviceDataSchemaRepository = serviceDataSchemaRepository;
        }

        public async Task CreateWorkflow(Guid id)
        {
            ModelEntity model = await _modelRepository.DetailToCreateWF(id);
            model.State = ModelStateEnum.Executable;
            BlockModelEntity startEvent = model.Pools.First(x => x.SystemId == StaticData.ThisSystemId)
                                               .Blocks.First(x => x is IStartEventModelEntity);

            List<BlockWorkflowEntity> blocks = await CreateBlocks(startEvent);

            blocks[0].SolvedDate = DateTime.Now;
            await ExecuteFirstBlock(startEvent.OutFlows[0].InBlock, blocks[1], model.Agenda.Id);

            WorkflowEntity workflow = await _workflowRepository.Waiting(model.Id);
            workflow.State = WorkflowStateEnum.Active;
            workflow.Blocks = blocks;
        }

        private async Task ExecuteFirstBlock(BlockModelEntity blockModel, BlockWorkflowEntity blockWorkflow, Guid agendaAdminId)
        {
            blockWorkflow.Active = true;
            if (blockWorkflow is IUserTaskWorkflowEntity)
            {
                IUserTaskModelEntity taskModel = blockModel as IUserTaskModelEntity;
                IUserTaskWorkflowEntity taskWorkflow = blockWorkflow as IUserTaskWorkflowEntity;
                taskWorkflow.UserId = await _agendaRoleRepository.LeastBussyUser(taskModel.RoleId ?? Guid.Empty) ?? agendaAdminId;
                taskWorkflow.SolveDate = DateTime.Now.AddDays(taskModel.Difficulty.TotalDays);
            }
            else if (blockWorkflow is IUserTaskWorkflowEntity)
            {
                IServiceTaskModelEntity serviceModel = blockModel as IServiceTaskModelEntity;
                (blockWorkflow as IServiceTaskWorkflowEntity).UserId =
                        await _agendaRoleRepository.LeastBussyUser(serviceModel.RoleId ?? Guid.Empty) ?? agendaAdminId;
            }
        }

        private async Task<List<BlockWorkflowEntity>> CreateBlocks(BlockModelEntity blockModel)
        {
            List<BlockWorkflowEntity> blocks = new List<BlockWorkflowEntity>();
            blocks.Add(await CreateBlock(blockModel));
            foreach (FlowEntity outFlows in blockModel.OutFlows)
            {
                blocks.AddRange(await CreateBlocks(outFlows.InBlock));
            }

            return blocks;
        }

        private async Task<BlockWorkflowEntity> CreateBlock(BlockModelEntity blockModel)
        {
            BlockWorkflowEntity blockWorkflow;
            switch (blockModel)
            {
                case IUserTaskModelEntity:
                    blockWorkflow = CreateUserTask(blockModel);
                    break;

                case IServiceTaskModelEntity serviceTask:
                    blockWorkflow = new ServiceTaskWorkflowEntity()
                    {
                        UserId = null
                    };

                    IServiceTaskWorkflowEntity sTask = blockWorkflow as IServiceTaskWorkflowEntity;
                    sTask.OutputData = CrateServiceTaskData(await _serviceDataSchemaRepository.AllWithMaps(serviceTask.ServiceId));
                    break;

                default:
                    blockWorkflow = new BlockWorkflowEntity();
                    break;
            }

            blockWorkflow.Active = true;
            blockWorkflow.BlockModelId = blockModel.Id;

            return blockWorkflow;
        }

        private BlockWorkflowEntity CreateUserTask(BlockModelEntity blockModel)
        {
            BlockWorkflowEntity blockWorkflow = new UserTaskWorkflowEntity()
            {
                UserId = null,
                Priority = TaskPriorityEnum.Medium
            };
            IUserTaskWorkflowEntity uTask = blockWorkflow as IUserTaskWorkflowEntity;

            _createdUserTasks[blockModel.Id] = blockWorkflow;
            foreach (BlockModelDataSchemaEntity schema in blockModel.DataSchemas)
            {
                TaskDataEntity? taskData = _createdServiceData.GetValueOrDefault(schema.DataSchemaId);
                if (taskData != null)
                {
                    blockWorkflow.InputData.Add(new TaskDataMapEntity
                    {
                        Task = blockWorkflow,
                        TaskData = taskData
                    });
                }
            }

            uTask.OutputData = CrateUserTaskData(blockModel.Attributes);
            return blockWorkflow;
        }

        private List<TaskDataEntity> CrateServiceTaskData(List<ServiceDataSchemaEntity> dataSchemas)
        {
            List<TaskDataEntity> data = new List<TaskDataEntity>();
            foreach(ServiceDataSchemaEntity attrib in dataSchemas)
            {
                TaskDataEntity taskData = CreateServiceTaskData(attrib.Type);
                data.Add(taskData);

                foreach (BlockModelDataSchemaEntity mappedAttrib in attrib.Blocks)
                {
                    BlockWorkflowEntity? blockWorkflow = _createdUserTasks.GetValueOrDefault(mappedAttrib.BlockId);
                    if (blockWorkflow != null)
                    {
                        blockWorkflow.InputData.Add(new TaskDataMapEntity
                        {
                            TaskData = taskData,
                            Task = blockWorkflow
                        });
                    }
                }
            }

            return data;
        }

        private TaskDataEntity CreateServiceTaskData(DataTypeEnum type)
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

        private List<TaskDataEntity> CrateUserTaskData(List<BlockAttributeEntity> blockAttributes)
        {
            List<TaskDataEntity> data = new List<TaskDataEntity>();
            foreach(BlockAttributeEntity attribute in blockAttributes)
            {
                TaskDataEntity taskData = CreateUserTaskData(attribute.Type);
                data.Add(taskData);

                foreach(BlockAttributeMapEntity mappedAttrib in attribute.MappedAttributes)
                {
                    BlockWorkflowEntity? blockWorkflow = _createdUserTasks.GetValueOrDefault(mappedAttrib.BlockId);
                    if (blockWorkflow != null)
                    {
                        blockWorkflow.InputData.Add(new TaskDataMapEntity
                        {
                            TaskData = taskData,
                            Task = blockWorkflow
                        });
                    }
                }
            }

            return data;
        }

        private TaskDataEntity CreateUserTaskData(AttributeTypeEnum type)
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
