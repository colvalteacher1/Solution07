using Microsoft.EntityFrameworkCore;
using MyApp.Data;
using MyApp.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//Add this service for Blazor
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddServerSideBlazor()
    .AddCircuitOptions(options =>
    {
        options.DetailedErrors = true;
    });

builder.Services.AddDbContext<SystemDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("OnlineMySqlDB"),
        ServerVersion.AutoDetect(
            builder.Configuration.GetConnectionString("OnlineMySqlDB")!
        )));

// Services métier
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

//Add this middleware for Blazor
app.MapRazorComponents<MyApp.UI.App>()
    .AddInteractiveServerRenderMode();

app.Run();
