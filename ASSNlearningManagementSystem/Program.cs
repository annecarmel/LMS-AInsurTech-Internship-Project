using ASSNlearningManagementSystem.DataAccess;

var builder = WebApplication.CreateBuilder(args);

// Add MVC
builder.Services.AddControllersWithViews();

// Add session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Fetch connection string from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Register repositories with the DI container
builder.Services.AddScoped<UserRepository>(provider => new UserRepository(connectionString));
builder.Services.AddScoped<DashboardRepository>(provider => new DashboardRepository(builder.Configuration));

var app = builder.Build();

// Enable static files, routing, session
app.UseStaticFiles();
app.UseRouting();
app.UseSession();

// Default route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
