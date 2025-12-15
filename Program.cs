using HealthWellbeing.Data;
using HealthWellbeingRoom;
using HealthWellBeingRoom.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Contexto principal da aplicação
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("HealthWellbeingConnection")
        ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.")
    ));

// Contexto para Identity
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity (apenas UMA vez)
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

// PIPELINE / MIDDLEWARE
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// SEEDING (corre em ambos os ambientes, mas com Hsts/ExceptionHandler ajustado em cima)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dataContext = services.GetRequiredService<HealthWellbeingDbContext>();
    dataContext.Database.Migrate();

    // Seed do grupo 2 (o teu)
    SeedData.Populate(dataContext);

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    SeedData.SeedRoles(roleManager);

    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    SeedData.SeedDefaultAdmin(userManager);
    SeedData.SeedUser(userManager);

    // GROUP 1
    SeedDataGroup1.SeedRoles(roleManager);
    SeedDataGroup1.SeedDefaultAdmin(userManager);
    SeedDataGroup1.SeedUsers(userManager);
    SeedDataGroup1.Populate(dataContext);

    // GROUP 2 - Consumíveis / Zonas
    SeedDataGroup2.Populate(dataContext);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // <--- importante para Identity
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
