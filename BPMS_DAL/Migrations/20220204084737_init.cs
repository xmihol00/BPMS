﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BPMS_DAL.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceType = table.Column<int>(type: "int", nullable: false),
                    Serialization = table.Column<int>(type: "int", nullable: false),
                    HttpMethod = table.Column<int>(type: "int", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SolvingRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SolvingRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Systems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UniqueName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObtainedName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ObtainedKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Systems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Agendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AdministratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Agendas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Agendas_Users_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemRoles",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemRoles", x => new { x.UserId, x.Role });
                    table.ForeignKey(
                        name: "FK_SystemRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AgendaRoles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AgendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendaRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgendaRoles_Agendas_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Agendas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AgendaRoles_SolvingRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SolvingRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgendaRoles_Users_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SVG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Models_Agendas_AgendaId",
                        column: x => x.AgendaId,
                        principalTable: "Agendas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemAgendas",
                columns: table => new
                {
                    AgendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemAgendas", x => new { x.AgendaId, x.SystemId });
                    table.ForeignKey(
                        name: "FK_SystemAgendas_Agendas_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Agendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemAgendas_Systems_AgendaId",
                        column: x => x.AgendaId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Pools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pools", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Pools_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    AgendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Workflows_Agendas_AgendaId",
                        column: x => x.AgendaId,
                        principalTable: "Agendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Workflows_Models_ModelId",
                        column: x => x.ModelId,
                        principalTable: "Models",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlockModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockModel_Pools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "Pools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SystemsPool",
                columns: table => new
                {
                    SystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemsPool", x => new { x.PoolId, x.SystemId });
                    table.ForeignKey(
                        name: "FK_SystemsPool_Pools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "Pools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemsPool_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockSchemas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Compulsory = table.Column<bool>(type: "bit", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockSchemas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockSchemas_BlockModel_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BlockModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockSchemas_BlockSchemas_ParentId",
                        column: x => x.ParentId,
                        principalTable: "BlockSchemas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlockWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    SolvedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockWorkflows_BlockModel_BlockModelId",
                        column: x => x.BlockModelId,
                        principalTable: "BlockModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockWorkflows_Workflows_WorkflowId",
                        column: x => x.WorkflowId,
                        principalTable: "Workflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EndEventsModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndEventsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndEventsModel_BlockModel_Id",
                        column: x => x.Id,
                        principalTable: "BlockModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ExclusiveGatewaysModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Condition = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExclusiveGatewaysModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExclusiveGatewaysModel_BlockModel_Id",
                        column: x => x.Id,
                        principalTable: "BlockModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Flows",
                columns: table => new
                {
                    InBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flows", x => new { x.InBlockId, x.OutBlockId });
                    table.ForeignKey(
                        name: "FK_Flows_BlockModel_InBlockId",
                        column: x => x.InBlockId,
                        principalTable: "BlockModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Flows_BlockModel_OutBlockId",
                        column: x => x.OutBlockId,
                        principalTable: "BlockModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ParallelGatewaysModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ParallelGatewaysModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ParallelGatewaysModel_BlockModel_Id",
                        column: x => x.Id,
                        principalTable: "BlockModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SendEventsModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendEventsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendEventsModel_BlockModel_Id",
                        column: x => x.Id,
                        principalTable: "BlockModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceTasksModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTasksModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTasksModel_BlockModel_Id",
                        column: x => x.Id,
                        principalTable: "BlockModel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceTasksModel_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StartEventsModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StartEventsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StartEventsModel_BlockModel_Id",
                        column: x => x.Id,
                        principalTable: "BlockModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserTasksModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Span = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTasksModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTasksModel_BlockModel_Id",
                        column: x => x.Id,
                        principalTable: "BlockModel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserTasksModel_SolvingRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SolvingRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlockData",
                columns: table => new
                {
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockData", x => new { x.BlockId, x.SchemaId });
                    table.ForeignKey(
                        name: "FK_BlockData_BlockSchemas_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "BlockSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockData_BlockWorkflows_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserTasksWorkflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolveDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTasksWorkflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTasksWorkflow_BlockWorkflows_Id",
                        column: x => x.Id,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserTasksWorkflow_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConditionData",
                columns: table => new
                {
                    ExclusiveGatewayId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataSchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConditionData", x => new { x.ExclusiveGatewayId, x.DataSchemaId });
                    table.ForeignKey(
                        name: "FK_ConditionData_BlockSchemas_DataSchemaId",
                        column: x => x.DataSchemaId,
                        principalTable: "BlockSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ConditionData_ExclusiveGatewaysModel_ExclusiveGatewayId",
                        column: x => x.ExclusiveGatewayId,
                        principalTable: "ExclusiveGatewaysModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RecieveEventsModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecieveEventsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecieveEventsModel_BlockModel_Id",
                        column: x => x.Id,
                        principalTable: "BlockModel",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecieveEventsModel_SendEventsModel_SenderId",
                        column: x => x.SenderId,
                        principalTable: "SendEventsModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArraysBlocks",
                columns: table => new
                {
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArraysBlocks", x => new { x.BlockId, x.SchemaId });
                    table.ForeignKey(
                        name: "FK_ArraysBlocks_BlockData_BlockId_SchemaId",
                        columns: x => new { x.BlockId, x.SchemaId },
                        principalTable: "BlockData",
                        principalColumns: new[] { "BlockId", "SchemaId" });
                    table.ForeignKey(
                        name: "FK_ArraysBlocks_BlockSchemas_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "BlockSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ArraysBlocks_BlockWorkflows_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BoolBlocks",
                columns: table => new
                {
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoolBlocks", x => new { x.BlockId, x.SchemaId });
                    table.ForeignKey(
                        name: "FK_BoolBlocks_BlockData_BlockId_SchemaId",
                        columns: x => new { x.BlockId, x.SchemaId },
                        principalTable: "BlockData",
                        principalColumns: new[] { "BlockId", "SchemaId" });
                    table.ForeignKey(
                        name: "FK_BoolBlocks_BlockSchemas_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "BlockSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BoolBlocks_BlockWorkflows_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NumberBlocks",
                columns: table => new
                {
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberBlocks", x => new { x.BlockId, x.SchemaId });
                    table.ForeignKey(
                        name: "FK_NumberBlocks_BlockData_BlockId_SchemaId",
                        columns: x => new { x.BlockId, x.SchemaId },
                        principalTable: "BlockData",
                        principalColumns: new[] { "BlockId", "SchemaId" });
                    table.ForeignKey(
                        name: "FK_NumberBlocks_BlockSchemas_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "BlockSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NumberBlocks_BlockWorkflows_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StringBlocks",
                columns: table => new
                {
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringBlocks", x => new { x.BlockId, x.SchemaId });
                    table.ForeignKey(
                        name: "FK_StringBlocks_BlockData_BlockId_SchemaId",
                        columns: x => new { x.BlockId, x.SchemaId },
                        principalTable: "BlockData",
                        principalColumns: new[] { "BlockId", "SchemaId" });
                    table.ForeignKey(
                        name: "FK_StringBlocks_BlockSchemas_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "BlockSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StringBlocks_BlockWorkflows_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password", "PhoneNumber", "Surname", "UserName" },
                values: new object[] { new Guid("442c2de7-eb92-44f9-acf1-41d5dade854a"), "spravce.system@test.cz", "Správce", "", "", "System", "spravce" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password", "PhoneNumber", "Surname", "UserName" },
                values: new object[] { new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"), "admin.system@test.cz", "Admin", "", "", "System", "admin" });

            migrationBuilder.InsertData(
                table: "SystemRoles",
                columns: new[] { "Role", "UserId" },
                values: new object[] { 1, new Guid("442c2de7-eb92-44f9-acf1-41d5dade854a") });

            migrationBuilder.InsertData(
                table: "SystemRoles",
                columns: new[] { "Role", "UserId" },
                values: new object[] { 0, new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4") });

            migrationBuilder.InsertData(
                table: "SystemRoles",
                columns: new[] { "Role", "UserId" },
                values: new object[] { 1, new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4") });

            migrationBuilder.CreateIndex(
                name: "IX_AgendaRoles_RoleId",
                table: "AgendaRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_AdministratorId",
                table: "Agendas",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_ArraysBlocks_SchemaId",
                table: "ArraysBlocks",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockData_SchemaId",
                table: "BlockData",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockModel_PoolId",
                table: "BlockModel",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockSchemas_BlockId",
                table: "BlockSchemas",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockSchemas_ParentId",
                table: "BlockSchemas",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockWorkflows_BlockModelId",
                table: "BlockWorkflows",
                column: "BlockModelId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockWorkflows_WorkflowId",
                table: "BlockWorkflows",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_BoolBlocks_SchemaId",
                table: "BoolBlocks",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionData_DataSchemaId",
                table: "ConditionData",
                column: "DataSchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_OutBlockId",
                table: "Flows",
                column: "OutBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_AgendaId",
                table: "Models",
                column: "AgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_NumberBlocks_SchemaId",
                table: "NumberBlocks",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_Pools_ModelId",
                table: "Pools",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_RecieveEventsModel_SenderId",
                table: "RecieveEventsModel",
                column: "SenderId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasksModel_ServiceId",
                table: "ServiceTasksModel",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_StringBlocks_SchemaId",
                table: "StringBlocks",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAgendas_SystemId",
                table: "SystemAgendas",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemsPool_SystemId",
                table: "SystemsPool",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasksModel_RoleId",
                table: "UserTasksModel",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasksWorkflow_UserId",
                table: "UserTasksWorkflow",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_AgendaId",
                table: "Workflows",
                column: "AgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_ModelId",
                table: "Workflows",
                column: "ModelId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AgendaRoles");

            migrationBuilder.DropTable(
                name: "ArraysBlocks");

            migrationBuilder.DropTable(
                name: "BoolBlocks");

            migrationBuilder.DropTable(
                name: "ConditionData");

            migrationBuilder.DropTable(
                name: "EndEventsModel");

            migrationBuilder.DropTable(
                name: "Flows");

            migrationBuilder.DropTable(
                name: "NumberBlocks");

            migrationBuilder.DropTable(
                name: "ParallelGatewaysModel");

            migrationBuilder.DropTable(
                name: "RecieveEventsModel");

            migrationBuilder.DropTable(
                name: "ServiceTasksModel");

            migrationBuilder.DropTable(
                name: "StartEventsModel");

            migrationBuilder.DropTable(
                name: "StringBlocks");

            migrationBuilder.DropTable(
                name: "SystemAgendas");

            migrationBuilder.DropTable(
                name: "SystemRoles");

            migrationBuilder.DropTable(
                name: "SystemsPool");

            migrationBuilder.DropTable(
                name: "UserTasksModel");

            migrationBuilder.DropTable(
                name: "UserTasksWorkflow");

            migrationBuilder.DropTable(
                name: "ExclusiveGatewaysModel");

            migrationBuilder.DropTable(
                name: "SendEventsModel");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "BlockData");

            migrationBuilder.DropTable(
                name: "Systems");

            migrationBuilder.DropTable(
                name: "SolvingRoles");

            migrationBuilder.DropTable(
                name: "BlockSchemas");

            migrationBuilder.DropTable(
                name: "BlockWorkflows");

            migrationBuilder.DropTable(
                name: "BlockModel");

            migrationBuilder.DropTable(
                name: "Workflows");

            migrationBuilder.DropTable(
                name: "Pools");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Agendas");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}