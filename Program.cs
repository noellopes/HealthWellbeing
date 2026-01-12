using HealthWellbeing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("HealthWellbeingConnection") ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.")));


var connectionString = builder.Configuration.GetConnectionString("HealthWellbeingConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options =>
    {
        // Sign in
        options.SignIn.RequireConfirmedAccount = false;

        // Password
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequiredLength = 8;
        options.Password.RequiredUniqueChars = 6;
        options.Password.RequireNonAlphanumeric = true;

        // Lockout
        options.Lockout.AllowedForNewUsers = true;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        options.Lockout.MaxFailedAccessAttempts = 5;
    })
    .AddEntityFrameworkStores<HealthWellbeingDbContext>()
    .AddDefaultUI();
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<HealthWellbeing.Services.IReceitaAjusteService, HealthWellbeing.Services.ReceitaAjusteService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    using (var scope = app.Services.CreateScope())
    {
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        SeedData.SeedRoles(roleManager);

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        SeedData.SeedDefaultAdmin(userManager);

        var context = scope.ServiceProvider.GetRequiredService<HealthWellbeingDbContext>();
        SeedData.Populate(context);
        SeedData.SeedPopulateClientsAsUsers(userManager, context);
        SeedData.SeedUsers(userManager, context);
        SeedData.SeedDefaultAdmin(userManager);
        SeedDataExercicio.Populate(context);
        SeedDataTipoExercicio.Populate(context);
        SeedDataProblemaSaude.Populate(context);
        
        // Seed ProgressRecord after users and clients are created
        SeedData.SeedProgressRecords(userManager, context);
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();