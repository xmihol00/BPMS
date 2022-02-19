﻿using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
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
        public DbSet<AgendaRoleUserEntity>? AgendaRoles { get; set; }
        public DbSet<TaskDataEntity>? TaskData { get; set; }
        public DbSet<BlockAttributeEntity>? Attributes { get; set; }
        public DbSet<BlockAttributeMapEntity>? AttributesMap { get; set; }
        public DbSet<BlockModelEntity>? BlockModel { get; set; }
        public DbSet<BlockWorkflowEntity>? BlockWorkflows { get; set; }
        public DbSet<ConditionDataEntity>? ConditionData { get; set; }
        public DbSet<FlowEntity>? Flows { get; set; }
        public DbSet<ModelEntity>? Models { get; set; }
        public DbSet<PoolEntity>? Pools { get; set; }
        public DbSet<ServiceEntity>? Services { get; set; }
        public DbSet<ServiceHeaderEntity>? Headers { get; set; }
        public DbSet<ServiceDataSchemaEntity>? ServiceSchemas { get; set; }
        public DbSet<SolvingRoleEntity>? SolvingRoles { get; set; }
        public DbSet<SystemAgendaEntity>? SystemAgendas { get; set; }
        public DbSet<SystemEntity>? Systems { get; set; }
        public DbSet<SystemRoleEntity>? SystemRoles { get; set; }
        public DbSet<UserEntity>? Users { get; set; }
        public DbSet<WorkflowEntity>? Workflows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SystemEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<SystemEntity>().HasIndex(x => x.URL).IsUnique(true);

            modelBuilder.Entity<SystemAgendaEntity>().HasKey(x => new { x.AgendaId, x.SystemId });
            modelBuilder.Entity<SystemAgendaEntity>().HasOne(x => x.Agenda).WithMany(x => x.Systems).HasForeignKey(x => x.AgendaId);
            modelBuilder.Entity<SystemAgendaEntity>().HasOne(x => x.System).WithMany(x => x.Agendas).HasForeignKey(x => x.SystemId);

            modelBuilder.Entity<UserEntity>().HasKey(x => x.Id);

            modelBuilder.Entity<SystemRoleEntity>().HasKey(x => new { x.UserId, x.Role });
            modelBuilder.Entity<SystemRoleEntity>().HasOne(x => x.User).WithMany(x => x.Roles).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<AgendaEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AgendaEntity>().HasOne(x => x.Administrator).WithMany(x => x.Agendas).HasForeignKey(x => x.AdministratorId);

            modelBuilder.Entity<AgendaRoleUserEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AgendaRoleUserEntity>().HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<AgendaRoleUserEntity>().HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<AgendaRoleUserEntity>().HasOne(x => x.Agenda).WithMany(x => x.UserRoles).HasForeignKey(x => x.AgendaId).OnDelete(DeleteBehavior.NoAction);

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
            modelBuilder.Entity<UserTaskModelEntity>().ToTable("UserTasksModel");
            modelBuilder.Entity<RecieveEventModelEntity>().ToTable("RecieveEventsModel");
            modelBuilder.Entity<ServiceTaskModelEntity>().ToTable("ServiceTasksModel");
            modelBuilder.Entity<ParallelGatewayModelEntity>().ToTable("ParallelGatewaysModel");
            modelBuilder.Entity<ExclusiveGatewayModelEntity>().ToTable("ExclusiveGatewaysModel");
            modelBuilder.Entity<SendEventModelEntity>().ToTable("SendEventsModel");
            modelBuilder.Entity<StartEventModelEntity>().ToTable("StartEventsModel");
            modelBuilder.Entity<EndEventModelEntity>().ToTable("EndEventsModel");

            modelBuilder.Entity<BlockAttributeEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<BlockAttributeEntity>().HasOne(x => x.Block).WithMany(x => x.Attributes).HasForeignKey(x => x.BlockId);

            modelBuilder.Entity<FlowEntity>().HasKey(x => new { x.InBlockId, x.OutBlockId });
            modelBuilder.Entity<FlowEntity>().HasOne(x => x.InBlock).WithMany(x => x.InFlows).HasForeignKey(x => x.InBlockId).OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<FlowEntity>().HasOne(x => x.OutBlock).WithMany(x => x.OutFlows).HasForeignKey(x => x.OutBlockId).OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<WorkflowEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<WorkflowEntity>().HasOne(x => x.Agenda).WithMany(x => x.Workflows).HasForeignKey(x => x.AgendaId);
            modelBuilder.Entity<WorkflowEntity>().HasOne(x => x.Model).WithMany(x => x.Workflows).HasForeignKey(x => x.ModelId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<ServiceDataSchemaEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ServiceDataSchemaEntity>().HasOne(x => x.Service).WithMany(x => x.DataSchemas).HasForeignKey(x => x.ServiceId);
            modelBuilder.Entity<ServiceDataSchemaEntity>().HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId).OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<BlockWorkflowEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<BlockWorkflowEntity>().HasOne(x => x.Workflow).WithMany(x => x.Blocks).HasForeignKey(x => x.WorkflowId).OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<BlockWorkflowEntity>().HasOne(x => x.BlockModel).WithMany(x => x.BlockWorkflows).HasForeignKey(x => x.BlockModelId);
            modelBuilder.Entity<TaskWorkflowEntity>().HasOne(x => x.User).WithMany(x => x.Tasks).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<TaskWorkflowEntity>().ToTable("TasksWorkflow");

            modelBuilder.Entity<TaskDataEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<TaskDataEntity>().HasOne(x => x.Task).WithMany(x => x.TaskData).HasForeignKey(x => x.TaskId);
            modelBuilder.Entity<TaskDataEntity>().HasOne(x => x.Attribute).WithMany(x => x.Data).HasForeignKey(x => x.AttributeId);
            modelBuilder.Entity<TaskDataEntity>().HasOne(x => x.Schema).WithMany(x => x.Data).HasForeignKey(x => x.SchemaId);
            modelBuilder.Entity<BoolBlockEntity>().ToTable("BoolBlocks");
            modelBuilder.Entity<NumberBlockEntity>().ToTable("NumberBlocks");
            modelBuilder.Entity<StringBlockEntity>().ToTable("StringBlocks");
            modelBuilder.Entity<ArrayBlockEntity>().ToTable("ArrayBlocks");
            modelBuilder.Entity<FileBlockEntity>().ToTable("FileBlocks");            

            modelBuilder.Entity<ConditionDataEntity>().HasKey(x => new { x.ExclusiveGatewayId, x.DataSchemaId });
            modelBuilder.Entity<ConditionDataEntity>().HasOne(x => x.ExclusiveGateway).WithMany(x => x.Conditions);
            modelBuilder.Entity<ConditionDataEntity>().HasOne(x => x.DataSchema).WithMany(x => x.Conditions);

            modelBuilder.Entity<ServiceEntity>().HasKey(x => x.Id);

            modelBuilder.Entity<ServiceHeaderEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ServiceHeaderEntity>().HasOne(x => x.Service).WithMany(x => x.Headers).HasForeignKey(x => x.ServiceId);

            modelBuilder.Entity<BlockAttributeMapEntity>().HasKey(x => new { x.AttributeId, x.BlockId });
            modelBuilder.Entity<BlockAttributeMapEntity>().HasOne(x => x.Block).WithMany(x => x.MappedAttributes).HasForeignKey(x => x.BlockId).OnDelete(DeleteBehavior.ClientCascade);
            modelBuilder.Entity<BlockAttributeMapEntity>().HasOne(x => x.Attribute).WithMany(x => x.MappedBlocks).HasForeignKey(x => x.AttributeId).OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.SeedUsers();
            modelBuilder.SeedSystemRoles();
            modelBuilder.SeedSystems();
        }
    }
}
