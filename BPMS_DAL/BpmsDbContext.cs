using BPMS_Common.Enums;
using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DAL.Entities.MessageTypes;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Entities.WorkflowBlocks;
using BPMS_DAL.Seeds;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMS_DAL
{
    public class BpmsDbContext : DbContext
    {
        public BpmsDbContext(DbContextOptions<BpmsDbContext> options) : base(options) { }

        public DbSet<AgendaEntity>? Agendas { get; set; }
        public DbSet<AgendaRoleEntity>? AgendaRoles { get; set; }
        public DbSet<TaskDataEntity>? TaskDatas { get; set; }
        public DbSet<TaskDataMapEntity>? TaskDataMaps { get; set; }
        public DbSet<AttributeEntity>? Attributes { get; set; }
        public DbSet<AttributeMapEntity>? AttributesMaps { get; set; }
        public DbSet<BlockModelEntity>? BlockModels { get; set; }
        public DbSet<BlockWorkflowEntity>? BlockWorkflows { get; set; }
        public DbSet<ConditionDataEntity>? ConditionData { get; set; }
        public DbSet<FlowEntity>? Flows { get; set; }
        public DbSet<ForeignAttributeMapEntity>? ForeignAttributeMaps { get; set; }
        public DbSet<ForeignRecieveEventEntity>? ForeignRecieveEvents { get; set; }
        public DbSet<ForeignSendEventEntity>? ForeignSendEvents { get; set; }
        public DbSet<MessageEntity>? Message { get; set; }
        public DbSet<ModelEntity>? Models { get; set; }
        public DbSet<NotificationEntity>? Notifications { get; set; }
        public DbSet<PoolEntity>? Pools { get; set; }
        public DbSet<ServiceEntity>? Services { get; set; }
        public DbSet<ServiceHeaderEntity>? Headers { get; set; }
        public DbSet<DataSchemaEntity>? DataSchemas { get; set; }
        public DbSet<DataSchemaMapEntity>? DataSchemaMaps { get; set; }
        public DbSet<SolvingRoleEntity>? SolvingRoles { get; set; }
        public DbSet<SystemAgendaEntity>? SystemAgendas { get; set; }
        public DbSet<SystemEntity>? Systems { get; set; }
        public DbSet<SystemRoleEntity>? SystemRoles { get; set; }
        public DbSet<UserEntity>? Users { get; set; }
        public DbSet<UserRoleEntity>? UserRoles { get; set; }
        public DbSet<WorkflowEntity>? Workflows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SystemEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<SystemEntity>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<SystemEntity>().HasIndex(x => x.URL).IsUnique(true);

            modelBuilder.Entity<SystemAgendaEntity>().HasKey(x => new { x.AgendaId, x.SystemId });
            modelBuilder.Entity<SystemAgendaEntity>().HasOne(x => x.Agenda).WithMany(x => x.Systems).HasForeignKey(x => x.AgendaId);
            modelBuilder.Entity<SystemAgendaEntity>().HasOne(x => x.System).WithMany(x => x.Agendas).HasForeignKey(x => x.SystemId);

            modelBuilder.Entity<UserEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<UserEntity>().HasIndex(x => x.UserName).IsUnique(true);

            modelBuilder.Entity<SystemRoleEntity>().HasKey(x => new { x.UserId, x.Role });
            modelBuilder.Entity<SystemRoleEntity>().HasOne(x => x.User).WithMany(x => x.SystemRoles).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<FilterEntity>().HasKey(x => new { x.UserId, x.Filter });
            modelBuilder.Entity<FilterEntity>().HasOne(x => x.User).WithMany(x => x.Fitlers).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<AgendaEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AgendaEntity>().HasOne(x => x.Administrator).WithMany(x => x.Agendas).HasForeignKey(x => x.AdministratorId);

            modelBuilder.Entity<AgendaRoleEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AgendaRoleEntity>().HasOne(x => x.Role).WithMany(x => x.AgendaRoles).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<AgendaRoleEntity>().HasOne(x => x.Agenda).WithMany(x => x.AgendaRoles).HasForeignKey(x => x.AgendaId);

            modelBuilder.Entity<UserRoleEntity>().HasKey(x => new { x.AgendaRoleId, x.UserId });
            modelBuilder.Entity<UserRoleEntity>().HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<UserRoleEntity>().HasOne(x => x.AgendaRole).WithMany(x => x.UserRoles).HasForeignKey(x => x.AgendaRoleId).OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<SolvingRoleEntity>().HasKey(x => x.Id);

            modelBuilder.Entity<PoolEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<PoolEntity>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<PoolEntity>().HasOne(x => x.Model).WithMany(x => x.Pools).HasForeignKey(x => x.ModelId);
            modelBuilder.Entity<PoolEntity>().HasOne(x => x.System).WithMany(x => x.Pools).HasForeignKey(x => x.SystemId);

            modelBuilder.Entity<ModelEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ModelEntity>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<ModelEntity>().HasOne(x => x.Agenda).WithMany(x => x.Models).HasForeignKey(x => x.AgendaId);

            modelBuilder.Entity<BlockModelEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<BlockModelEntity>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<BlockModelEntity>().HasOne(x => x.Pool).WithMany(x => x.Blocks).HasForeignKey(x => x.PoolId);
            modelBuilder.Entity<UserTaskModelEntity>().HasOne(x => x.Role).WithMany(x => x.UserTasks).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<RecieveEventModelEntity>().HasOne(x => x.Sender).WithMany(x => x.Recievers).HasForeignKey(x => x.SenderId);
            modelBuilder.Entity<ServiceTaskModelEntity>().HasOne(x => x.Service).WithMany(x => x.ServiceTasks).HasForeignKey(x => x.ServiceId);
            modelBuilder.Entity<ServiceTaskModelEntity>().HasOne(x => x.Role).WithMany(x => x.ServiceTask).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<RecieveEventModelEntity>().HasOne(x => x.ForeignSender).WithOne(x => x.Reciever).HasForeignKey<RecieveEventModelEntity>(x => x.ForeignSenderId);
            modelBuilder.Entity<UserTaskModelEntity>().ToTable("UserTasksModel");
            modelBuilder.Entity<RecieveEventModelEntity>().ToTable("RecieveEventsModel");
            modelBuilder.Entity<ServiceTaskModelEntity>().ToTable("ServiceTasksModel");
            modelBuilder.Entity<ParallelGatewayModelEntity>().ToTable("ParallelGatewaysModel");
            modelBuilder.Entity<ExclusiveGatewayModelEntity>().ToTable("ExclusiveGatewaysModel");
            modelBuilder.Entity<SendEventModelEntity>().ToTable("SendEventsModel");
            modelBuilder.Entity<StartEventModelEntity>().ToTable("StartEventsModel");
            modelBuilder.Entity<EndEventModelEntity>().ToTable("EndEventsModel");

            modelBuilder.Entity<BlockModelDataSchemaEntity>().HasKey(x => new { x.BlockId, x.DataSchemaId, x.ServiceTaskId });
            modelBuilder.Entity<BlockModelDataSchemaEntity>().HasOne(x => x.Block).WithMany(x => x.DataSchemas).HasForeignKey(x => x.BlockId);
            modelBuilder.Entity<BlockModelDataSchemaEntity>().HasOne(x => x.DataSchema).WithMany(x => x.Blocks).HasForeignKey(x => x.DataSchemaId);
            modelBuilder.Entity<BlockModelDataSchemaEntity>().HasOne(x => x.ServiceTask).WithMany(x => x.Blocks).HasForeignKey(x => x.ServiceTaskId);

            modelBuilder.Entity<AttributeEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AttributeEntity>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<AttributeEntity>().HasOne(x => x.Block).WithMany(x => x.Attributes).HasForeignKey(x => x.BlockId);

            modelBuilder.Entity<FlowEntity>().HasKey(x => new { x.InBlockId, x.OutBlockId });
            modelBuilder.Entity<FlowEntity>().HasOne(x => x.InBlock).WithMany(x => x.InFlows).HasForeignKey(x => x.InBlockId).OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<FlowEntity>().HasOne(x => x.OutBlock).WithMany(x => x.OutFlows).HasForeignKey(x => x.OutBlockId).OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<WorkflowEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<WorkflowEntity>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<WorkflowEntity>().HasOne(x => x.Agenda).WithMany(x => x.Workflows).HasForeignKey(x => x.AgendaId);
            modelBuilder.Entity<WorkflowEntity>().HasOne(x => x.Model).WithMany(x => x.Workflows).HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<WorkflowEntity>().HasOne(x => x.Administrator).WithMany(x => x.Workflows).HasForeignKey(x => x.AdministratorId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DataSchemaEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<DataSchemaEntity>().HasOne(x => x.Service).WithMany(x => x.DataSchemas).HasForeignKey(x => x.ServiceId);
            modelBuilder.Entity<DataSchemaEntity>().HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<DataSchemaMapEntity>().HasKey(x => new { x.ServiceTaskId, x.SourceId, x.TargetId });
            modelBuilder.Entity<DataSchemaMapEntity>().HasOne(x => x.ServiceTask).WithMany(x => x.MappedSchemas).HasForeignKey(x => x.ServiceTaskId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<DataSchemaMapEntity>().HasOne(x => x.Target).WithMany(x => x.Targets).HasForeignKey(x => x.TargetId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<DataSchemaMapEntity>().HasOne(x => x.Source).WithMany(x => x.Sources).HasForeignKey(x => x.SourceId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BlockWorkflowEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<BlockWorkflowEntity>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<BlockWorkflowEntity>().HasOne(x => x.Workflow).WithMany(x => x.Blocks).HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<BlockWorkflowEntity>().HasOne(x => x.BlockModel).WithMany(x => x.BlockWorkflows).HasForeignKey(x => x.BlockModelId);
            modelBuilder.Entity<UserTaskWorkflowEntity>().HasOne(x => x.User).WithMany(x => x.Tasks).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<ServiceTaskWorkflowEntity>().HasOne(x => x.User).WithMany(x => x.Services).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserTaskWorkflowEntity>().ToTable("UserTasksWorkflow");
            modelBuilder.Entity<ServiceTaskWorkflowEntity>().ToTable("ServiceTasksWorkflow");
            modelBuilder.Entity<StartEventWorkflowEntity>().ToTable("StartEventsWorkflow");
            modelBuilder.Entity<EndEventWorkflowEntity>().ToTable("EndEventsWorkflow");
            modelBuilder.Entity<RecieveEventWorkflowEntity>().ToTable("RecieveEventsWorkflow");
            modelBuilder.Entity<SendEventWorkflowEntity>().ToTable("SendEventsWorkflow");

            modelBuilder.Entity<TaskDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<TaskDataEntity>().HasOne(x => x.OutputTask).WithMany(x => x.OutputData).HasForeignKey(x => x.OutputTaskId);
            modelBuilder.Entity<TaskDataEntity>().HasOne(x => x.Attribute).WithMany(x => x.Data).HasForeignKey(x => x.AttributeId);
            modelBuilder.Entity<TaskDataEntity>().HasOne(x => x.Schema).WithMany(x => x.Data).HasForeignKey(x => x.SchemaId);
            modelBuilder.Entity<BoolDataEntity>().ToTable("BoolTaskData");
            modelBuilder.Entity<NumberDataEntity>().ToTable("NumberTaskData");
            modelBuilder.Entity<StringDataEntity>().ToTable("StringTaskData");
            modelBuilder.Entity<TextDataEntity>().ToTable("TextTaskData");
            modelBuilder.Entity<ArrayDataEntity>().ToTable("ArrayTaskData");
            modelBuilder.Entity<FileDataEntity>().ToTable("FileTaskData");
            modelBuilder.Entity<SelectDataEntity>().ToTable("SelectTaskData");
            modelBuilder.Entity<DateDataEntity>().ToTable("DateTaskData");

            modelBuilder.Entity<TaskDataMapEntity>().HasKey(x => new { x.TaskDataId, x.TaskId });
            modelBuilder.Entity<TaskDataMapEntity>().HasOne(x => x.Task).WithMany(x => x.InputData).HasForeignKey(x => x.TaskId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<TaskDataMapEntity>().HasOne(x => x.TaskData).WithMany(x => x.InputData).HasForeignKey(x => x.TaskDataId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ConditionDataEntity>().HasKey(x => new { x.ExclusiveGatewayId, x.DataSchemaId });
            modelBuilder.Entity<ConditionDataEntity>().HasOne(x => x.ExclusiveGateway).WithMany(x => x.Conditions);
            modelBuilder.Entity<ConditionDataEntity>().HasOne(x => x.DataSchema).WithMany(x => x.Conditions);

            modelBuilder.Entity<ServiceEntity>().HasKey(x => x.Id);

            modelBuilder.Entity<ServiceHeaderEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ServiceHeaderEntity>().HasOne(x => x.Service).WithMany(x => x.Headers).HasForeignKey(x => x.ServiceId);

            modelBuilder.Entity<AttributeMapEntity>().HasKey(x => new { x.AttributeId, x.BlockId });
            modelBuilder.Entity<AttributeMapEntity>().HasOne(x => x.Block).WithMany(x => x.MappedAttributes).HasForeignKey(x => x.BlockId).OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<AttributeMapEntity>().HasOne(x => x.Attribute).WithMany(x => x.MappedBlocks).HasForeignKey(x => x.AttributeId).OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<MessageEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<MessageEntity>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<MessageEntity>().HasOne(x => x.System).WithMany(x => x.Messages).HasForeignKey(x => x.SystemId);
            modelBuilder.Entity<MessageEntity>().HasDiscriminator<MessageTypeEnum>(x => x.Type)
                        .HasValue<MessageEntity>(MessageTypeEnum.None)
                        .HasValue<AcceptationMessageEntity>(MessageTypeEnum.Acceptation);

            modelBuilder.Entity<ForeignRecieveEventEntity>().HasKey(x => new { x.SenderId, x.SystemId, x.ForeignBlockId });
            modelBuilder.Entity<ForeignRecieveEventEntity>().HasOne(x => x.Sender).WithMany(x => x.ForeignRecievers).HasForeignKey(x => x.SenderId);
            modelBuilder.Entity<ForeignRecieveEventEntity>().HasOne(x => x.System).WithMany(x => x.ForeignRecievers).HasForeignKey(x => x.SystemId);

            modelBuilder.Entity<ForeignSendEventEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ForeignSendEventEntity>().Property(x => x.Id).ValueGeneratedNever();
            modelBuilder.Entity<ForeignSendEventEntity>().HasOne(x => x.System).WithMany(x => x.ForeignSenedrs).HasForeignKey(x => x.SystemId);

            modelBuilder.Entity<ForeignAttributeMapEntity>().HasKey(x => new { x.AttributeId, x.ForeignSendEventId });
            modelBuilder.Entity<ForeignAttributeMapEntity>().HasOne(x => x.Attribute).WithOne(x => x.MappedForeignBlock).HasForeignKey<ForeignAttributeMapEntity>(x => x.AttributeId);
            modelBuilder.Entity<ForeignAttributeMapEntity>().HasOne(x => x.ForeignSendEvent).WithMany(x => x.MappedAttributes).HasForeignKey(x => x.ForeignSendEventId);

            modelBuilder.Entity<NotificationEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<NotificationEntity>().HasOne(x => x.User).WithMany(x => x.Notifications).HasForeignKey(x => x.UserId);

            modelBuilder.SeedUsers();
            modelBuilder.SeedSystemRoles();
            modelBuilder.SeedSystems();
            modelBuilder.SeedServices();
            modelBuilder.SeedDataSchemas();
        }
    }
}
