
using BPMS_DAL;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddControllersWithViews()
        .AddRazorRuntimeCompilation();

services.AddDbContext<BpmsDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DB")));

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
