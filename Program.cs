using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellbeing.Utils.Group1.Interfaces;
using HealthWellbeing.Utils.Group1.Repositories;
using HealthWellbeing.Utils.Group1.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HealthWellbeingConnection") ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.")));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
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

builder.Services.AddControllersWithViews();

// GROUP 1 - Services
builder.Services.AddScoped<IRecordFilterService<Pathology>, PathologyFilterService>();
builder.Services.AddScoped<IRecordFilterService<TreatmentType>, TreatmentTypeFilterService>();
builder.Services.AddScoped<ITreatmentRecordRepository, TreatmentRecordRepository>();
builder.Services.AddScoped<IRecordFilterService<TreatmentRecord>, TreatmentRecordFilterService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
        app.UseHsts();
    }
    else
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var dbContext = services.GetRequiredService<HealthWellbeingDbContext>();

        app.UseMigrationsEndPoint();
        dbContext.Database.Migrate();

        // Group 1 - SEED DATA
        SeedDataGroup1.SeedRoles(roleManager);
        SeedDataGroup1.SeedDefaultAdmin(userManager);
        SeedDataGroup1.SeedUsers(userManager);
        SeedDataGroup1.Populate(dbContext);
        SeedDataGroup1.Populate(dbContext);

        //SeedData.Populate(dbContext); - COM PROBLEMAS..
        SeedDataExercicio.Populate(dbContext);
        SeedDataTipoExercicio.Populate(dbContext);
        SeedDataProblemaSaude.Populate(dbContext);
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