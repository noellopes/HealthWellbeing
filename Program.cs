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

builder.Services.AddIdentity<IdentityUser, IdentityRole>(
    options => {
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
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddDefaultUI();

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
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    SeedDataGroup1.SeedRoles(roleManager);


    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
    SeedDataGroup1.SeedDefaultAdmin(userManager);

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    else
    {
        var AppContext = services.GetService<ApplicationDbContext>();
        var DataContext = services.GetRequiredService<HealthWellbeingDbContext>();

        app.UseMigrationsEndPoint();
        DataContext.Database.Migrate();
       
        SeedDataGroup1.SeedUsers(userManager);
        SeedDataGroup1.Populate(DataContext);

        SeedDataGroup2.Populate(DataContext);

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