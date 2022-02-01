﻿// <auto-generated />
using System;
using BPMS_DAL;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace BPMS_DAL.Migrations
{
    [DbContext(typeof(BpmsDbContext))]
    partial class BpmsDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("BPMS_DAL.Entities.AgendaEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AdministratorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AdministratorId");

                    b.ToTable("Agendas");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.AgendaRoleUserEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AgendaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("UserId", "RoleId", "AgendaId");

                    b.HasIndex("RoleId");

                    b.ToTable("AgendaRoles");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataEntity", b =>
                {
                    b.Property<Guid>("BlockId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SchemaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("BlockId", "SchemaId");

                    b.HasIndex("SchemaId");

                    b.ToTable("BlockData");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataSchemaEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("BlockId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Compulsory")
                        .HasColumnType("bit");

                    b.Property<int>("DataType")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ParentId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("BlockId");

                    b.HasIndex("ParentId");

                    b.ToTable("BlockSchemas");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockModelEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("PoolId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("PoolId");

                    b.ToTable("BlockModel");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockWorkflowEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<bool>("Active")
                        .HasColumnType("bit");

                    b.Property<Guid>("BlockModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("SolvedDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("WorkflowId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("BlockModelId");

                    b.HasIndex("WorkflowId");

                    b.ToTable("BlockWorkflows");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ConditionDataEntity", b =>
                {
                    b.Property<Guid>("ExclusiveGatewayId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("DataSchemaId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("ExclusiveGatewayId", "DataSchemaId");

                    b.HasIndex("DataSchemaId");

                    b.ToTable("ConditionData");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.FlowEntity", b =>
                {
                    b.Property<Guid>("InBlockId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("OutBlockId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.HasKey("InBlockId", "OutBlockId");

                    b.HasIndex("OutBlockId");

                    b.ToTable("Flows");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("AgendaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("SVG")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AgendaId");

                    b.ToTable("Models");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.PoolEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("ModelId");

                    b.ToTable("Pools");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ServiceEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("HttpMethod")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("Serialization")
                        .HasColumnType("int");

                    b.Property<int>("ServiceType")
                        .HasColumnType("int");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Services");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SolvingRoleEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("SolvingRoles");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SystemAgendaEntity", b =>
                {
                    b.Property<Guid>("AgendaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("AgendaId", "SystemId");

                    b.HasIndex("SystemId");

                    b.ToTable("SystemAgendas");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SystemEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ObtainedKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ObtainedName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("URL")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UniqueName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Systems");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SystemPoolEntity", b =>
                {
                    b.Property<Guid>("PoolId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("SystemId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("PoolId", "SystemId");

                    b.HasIndex("SystemId");

                    b.ToTable("SystemsPool");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SystemRoleEntity", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<int>("Role")
                        .HasColumnType("int");

                    b.HasKey("UserId", "Role");

                    b.ToTable("SystemRoles");

                    b.HasData(
                        new
                        {
                            UserId = new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                            Role = 0
                        },
                        new
                        {
                            UserId = new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                            Role = 1
                        },
                        new
                        {
                            UserId = new Guid("442c2de7-eb92-44f9-acf1-41d5dade854a"),
                            Role = 1
                        });
                });

            modelBuilder.Entity("BPMS_DAL.Entities.UserEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Surname")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"),
                            Email = "admin.system@test.cz",
                            Name = "Admin",
                            Password = "",
                            PhoneNumber = "",
                            Surname = "System",
                            UserName = "admin"
                        },
                        new
                        {
                            Id = new Guid("442c2de7-eb92-44f9-acf1-41d5dade854a"),
                            Email = "spravce.system@test.cz",
                            Name = "Správce",
                            Password = "",
                            PhoneNumber = "",
                            Surname = "System",
                            UserName = "spravce"
                        });
                });

            modelBuilder.Entity("BPMS_DAL.Entities.WorkflowEntity", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AgendaId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("ModelId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AgendaId");

                    b.HasIndex("ModelId");

                    b.ToTable("Workflows");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataTypes.ArrayBlockEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockDataEntity");

                    b.Property<int>("Type")
                        .HasColumnType("int");

                    b.ToTable("ArraysBlocks", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataTypes.BoolBlockEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockDataEntity");

                    b.Property<bool?>("Value")
                        .HasColumnType("bit");

                    b.ToTable("BoolBlocks", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataTypes.NumberBlockEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockDataEntity");

                    b.Property<double?>("Value")
                        .HasColumnType("float");

                    b.ToTable("NumberBlocks", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataTypes.StringBlockEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockDataEntity");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("StringBlocks", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.EndEventModelEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockModelEntity");

                    b.ToTable("EndEventsModel", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.ExclusiveGatewayModelEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockModelEntity");

                    b.Property<string>("Condition")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.ToTable("ExclusiveGatewaysModel", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.ParallelGatewayModelEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockModelEntity");

                    b.ToTable("ParallelGatewaysModel", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.RecieveEventModelEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockModelEntity");

                    b.Property<Guid?>("SenderId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("SenderId");

                    b.ToTable("RecieveEventsModel", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.SendEventModelEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockModelEntity");

                    b.ToTable("SendEventsModel", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.ServiceTaskModelEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockModelEntity");

                    b.Property<Guid?>("ServiceId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("ServiceId");

                    b.ToTable("ServiceTasksModel", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.StartEventModelEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockModelEntity");

                    b.ToTable("StartEventsModel", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.UserTaskModelEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockModelEntity");

                    b.Property<Guid?>("RoleId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<TimeSpan>("Span")
                        .HasColumnType("time");

                    b.HasIndex("RoleId");

                    b.ToTable("UserTasksModel", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.WorkflowBlocks.UserTaskWorkflowEntity", b =>
                {
                    b.HasBaseType("BPMS_DAL.Entities.BlockWorkflowEntity");

                    b.Property<DateTime>("SolveDate")
                        .HasColumnType("datetime2");

                    b.Property<Guid>("UserId")
                        .HasColumnType("uniqueidentifier");

                    b.HasIndex("UserId");

                    b.ToTable("UserTasksWorkflow", (string)null);
                });

            modelBuilder.Entity("BPMS_DAL.Entities.AgendaEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.UserEntity", "Administrator")
                        .WithMany("Agendas")
                        .HasForeignKey("AdministratorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Administrator");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.AgendaRoleUserEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.AgendaEntity", "Agenda")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.SolvingRoleEntity", "Role")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.UserEntity", "User")
                        .WithMany("UserRoles")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agenda");

                    b.Navigation("Role");

                    b.Navigation("User");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockWorkflowEntity", "Block")
                        .WithMany("BlockData")
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.BlockDataSchemaEntity", "Schema")
                        .WithMany("Data")
                        .HasForeignKey("SchemaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Block");

                    b.Navigation("Schema");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataSchemaEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", "Block")
                        .WithMany("DataSchemas")
                        .HasForeignKey("BlockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.BlockDataSchemaEntity", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.NoAction);

                    b.Navigation("Block");

                    b.Navigation("Parent");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.PoolEntity", "Pool")
                        .WithMany("Blocks")
                        .HasForeignKey("PoolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pool");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockWorkflowEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", "BlockModel")
                        .WithMany("BlockWorkflows")
                        .HasForeignKey("BlockModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.WorkflowEntity", "Workflow")
                        .WithMany("Blocks")
                        .HasForeignKey("WorkflowId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("BlockModel");

                    b.Navigation("Workflow");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ConditionDataEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockDataSchemaEntity", "DataSchema")
                        .WithMany("Conditions")
                        .HasForeignKey("DataSchemaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.ModelBlocks.ExclusiveGatewayModelEntity", "ExclusiveGateway")
                        .WithMany("Conditions")
                        .HasForeignKey("ExclusiveGatewayId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("DataSchema");

                    b.Navigation("ExclusiveGateway");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.FlowEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", "InBlock")
                        .WithMany("InFlows")
                        .HasForeignKey("InBlockId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", "OutBlock")
                        .WithMany("OutFlows")
                        .HasForeignKey("OutBlockId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("InBlock");

                    b.Navigation("OutBlock");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.AgendaEntity", "Agenda")
                        .WithMany("Models")
                        .HasForeignKey("AgendaId");

                    b.Navigation("Agenda");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.PoolEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.ModelEntity", "Model")
                        .WithMany("Pools")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Model");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SystemAgendaEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.SystemEntity", "System")
                        .WithMany("Agendas")
                        .HasForeignKey("AgendaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.AgendaEntity", "Agenda")
                        .WithMany("Systems")
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Agenda");

                    b.Navigation("System");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SystemPoolEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.PoolEntity", "Pool")
                        .WithMany("Systems")
                        .HasForeignKey("PoolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.SystemEntity", "System")
                        .WithMany("Pools")
                        .HasForeignKey("SystemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pool");

                    b.Navigation("System");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SystemRoleEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.UserEntity", "User")
                        .WithMany("Roles")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.WorkflowEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.AgendaEntity", "Agenda")
                        .WithMany("Workflows")
                        .HasForeignKey("AgendaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.ModelEntity", "Model")
                        .WithMany("Workflows")
                        .HasForeignKey("ModelId")
                        .OnDelete(DeleteBehavior.NoAction)
                        .IsRequired();

                    b.Navigation("Agenda");

                    b.Navigation("Model");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataTypes.ArrayBlockEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockDataEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.BlockDataTypes.ArrayBlockEntity", "BlockId", "SchemaId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataTypes.BoolBlockEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockDataEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.BlockDataTypes.BoolBlockEntity", "BlockId", "SchemaId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataTypes.NumberBlockEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockDataEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.BlockDataTypes.NumberBlockEntity", "BlockId", "SchemaId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataTypes.StringBlockEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockDataEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.BlockDataTypes.StringBlockEntity", "BlockId", "SchemaId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.EndEventModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.ModelBlocks.EndEventModelEntity", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.ExclusiveGatewayModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.ModelBlocks.ExclusiveGatewayModelEntity", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.ParallelGatewayModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.ModelBlocks.ParallelGatewayModelEntity", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.RecieveEventModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.ModelBlocks.RecieveEventModelEntity", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.ModelBlocks.SendEventModelEntity", "Sender")
                        .WithMany("Recievers")
                        .HasForeignKey("SenderId");

                    b.Navigation("Sender");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.SendEventModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.ModelBlocks.SendEventModelEntity", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.ServiceTaskModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.ModelBlocks.ServiceTaskModelEntity", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.ServiceEntity", "Service")
                        .WithMany("ServiceTasks")
                        .HasForeignKey("ServiceId");

                    b.Navigation("Service");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.StartEventModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.ModelBlocks.StartEventModelEntity", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.UserTaskModelEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockModelEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.ModelBlocks.UserTaskModelEntity", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.SolvingRoleEntity", "Role")
                        .WithMany("UserTasks")
                        .HasForeignKey("RoleId");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.WorkflowBlocks.UserTaskWorkflowEntity", b =>
                {
                    b.HasOne("BPMS_DAL.Entities.BlockWorkflowEntity", null)
                        .WithOne()
                        .HasForeignKey("BPMS_DAL.Entities.WorkflowBlocks.UserTaskWorkflowEntity", "Id")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("BPMS_DAL.Entities.UserEntity", "User")
                        .WithMany("Tasks")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.AgendaEntity", b =>
                {
                    b.Navigation("Models");

                    b.Navigation("Systems");

                    b.Navigation("UserRoles");

                    b.Navigation("Workflows");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockDataSchemaEntity", b =>
                {
                    b.Navigation("Children");

                    b.Navigation("Conditions");

                    b.Navigation("Data");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockModelEntity", b =>
                {
                    b.Navigation("BlockWorkflows");

                    b.Navigation("DataSchemas");

                    b.Navigation("InFlows");

                    b.Navigation("OutFlows");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.BlockWorkflowEntity", b =>
                {
                    b.Navigation("BlockData");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelEntity", b =>
                {
                    b.Navigation("Pools");

                    b.Navigation("Workflows");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.PoolEntity", b =>
                {
                    b.Navigation("Blocks");

                    b.Navigation("Systems");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ServiceEntity", b =>
                {
                    b.Navigation("ServiceTasks");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SolvingRoleEntity", b =>
                {
                    b.Navigation("UserRoles");

                    b.Navigation("UserTasks");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.SystemEntity", b =>
                {
                    b.Navigation("Agendas");

                    b.Navigation("Pools");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.UserEntity", b =>
                {
                    b.Navigation("Agendas");

                    b.Navigation("Roles");

                    b.Navigation("Tasks");

                    b.Navigation("UserRoles");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.WorkflowEntity", b =>
                {
                    b.Navigation("Blocks");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.ExclusiveGatewayModelEntity", b =>
                {
                    b.Navigation("Conditions");
                });

            modelBuilder.Entity("BPMS_DAL.Entities.ModelBlocks.SendEventModelEntity", b =>
                {
                    b.Navigation("Recievers");
                });
#pragma warning restore 612, 618
        }
    }
}
