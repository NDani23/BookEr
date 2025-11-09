using BookEr.Persistence;
using BookEr.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<BookerDbContext>(options =>
{
    IConfigurationRoot configuration = builder.Configuration;

    options.UseSqlServer(configuration.GetConnectionString("SqlServerConnection"));

    options.UseLazyLoadingProxies();
});

builder.Services.AddIdentity<ApplicationUser, IdentityRole<int>>(options =>
{

    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = false;
    options.Password.RequiredUniqueChars = 3;


    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 10;
    options.Lockout.AllowedForNewUsers = true;


    options.User.RequireUniqueEmail = true;
})
.AddEntityFrameworkStores<BookerDbContext>()
.AddRoles<IdentityRole<int>>()
.AddDefaultTokenProviders();

builder.Services.AddTransient<IBookerService, BookerService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var serviceScope = app.Services.CreateScope())
using (var context = serviceScope.ServiceProvider.GetRequiredService<BookerDbContext>())
using (var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>())
using (var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>())
{
    string imageSource = app.Configuration.GetValue<string>("ImageSource");
    await DbInitializer.Initialize(context, imageSource, userManager, roleManager);
}

app.Run();
