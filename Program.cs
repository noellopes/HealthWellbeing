using HealthWellbeing.Data;
using HealthWellbeing.Models;
using HealthWellBeing.Models; // Adicionado para garantir compatibilidade de namespaces
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // ==========================================
        // 1. CONFIGURAÇÃO DOS SERVIÇOS (DI)
        // ==========================================

        // Configuração do HealthWellbeingDbContext (Dados de Negócio)
        var healthConnection = builder.Configuration.GetConnectionString("HealthWellBeingConnection")
            ?? throw new InvalidOperationException("Connection string 'HealthWellBeingConnection' not found.");

        builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
            options.UseSqlServer(healthConnection));

        // Configuração do ApplicationDbContext (Identity)
        var identityConnection = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(identityConnection));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Configuração do Identity
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

        // ==========================================
        // 2. PIPELINE DE PEDIDOS HTTP
        // ==========================================

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

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=Index}/{id?}");

        app.MapRazorPages();

        // ==========================================
        // 3. ÁREA DE INICIALIZAÇÃO DA BASE DE DADOS (SEEDING)
        // ==========================================

        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var contextIdentity = services.GetRequiredService<ApplicationDbContext>();
                var contextHealth = services.GetRequiredService<HealthWellbeingDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // 1. Migrar BDs (Cria as tabelas se não existirem)
                await contextIdentity.Database.MigrateAsync();
                await contextHealth.Database.MigrateAsync();

                // 2. Executar Seeding (Preenche com dados falsos)
                await SeedDataG6.Populate(contextHealth, userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Ocorreu um erro ao inicializar ou popular a base de dados.");
            }
        }

        app.Run();
    }
}