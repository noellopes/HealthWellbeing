using HealthWellbeing.Data;
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

        // Configuração do HealthWellbeingDbContext (Dados de Negócio: Exames, Utentes, etc.)
        var healthConnection = builder.Configuration.GetConnectionString("HealthWellBeingConnection")
            ?? throw new InvalidOperationException("Connection string 'HealthWellBeingConnection' not found.");

        builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
            options.UseSqlServer(healthConnection));

        // Configuração do ApplicationDbContext (Dados de Identidade: Users, Roles)
        var identityConnection = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(identityConnection));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // Configuração do Identity (Users e Roles)
        builder.Services.AddDefaultIdentity<IdentityUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            // Configurações extra de password (opcional)
            options.Password.RequireDigit = false;
            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequiredLength = 6;
        })
        .AddRoles<IdentityRole>() // IMPORTANTE: Permite o uso de Roles (Admin, Medico, etc.)
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
            // The default HSTS value is 30 days.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseAuthentication(); // Obrigatório antes do Authorization
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
                // Obter os serviços necessários
                var contextIdentity = services.GetRequiredService<ApplicationDbContext>();
                var contextHealth = services.GetRequiredService<HealthWellbeingDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // 1. MIGRAR AS BASES DE DADOS AUTOMATICAMENTE
                // Isto garante que as tabelas são criadas se não existirem
                await contextIdentity.Database.MigrateAsync();
                await contextHealth.Database.MigrateAsync();

                // 2. EXECUTAR O SEEDING
                // Chama a classe SeedDataG6 que criámos anteriormente
                await SeedDataG6.Populate(contextHealth, userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Ocorreu um erro ao inicializar ou popular a base de dados.");
            }
        }

        // ==========================================
        // 4. INICIAR A APLICAÇÃO
        // ==========================================
        app.Run();
    }
}