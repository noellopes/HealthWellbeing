using HealthWellbeing.Data;
using HealthWellbeingRoom;
using HealthWellBeingRoom.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using static System.Formats.Asn1.AsnWriter;

var builder = WebApplication.CreateBuilder(args);

// Contexto principal da aplicação
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HealthWellbeingConnection")
        ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.")));

// Contexto para Identity
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Configuração do Identity com roles
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
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
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

builder.Services.AddControllersWithViews();

// GROUP 1 - Filtering Service (Sort/Search)
builder.Services.AddScoped<IRecordFilterService<Pathology>, PathologyFilterService>();
builder.Services.AddScoped<IRecordFilterService<TreatmentType>, TreatmentTypeFilterService>();
builder.Services.AddScoped<IRecordFilterService<TreatmentRecord>, TreatmentRecordFilterService>();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    using (var serviceScope = app.Services.CreateScope())
    {
        // Obter o DbContext e aplicar migrações
        var dbcontext = serviceScope.ServiceProvider.GetRequiredService<HealthWellbeingDbContext>();
        SeedData.Populate(dbcontext);

        var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        SeedData.SeedRoles(roleManager);

        var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        SeedData.SeedDefaultAdmin(userManager);

        SeedData.SeedRoles(roleManager);
        SeedData.SeedDefaultAdmin(userManager);
        SeedData.SeedUser(userManager);
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();

// Helper para criar utilizadores com roles
static async Task EnsureUserIsCreatedAsync(UserManager<IdentityUser> userManager, string email, string password, string[] roles)
{
    var user = await userManager.FindByEmailAsync(email);
    if (user == null)
    {
        user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
        var result = await userManager.CreateAsync(user, password);
        if (result.Succeeded)
        {
            foreach (var role in roles)
            {
                await userManager.AddToRoleAsync(user, role);
            }
        }
    }
}