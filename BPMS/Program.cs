using BPMS_BL.Hubs;
using BPMS_BL.Facades;
using BPMS_BL.Profiles;
using BPMS_DAL;
using BPMS_DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using BPMS_BL;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllersWithViews()
        .AddRazorRuntimeCompilation();

services.AddDbContext<BpmsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DB")));
StaticData.ThisSystemURL = builder.Configuration.GetValue<string>("SystemURL");
StaticData.FileStore = builder.Configuration.GetValue<string>("FileStore");

services.AddScoped<AgendaRepository>();
services.AddScoped<BlockModelRepository>();
services.AddScoped<ModelRepository>();
services.AddScoped<PoolRepository>();
services.AddScoped<UserRepository>();
services.AddScoped<BlockModelRepository>();
services.AddScoped<DataSchemaRepository>();
services.AddScoped<AttributeRepository>();
services.AddScoped<AttributeMapRepository>();
services.AddScoped<SystemRepository>();
services.AddScoped<ServiceRepository>();
services.AddScoped<ServiceHeaderRepository>();
services.AddScoped<SolvingRoleRepository>();
services.AddScoped<AgendaRoleRepository>();
services.AddScoped<UserRoleRepository>();
services.AddScoped<PoolRepository>();
services.AddScoped<FlowRepository>();
services.AddScoped<SystemAgendaRepository>();
services.AddScoped<BlockWorkflowRepository>();
services.AddScoped<TaskDataRepository>();
services.AddScoped<WorkflowRepository>();
services.AddScoped<BlockModelDataSchemaRepository>();
services.AddScoped<TaskDataMapRepository>();
services.AddScoped<SystemRoleRepository>();
services.AddScoped<ForeignRecieveEventRepository>();
services.AddScoped<ForeignSendEventRepository>();
services.AddScoped<ForeignAttributeMapRepository>();
services.AddScoped<FilterRepository>();
services.AddScoped<DataSchemaMapRepository>();
services.AddScoped<NotificationRepository>();
services.AddScoped<ConnectionRequestRepository>();
services.AddScoped<AuditMessageRepository>();
services.AddScoped<LaneRepository>();

services.AddScoped<BaseFacade>();
services.AddScoped<AgendaFacade>();
services.AddScoped<ModelUploadFacade>();
services.AddScoped<ModelFacade>();
services.AddScoped<BlockModelFacade>();
services.AddScoped<ServiceFacade>();
services.AddScoped<PoolFacade>();
services.AddScoped<CommunicationFacade>();
services.AddScoped<UserFacade>();
services.AddScoped<TaskFacade>();
services.AddScoped<WorkflowFacade>();
services.AddScoped<SystemFacade>();
services.AddScoped<BlockWorkflowFacade>();
services.AddScoped<NotificationFacade>();

services.AddSignalR();

services.AddAutoMapper(typeof(AgendaProfile), typeof(ServiceProfile), typeof(AttributeProfile), typeof(CommunicationProfile),
                       typeof(BlockWorkflowProfile), typeof(UserProfile), typeof(SystemProfile));

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/SignIn";
        });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStatusCodePagesWithRedirects("/Error/{0}");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(name: "default", pattern: "{controller=Task}/{action=Overview}/{id?}");
app.MapHub<NotificationHub>("/Notification");

StaticData.ServiceProvider = app.Services.CreateScope().ServiceProvider;
BpmsDbContext context = StaticData.ServiceProvider.GetService<BpmsDbContext>();
context?.Database.Migrate();
StaticData.Load(context);
context?.Dispose();

app.Run();
