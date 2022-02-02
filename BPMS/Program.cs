
using BPMS_BL.Facades;
using BPMS_BL.Profiles;
using BPMS_DAL;
using BPMS_DAL.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllersWithViews()
        .AddRazorRuntimeCompilation();

services.AddDbContext<BpmsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DB")));

services.AddScoped<AgendaRepository>();
services.AddScoped<BlockModelRepository>();
services.AddScoped<ModelRepository>();
services.AddScoped<PoolRepository>();
services.AddScoped<UserRepository>();

services.AddScoped<AgendaFacade>();
services.AddScoped<ModelUploadFacade>();

services.AddAutoMapper(typeof(AgendaProfile));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using var scope = app.Services.CreateScope();
using var context = scope.ServiceProvider.GetService<BpmsDbContext>();
context?.Database.Migrate();

app.Run();
