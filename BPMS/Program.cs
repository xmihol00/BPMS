using BPMS_BL.Facades;
using BPMS_BL.Profiles;
using BPMS_Common;
using BPMS_DAL;
using BPMS_DAL.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

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
services.AddScoped<ServiceDataSchemaRepository>();
services.AddScoped<BlockAttributeRepository>();
services.AddScoped<BlockAttributeMapRepository>();
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

services.AddAutoMapper(typeof(AgendaProfile), typeof(ServiceProfile), typeof(BlockAttributeProfile), typeof(CommunicationProfile),
                       typeof(BlockWorkflowProfile), typeof(UserProfile), typeof(SystemProfile));

services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
        .AddCookie(options =>
        {
            options.LoginPath = "/Account/SignIn";
        });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Task}/{action=Overview}/{id?}");

using BpmsDbContext context = app.Services.CreateScope().ServiceProvider.GetService<BpmsDbContext>();
context?.Database.Migrate();

app.Run();
