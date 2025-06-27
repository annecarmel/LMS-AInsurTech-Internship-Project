using ASSNlearningManagementSystem.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();

// Add session support
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Let DI inject IConfiguration into UserRepository automatically
builder.Services.AddScoped<UserRepository>();

// OPTIONAL: Register other repositories like EmployeeRepository
// builder.Services.AddScoped<EmployeeRepository>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
