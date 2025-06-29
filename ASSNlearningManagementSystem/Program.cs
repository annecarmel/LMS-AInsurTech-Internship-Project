using ASSNlearningManagementSystem.DataAccess;
using ASSNlearningManagementSystem.Repository;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ✅ Get connection string from appsettings.json
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ✅ Register custom repositories
builder.Services.AddScoped<CourseRepository>(provider => new CourseRepository(connectionString));
builder.Services.AddScoped<UserRepository>(provider => new UserRepository(connectionString));
builder.Services.AddScoped<DashboardRepository>(provider => new DashboardRepository(builder.Configuration));
builder.Services.AddScoped<SessionRepository>(provider => new SessionRepository(connectionString)); // ✅ Added this

// ✅ Add MVC support
builder.Services.AddControllersWithViews();

// ✅ Configure Cookie-based Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;
    });

// ✅ Configure Session State
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// ✅ Build the app
var app = builder.Build();

// ✅ Middleware pipeline
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

// ✅ Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
