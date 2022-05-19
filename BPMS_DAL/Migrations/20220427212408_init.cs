using System;
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
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Serialization = table.Column<int>(type: "int", nullable: false),
                    HttpMethod = table.Column<int>(type: "int", nullable: false),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AuthType = table.Column<int>(type: "int", nullable: false),
                    AppId = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    AppSecret = table.Column<byte[]>(type: "varbinary(max)", nullable: true)
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
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    Key = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    URL = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Encryption = table.Column<int>(type: "int", nullable: false),
                    ForeignEncryption = table.Column<int>(type: "int", nullable: false)
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
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DataSchemas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alias = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StaticData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Compulsory = table.Column<bool>(type: "bit", nullable: false),
                    Array = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Direction = table.Column<int>(type: "int", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSchemas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DataSchemas_DataSchemas_ParentId",
                        column: x => x.ParentId,
                        principalTable: "DataSchemas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DataSchemas_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Headers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Headers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Headers_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AuditMessages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditMessages_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ConnectionRequests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SenderName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ForeignUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SenderEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SenderPhone = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConnectionRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConnectionRequests_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForeignSendEvents",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ForeignBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ForeignBlockName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignSendEvents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ForeignSendEvents_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Agendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
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
                name: "FilterEntity",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Filter = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilterEntity", x => new { x.UserId, x.Filter });
                    table.ForeignKey(
                        name: "FK_FilterEntity_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    Info = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
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
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AgendaRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AgendaRoles_Agendas_AgendaId",
                        column: x => x.AgendaId,
                        principalTable: "Agendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AgendaRoles_SolvingRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SolvingRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SVG = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_SystemAgendas_Agendas_AgendaId",
                        column: x => x.AgendaId,
                        principalTable: "Agendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SystemAgendas_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                columns: table => new
                {
                    AgendaRoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.AgendaRoleId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserRoles_AgendaRoles_AgendaRoleId",
                        column: x => x.AgendaRoleId,
                        principalTable: "AgendaRoles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Pools",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartedId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Pools_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false),
                    Start = table.Column<DateTime>(type: "datetime2", nullable: false),
                    End = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AgendaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdministratorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                    table.ForeignKey(
                        name: "FK_Workflows_Users_AdministratorId",
                        column: x => x.AdministratorId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Lanes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lanes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lanes_Pools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "Pools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Lanes_SolvingRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "SolvingRoles",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlockModels",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Order = table.Column<long>(type: "bigint", nullable: false),
                    PoolId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaneId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockModels_Lanes_LaneId",
                        column: x => x.LaneId,
                        principalTable: "Lanes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BlockModels_Pools_PoolId",
                        column: x => x.PoolId,
                        principalTable: "Pools",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockWorkflows",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    State = table.Column<int>(type: "int", nullable: false),
                    SolvedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    WorkflowId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlockModelId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockWorkflows", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlockWorkflows_BlockModels_BlockModelId",
                        column: x => x.BlockModelId,
                        principalTable: "BlockModels",
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
                        name: "FK_EndEventsModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
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
                        name: "FK_ExclusiveGatewaysModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Flows",
                columns: table => new
                {
                    InBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Flows", x => new { x.InBlockId, x.OutBlockId });
                    table.ForeignKey(
                        name: "FK_Flows_BlockModels_InBlockId",
                        column: x => x.InBlockId,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Flows_BlockModels_OutBlockId",
                        column: x => x.OutBlockId,
                        principalTable: "BlockModels",
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
                        name: "FK_ParallelGatewaysModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecieveMessageEventsModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecieveMessageEventsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecieveMessageEventsModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecieveSignalEventsModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ForeignSenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecieveSignalEventsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecieveSignalEventsModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RecieveSignalEventsModel_ForeignSendEvents_ForeignSenderId",
                        column: x => x.ForeignSenderId,
                        principalTable: "ForeignSendEvents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SendSignalEventsModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendSignalEventsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendSignalEventsModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceTasksModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    State = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTasksModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTasksModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
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
                        name: "FK_StartEventsModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserTasksModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Difficulty = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTasksModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTasksModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EndEventsWorkflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EndEventsWorkflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EndEventsWorkflow_BlockWorkflows_Id",
                        column: x => x.Id,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecieveMessageEventsWorkflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Delivered = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecieveMessageEventsWorkflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecieveMessageEventsWorkflow_BlockWorkflows_Id",
                        column: x => x.Id,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecieveSignalEventsWorkflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Delivered = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecieveSignalEventsWorkflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RecieveSignalEventsWorkflow_BlockWorkflows_Id",
                        column: x => x.Id,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SendMessageEventsWorkflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendMessageEventsWorkflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendMessageEventsWorkflow_BlockWorkflows_Id",
                        column: x => x.Id,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SendSignalEventsWorkflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendSignalEventsWorkflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendSignalEventsWorkflow_BlockWorkflows_Id",
                        column: x => x.Id,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ServiceTasksWorkflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FailedResponse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceTasksWorkflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceTasksWorkflow_BlockWorkflows_Id",
                        column: x => x.Id,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ServiceTasksWorkflow_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StartEventsWorkflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StartEventsWorkflow", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StartEventsWorkflow_BlockWorkflows_Id",
                        column: x => x.Id,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "UserTasksWorkflow",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolveDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
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
                        principalColumn: "Id");
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
                        name: "FK_ConditionData_DataSchemas_DataSchemaId",
                        column: x => x.DataSchemaId,
                        principalTable: "DataSchemas",
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
                name: "SendMessageEventsModel",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RecieverId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendMessageEventsModel", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SendMessageEventsModel_BlockModels_Id",
                        column: x => x.Id,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SendMessageEventsModel_RecieveMessageEventsModel_RecieverId",
                        column: x => x.RecieverId,
                        principalTable: "RecieveMessageEventsModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ForeignRecieveEvents",
                columns: table => new
                {
                    SenderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ForeignBlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ForeignBlockName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignRecieveEvents", x => new { x.SenderId, x.SystemId, x.ForeignBlockId });
                    table.ForeignKey(
                        name: "FK_ForeignRecieveEvents_SendSignalEventsModel_SenderId",
                        column: x => x.SenderId,
                        principalTable: "SendSignalEventsModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ForeignRecieveEvents_Systems_SystemId",
                        column: x => x.SystemId,
                        principalTable: "Systems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlockModelDataSchemaEntity",
                columns: table => new
                {
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataSchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlockModelDataSchemaEntity", x => new { x.BlockId, x.DataSchemaId, x.ServiceTaskId });
                    table.ForeignKey(
                        name: "FK_BlockModelDataSchemaEntity_BlockModels_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BlockModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockModelDataSchemaEntity_DataSchemas_DataSchemaId",
                        column: x => x.DataSchemaId,
                        principalTable: "DataSchemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlockModelDataSchemaEntity_ServiceTasksModel_ServiceTaskId",
                        column: x => x.ServiceTaskId,
                        principalTable: "ServiceTasksModel",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DataSchemaMaps",
                columns: table => new
                {
                    ServiceTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SourceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataSchemaMaps", x => new { x.ServiceTaskId, x.SourceId, x.TargetId });
                    table.ForeignKey(
                        name: "FK_DataSchemaMaps_DataSchemas_SourceId",
                        column: x => x.SourceId,
                        principalTable: "DataSchemas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DataSchemaMaps_DataSchemas_TargetId",
                        column: x => x.TargetId,
                        principalTable: "DataSchemas",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DataSchemaMaps_ServiceTasksModel_ServiceTaskId",
                        column: x => x.ServiceTaskId,
                        principalTable: "ServiceTasksModel",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Specification = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Compulsory = table.Column<bool>(type: "bit", nullable: false),
                    Disabled = table.Column<bool>(type: "bit", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ConditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConditionExclusiveGatewayId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ConditionDataSchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attributes_BlockModels_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BlockModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Attributes_ConditionData_ConditionExclusiveGatewayId_ConditionDataSchemaId",
                        columns: x => new { x.ConditionExclusiveGatewayId, x.ConditionDataSchemaId },
                        principalTable: "ConditionData",
                        principalColumns: new[] { "ExclusiveGatewayId", "DataSchemaId" });
                });

            migrationBuilder.CreateTable(
                name: "AttributesMaps",
                columns: table => new
                {
                    BlockId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AttributesMaps", x => new { x.AttributeId, x.BlockId });
                    table.ForeignKey(
                        name: "FK_AttributesMaps_Attributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attributes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_AttributesMaps_BlockModels_BlockId",
                        column: x => x.BlockId,
                        principalTable: "BlockModels",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ForeignAttributeMaps",
                columns: table => new
                {
                    ForeignSendEventId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ForeignAttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ForeignAttributeMaps", x => new { x.AttributeId, x.ForeignSendEventId });
                    table.ForeignKey(
                        name: "FK_ForeignAttributeMaps_Attributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attributes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ForeignAttributeMaps_ForeignSendEvents_ForeignSendEventId",
                        column: x => x.ForeignSendEventId,
                        principalTable: "ForeignSendEvents",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskDatas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OutputTaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AttributeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SchemaId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskDatas_Attributes_AttributeId",
                        column: x => x.AttributeId,
                        principalTable: "Attributes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskDatas_BlockWorkflows_OutputTaskId",
                        column: x => x.OutputTaskId,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskDatas_DataSchemas_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "DataSchemas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ArrayTaskData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArrayTaskData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ArrayTaskData_TaskDatas_Id",
                        column: x => x.Id,
                        principalTable: "TaskDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BoolTaskData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<bool>(type: "bit", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoolTaskData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BoolTaskData_TaskDatas_Id",
                        column: x => x.Id,
                        principalTable: "TaskDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DateTaskData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DateTaskData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DateTaskData_TaskDatas_Id",
                        column: x => x.Id,
                        principalTable: "TaskDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "FileTaskData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MIMEType = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileTaskData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileTaskData_TaskDatas_Id",
                        column: x => x.Id,
                        principalTable: "TaskDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "NumberTaskData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NumberTaskData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NumberTaskData_TaskDatas_Id",
                        column: x => x.Id,
                        principalTable: "TaskDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SelectTaskData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectTaskData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectTaskData_TaskDatas_Id",
                        column: x => x.Id,
                        principalTable: "TaskDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StringTaskData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StringTaskData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StringTaskData_TaskDatas_Id",
                        column: x => x.Id,
                        principalTable: "TaskDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskDataMaps",
                columns: table => new
                {
                    TaskId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TaskDataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskDataMaps", x => new { x.TaskDataId, x.TaskId });
                    table.ForeignKey(
                        name: "FK_TaskDataMaps_BlockWorkflows_TaskId",
                        column: x => x.TaskId,
                        principalTable: "BlockWorkflows",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskDataMaps_TaskDatas_TaskDataId",
                        column: x => x.TaskDataId,
                        principalTable: "TaskDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TextTaskData",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextTaskData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TextTaskData_TaskDatas_Id",
                        column: x => x.Id,
                        principalTable: "TaskDatas",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Systems",
                columns: new[] { "Id", "Description", "Encryption", "ForeignEncryption", "Key", "Name", "State", "URL" },
                values: new object[] { new Guid("eef5b551-ea28-4439-9e6f-df4f45055e48"), null, 3, 3, new byte[] { 51, 255, 78, 181, 34, 125, 218, 30, 175, 231, 117, 17, 64, 175, 245, 163, 230, 97, 5, 161, 118, 34, 29, 135, 52, 187, 82, 147, 172, 241, 123, 255, 248, 59, 64, 11, 31, 29, 245, 61, 145, 141, 225, 140, 225, 181, 47, 117 }, "Tento systém", 5, "https://localhost:5001/" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Email", "Name", "Password", "PhoneNumber", "Surname", "Title", "UserName" },
                values: new object[] { new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4"), "admin.system@test.cz", "Admin", null, null, "System", "Ing.", "admin" });

            migrationBuilder.InsertData(
                table: "SystemRoles",
                columns: new[] { "Role", "UserId" },
                values: new object[] { 0, new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4") });

            migrationBuilder.InsertData(
                table: "SystemRoles",
                columns: new[] { "Role", "UserId" },
                values: new object[] { 1, new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4") });

            migrationBuilder.InsertData(
                table: "SystemRoles",
                columns: new[] { "Role", "UserId" },
                values: new object[] { 2, new Guid("5e250b64-ea22-4880-86d2-94d547b2e1b4") });

            migrationBuilder.CreateIndex(
                name: "IX_AgendaRoles_AgendaId",
                table: "AgendaRoles",
                column: "AgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_AgendaRoles_RoleId",
                table: "AgendaRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Agendas_AdministratorId",
                table: "Agendas",
                column: "AdministratorId");

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_BlockId",
                table: "Attributes",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_ConditionExclusiveGatewayId_ConditionDataSchemaId",
                table: "Attributes",
                columns: new[] { "ConditionExclusiveGatewayId", "ConditionDataSchemaId" });

            migrationBuilder.CreateIndex(
                name: "IX_AttributesMaps_BlockId",
                table: "AttributesMaps",
                column: "BlockId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditMessages_SystemId",
                table: "AuditMessages",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockModelDataSchemaEntity_DataSchemaId",
                table: "BlockModelDataSchemaEntity",
                column: "DataSchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockModelDataSchemaEntity_ServiceTaskId",
                table: "BlockModelDataSchemaEntity",
                column: "ServiceTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockModels_LaneId",
                table: "BlockModels",
                column: "LaneId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockModels_PoolId",
                table: "BlockModels",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockWorkflows_BlockModelId",
                table: "BlockWorkflows",
                column: "BlockModelId");

            migrationBuilder.CreateIndex(
                name: "IX_BlockWorkflows_WorkflowId",
                table: "BlockWorkflows",
                column: "WorkflowId");

            migrationBuilder.CreateIndex(
                name: "IX_ConditionData_DataSchemaId",
                table: "ConditionData",
                column: "DataSchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_ConnectionRequests_SystemId",
                table: "ConnectionRequests",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSchemaMaps_SourceId",
                table: "DataSchemaMaps",
                column: "SourceId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSchemaMaps_TargetId",
                table: "DataSchemaMaps",
                column: "TargetId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSchemas_ParentId",
                table: "DataSchemas",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_DataSchemas_ServiceId",
                table: "DataSchemas",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Flows_OutBlockId",
                table: "Flows",
                column: "OutBlockId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignAttributeMaps_AttributeId",
                table: "ForeignAttributeMaps",
                column: "AttributeId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ForeignAttributeMaps_ForeignSendEventId",
                table: "ForeignAttributeMaps",
                column: "ForeignSendEventId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignRecieveEvents_SystemId",
                table: "ForeignRecieveEvents",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_ForeignSendEvents_SystemId",
                table: "ForeignSendEvents",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_Headers_ServiceId",
                table: "Headers",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Lanes_PoolId",
                table: "Lanes",
                column: "PoolId");

            migrationBuilder.CreateIndex(
                name: "IX_Lanes_RoleId",
                table: "Lanes",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_Models_AgendaId",
                table: "Models",
                column: "AgendaId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Pools_ModelId",
                table: "Pools",
                column: "ModelId");

            migrationBuilder.CreateIndex(
                name: "IX_Pools_SystemId",
                table: "Pools",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_RecieveSignalEventsModel_ForeignSenderId",
                table: "RecieveSignalEventsModel",
                column: "ForeignSenderId",
                unique: true,
                filter: "[ForeignSenderId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_SendMessageEventsModel_RecieverId",
                table: "SendMessageEventsModel",
                column: "RecieverId",
                unique: true,
                filter: "[RecieverId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasksModel_ServiceId",
                table: "ServiceTasksModel",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ServiceTasksWorkflow_UserId",
                table: "ServiceTasksWorkflow",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SystemAgendas_SystemId",
                table: "SystemAgendas",
                column: "SystemId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDataMaps_TaskId",
                table: "TaskDataMaps",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDatas_AttributeId",
                table: "TaskDatas",
                column: "AttributeId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDatas_OutputTaskId",
                table: "TaskDatas",
                column: "OutputTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskDatas_SchemaId",
                table: "TaskDatas",
                column: "SchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_UserId",
                table: "UserRoles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasksWorkflow_UserId",
                table: "UserTasksWorkflow",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Workflows_AdministratorId",
                table: "Workflows",
                column: "AdministratorId");

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
                name: "ArrayTaskData");

            migrationBuilder.DropTable(
                name: "AttributesMaps");

            migrationBuilder.DropTable(
                name: "AuditMessages");

            migrationBuilder.DropTable(
                name: "BlockModelDataSchemaEntity");

            migrationBuilder.DropTable(
                name: "BoolTaskData");

            migrationBuilder.DropTable(
                name: "ConnectionRequests");

            migrationBuilder.DropTable(
                name: "DataSchemaMaps");

            migrationBuilder.DropTable(
                name: "DateTaskData");

            migrationBuilder.DropTable(
                name: "EndEventsModel");

            migrationBuilder.DropTable(
                name: "EndEventsWorkflow");

            migrationBuilder.DropTable(
                name: "FileTaskData");

            migrationBuilder.DropTable(
                name: "FilterEntity");

            migrationBuilder.DropTable(
                name: "Flows");

            migrationBuilder.DropTable(
                name: "ForeignAttributeMaps");

            migrationBuilder.DropTable(
                name: "ForeignRecieveEvents");

            migrationBuilder.DropTable(
                name: "Headers");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "NumberTaskData");

            migrationBuilder.DropTable(
                name: "ParallelGatewaysModel");

            migrationBuilder.DropTable(
                name: "RecieveMessageEventsWorkflow");

            migrationBuilder.DropTable(
                name: "RecieveSignalEventsModel");

            migrationBuilder.DropTable(
                name: "RecieveSignalEventsWorkflow");

            migrationBuilder.DropTable(
                name: "SelectTaskData");

            migrationBuilder.DropTable(
                name: "SendMessageEventsModel");

            migrationBuilder.DropTable(
                name: "SendMessageEventsWorkflow");

            migrationBuilder.DropTable(
                name: "SendSignalEventsWorkflow");

            migrationBuilder.DropTable(
                name: "ServiceTasksWorkflow");

            migrationBuilder.DropTable(
                name: "StartEventsModel");

            migrationBuilder.DropTable(
                name: "StartEventsWorkflow");

            migrationBuilder.DropTable(
                name: "StringTaskData");

            migrationBuilder.DropTable(
                name: "SystemAgendas");

            migrationBuilder.DropTable(
                name: "SystemRoles");

            migrationBuilder.DropTable(
                name: "TaskDataMaps");

            migrationBuilder.DropTable(
                name: "TextTaskData");

            migrationBuilder.DropTable(
                name: "UserRoles");

            migrationBuilder.DropTable(
                name: "UserTasksModel");

            migrationBuilder.DropTable(
                name: "UserTasksWorkflow");

            migrationBuilder.DropTable(
                name: "ServiceTasksModel");

            migrationBuilder.DropTable(
                name: "SendSignalEventsModel");

            migrationBuilder.DropTable(
                name: "ForeignSendEvents");

            migrationBuilder.DropTable(
                name: "RecieveMessageEventsModel");

            migrationBuilder.DropTable(
                name: "TaskDatas");

            migrationBuilder.DropTable(
                name: "AgendaRoles");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropTable(
                name: "BlockWorkflows");

            migrationBuilder.DropTable(
                name: "ConditionData");

            migrationBuilder.DropTable(
                name: "Workflows");

            migrationBuilder.DropTable(
                name: "DataSchemas");

            migrationBuilder.DropTable(
                name: "ExclusiveGatewaysModel");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "BlockModels");

            migrationBuilder.DropTable(
                name: "Lanes");

            migrationBuilder.DropTable(
                name: "Pools");

            migrationBuilder.DropTable(
                name: "SolvingRoles");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropTable(
                name: "Systems");

            migrationBuilder.DropTable(
                name: "Agendas");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
