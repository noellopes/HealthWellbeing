using HealthWellbeing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DbContexts
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.")
    )
);

builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("HealthWellbeingConnection")
        ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.")
    )
);

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;

    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequiredUniqueChars = 6;
    options.Password.RequireNonAlphanumeric = true;

    options.Lockout.AllowedForNewUsers = true;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
    options.Lockout.MaxFailedAccessAttempts = 5;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultUI();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseMigrationsEndPoint();
}

// SEEDING (correto: fora do if/else da pipeline)
using (var scope = app.Services.CreateScope())
{
    var serviceProvider = scope.ServiceProvider;

    var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    SeedData.SeedRoles(roleManager);

    var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
    SeedData.SeedDefaultAdmin(userManager);

    if (app.Environment.IsDevelopment())
    {
        // se quiseres semear utilizadores extra só em dev
        SeedData.SeedUsers(userManager);

        var dbContext = serviceProvider.GetRequiredService<HealthWellbeingDbContext>();
        SeedData.Populate(dbContext);
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
    pattern: "{controller=UtenteSaude}/{action=Index}/{id?}"
);

app.MapRazorPages();

app.Run();
