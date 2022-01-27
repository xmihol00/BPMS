using BPMS_DAL.Entities;
using BPMS_DAL.Entities.BlockDataTypes;
using BPMS_DAL.Entities.ModelBlocks;
using BPMS_DAL.Entities.WorkflowBlocks;
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
        public DbSet<BlockDataEntity>? BlockData { get; set; }
        public DbSet<BlockModelEntity>? BlockModel { get; set; }
        public DbSet<BlockWorkflowEntity>? BlockWorkflows { get; set; }
        public DbSet<ConditionDataEntity>? ConditionData { get; set; }
        public DbSet<FlowEntity>? Flows { get; set; }
        public DbSet<ModelEntity>? Models { get; set; }
        public DbSet<PoolEntity>? Pools { get; set; }
        public DbSet<ServiceEntity>? Services { get; set; }
        public DbSet<SolvingRoleEntity>? SolvingRoles { get; set; }
        public DbSet<SystemAgendaEntity>? SystemAgendas { get; set; }
        public DbSet<SystemEntity>? Systems { get; set; }
        public DbSet<SystemPoolEntity>? SystemsPool { get; set; }
        public DbSet<SystemRoleEntity>? SystemRoles { get; set; }
        public DbSet<UserEntity>? Users { get; set; }
        public DbSet<WorkflowEntity>? Workflows { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SystemEntity>().HasKey(x => x.Id);

            modelBuilder.Entity<SystemAgendaEntity>().HasKey(x => new { x.AgendaId, x.SystemId });
            modelBuilder.Entity<SystemAgendaEntity>().HasOne(x => x.Agenda).WithMany(x => x.Systems).HasForeignKey(x => x.SystemId);
            modelBuilder.Entity<SystemAgendaEntity>().HasOne(x => x.System).WithMany(x => x.Agendas).HasForeignKey(x => x.AgendaId);

            modelBuilder.Entity<SystemPoolEntity>().HasKey(x => new { x.PoolId, x.SystemId });
            modelBuilder.Entity<SystemPoolEntity>().HasOne(x => x.System).WithMany(x => x.Pools).HasForeignKey(x => x.SystemId);
            modelBuilder.Entity<SystemPoolEntity>().HasOne(x => x.Pool).WithMany(x => x.Systems).HasForeignKey(x => x.PoolId);

            modelBuilder.Entity<UserEntity>().HasKey(x => x.Id);

            modelBuilder.Entity<SystemRoleEntity>().HasKey(x => new { x.UserId, x.Role });
            modelBuilder.Entity<SystemRoleEntity>().HasOne(x => x.User).WithMany(x => x.Roles).HasForeignKey(x => x.UserId);

            modelBuilder.Entity<AgendaEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<AgendaEntity>().HasOne(x => x.Administrator).WithMany(x => x.Agendas).HasForeignKey(x => x.AdministratorId);

            modelBuilder.Entity<AgendaRoleUserEntity>().HasKey(x => new { x.UserId, x.RoleId, x.AgendaId });
            modelBuilder.Entity<AgendaRoleUserEntity>().HasOne(x => x.Role).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<AgendaRoleUserEntity>().HasOne(x => x.User).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<AgendaRoleUserEntity>().HasOne(x => x.Agenda).WithMany(x => x.UserRoles).HasForeignKey(x => x.RoleId);

            modelBuilder.Entity<SolvingRoleEntity>().HasKey(x => x.Id);

            modelBuilder.Entity<PoolEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<PoolEntity>().HasOne(x => x.Model).WithMany(x => x.Pools).HasForeignKey(x => x.ModelId);

            modelBuilder.Entity<ModelEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<ModelEntity>().HasOne(x => x.Agenda).WithMany(x => x.Models).HasForeignKey(x => x.AgendaId);

            modelBuilder.Entity<BlockModelEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<BlockModelEntity>().HasOne(x => x.Pool).WithMany(x => x.Blocks).HasForeignKey(x => x.PoolId);
            modelBuilder.Entity<UserTaskModelEntity>().HasOne(x => x.Role).WithMany(x => x.UserTasks).HasForeignKey(x => x.RoleId);
            modelBuilder.Entity<UserTaskModelEntity>().ToTable("UserTasksModel");
            modelBuilder.Entity<RecieveEventModelEntity>().HasOne(x => x.Sender).WithMany(x => x.Recievers).HasForeignKey(x => x.SenderId);
            modelBuilder.Entity<RecieveEventModelEntity>().ToTable("RecieveEventsModel");
            modelBuilder.Entity<ServiceTaskModelEntity>().HasOne(x => x.Service).WithMany(x => x.ServiceTasks).HasForeignKey(x => x.ServiceId);
            modelBuilder.Entity<ServiceTaskModelEntity>().ToTable("ServiceTasksModel");
            modelBuilder.Entity<InclusiveGatewayModelEntity>().ToTable("InclusiveGatewaiesModel");
            modelBuilder.Entity<SendeEventModelEntity>().ToTable("SendEventsModel");
            modelBuilder.Entity<InclusiveGatewayModelEntity>().ToTable("InclusiveGatewaiesModel");

            modelBuilder.Entity<FlowEntity>().HasKey(x => new { x.InBlockId, x.OutBlockId });
            modelBuilder.Entity<FlowEntity>().HasOne(x => x.InBlock).WithMany(x => x.InFlows).HasForeignKey(x => x.InBlockId);
            modelBuilder.Entity<FlowEntity>().HasOne(x => x.OutBlock).WithMany(x => x.OutFlows).HasForeignKey(x => x.OutBlockId);

            modelBuilder.Entity<WorkflowEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<WorkflowEntity>().HasOne(x => x.Agenda).WithMany(x => x.Workflows).HasForeignKey(x => x.AgendaId);
            modelBuilder.Entity<WorkflowEntity>().HasOne(x => x.Model).WithMany(x => x.Workflows).HasForeignKey(x => x.ModelId);

            modelBuilder.Entity<BlockDataSchemaEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<BlockDataSchemaEntity>().HasOne(x => x.Block).WithMany(x => x.DataSchemas).HasForeignKey(x => x.BlockId);
            modelBuilder.Entity<BlockDataSchemaEntity>().HasOne(x => x.Parent).WithMany(x => x.Children).HasForeignKey(x => x.ParentId);

            modelBuilder.Entity<BlockWorkflowEntity>().HasKey(x => x.Id);
            modelBuilder.Entity<BlockWorkflowEntity>().HasOne(x => x.Workflow).WithMany(x => x.Blocks).HasForeignKey(x => x.Id);
            modelBuilder.Entity<BlockWorkflowEntity>().HasOne(x => x.BlockModel).WithMany(x => x.BlockWorkflows).HasForeignKey(x => x.BlockModelId);
            modelBuilder.Entity<UserTaskWorkflowEntity>().HasOne(x => x.User).WithMany(x => x.Tasks).HasForeignKey(x => x.UserId);
            modelBuilder.Entity<UserTaskWorkflowEntity>().ToTable("UserTasksWorkflow");

            modelBuilder.Entity<BlockDataEntity>().HasKey(x => new { x.BlockId, x.SchemaId });
            modelBuilder.Entity<BlockDataEntity>().HasOne(x => x.Block).WithMany(x => x.BlockData).HasForeignKey(x => x.BlockId);
            modelBuilder.Entity<BlockDataEntity>().HasOne(x => x.Schema).WithMany(x => x.Data).HasForeignKey(x => x.SchemaId);
            modelBuilder.Entity<BoolBlockEntity>().ToTable("BoolBlocks");
            modelBuilder.Entity<NumberBlockEntity>().ToTable("NumberBlocks");
            modelBuilder.Entity<StringBlockEntity>().ToTable("StringBlocks");
            modelBuilder.Entity<ArrayBlockEntity>().ToTable("ArraysBlocks");

            modelBuilder.Entity<ConditionDataEntity>().HasKey(x => new { x.ExclusiveGatewayId, x.DataSchemaId });
            modelBuilder.Entity<ConditionDataEntity>().HasOne(x => x.ExclusiveGateway).WithMany(x => x.Conditions);
            modelBuilder.Entity<ConditionDataEntity>().HasOne(x => x.DataSchema).WithMany(x => x.Conditions);

            modelBuilder.Entity<ServiceEntity>().HasKey(x => x.Id);
        }
    }

}
