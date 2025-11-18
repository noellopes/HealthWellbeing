using HealthWellbeing.Data;
using HealthWellbeing.Models; // Confirma se os teus modelos estão aqui
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração da Base de Dados dos DADOS (HealthWellbeingDbContext)
var healthConnectionString = builder.Configuration.GetConnectionString("HealthWellbeingConnection")
    ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.");

builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(healthConnectionString));

// 2. Configuração da Base de Dados do IDENTITY (ApplicationDbContext)
var identityConnectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(identityConnectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. Configuração do Identity (Regras de Password e Bloqueio)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
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
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders() // Essencial para o registo funcionar
.AddDefaultUI();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// 4. O SEGREDO DO SEED (Isto popula a base de dados ao arrancar)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // A. Migrar e Popular IDENTITY (Users e Roles)
        var contextIdentity = services.GetRequiredService<ApplicationDbContext>();
        contextIdentity.Database.Migrate(); // Cria a BD se não existir

        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        // Chama a classe SeedData que vou meter abaixo
        await SeedData.SeedRolesAsync(roleManager);
        await SeedData.SeedDefaultAdminAsync(userManager);
        await SeedData.SeedUsersAsync(userManager);

        // B. Migrar e Popular DADOS DE SAÚDE
        var contextHealth = services.GetRequiredService<HealthWellbeingDbContext>();
        contextHealth.Database.Migrate(); // Cria a BD se não existir

        SeedDataProblemaSaude.Populate(contextHealth);

        // ATENÇÃO: Se tiveres estas classes, descomenta estas linhas:
        // SeedDataExercicio.Populate(contextHealth);
        // SeedDataTipoExercicio.Populate(contextHealth);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao popular a base de dados.");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
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