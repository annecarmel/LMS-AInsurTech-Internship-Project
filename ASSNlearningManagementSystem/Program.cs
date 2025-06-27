using ASSNlearningManagementSystem.DataAccess;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// ✅ Get connection string
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// ✅ Register repositories with required dependencies
builder.Services.AddScoped<CourseRepository>(provider => new CourseRepository(connectionString));
builder.Services.AddScoped<UserRepository>(provider => new UserRepository(connectionString));
builder.Services.AddScoped<DashboardRepository>(provider => new DashboardRepository(builder.Configuration));

// ✅ Add MVC
builder.Services.AddControllersWithViews();

// ✅ Add Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login/Login"; // fallback redirect if not logged in
    });

// ✅ Add Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// ✅ Middleware pipeline
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication(); // Enable authentication
app.UseAuthorization();  // Enable authorization
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Login}/{action=Login}/{id?}");

app.Run();
