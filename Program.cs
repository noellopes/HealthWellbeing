using HealthWellbeing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

internal class Program
{
    private static async Task Main(string[] args) // Alterado para async Task para ser mais robusto
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Configuração da Base de Dados de Negócio (HealthWellbeing) com Proteção contra Falhas
        builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("HealthWellBeingConnection")
                ?? throw new InvalidOperationException("Connection string 'HealthWellBeingConnection' not found."),
                sqlServerOptionsAction: sqlOptions =>
                {
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                }));

        // 2. Configuração da Base de Dados de Login (Identity)
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddDatabaseDeveloperPageExceptionFilter();

        // 3. Configuração do Identity (CORRIGIDO: ADICIONADO .AddRoles)
        builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
            .AddRoles<IdentityRole>() // <--- ISTO FALTAVA E É OBRIGATÓRIO PARA TER ADMINS
            .AddEntityFrameworkStores<ApplicationDbContext>();

        builder.Services.AddControllersWithViews();

        var app = builder.Build();

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

        // ==============================================================================
        // BLOCO ÚNICO DE INICIALIZAÇÃO DA BASE DE DADOS
        // (Removemos o bloco repetido que estava a dar erro)
        // ==============================================================================
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<ApplicationDbContext>();
                var healthContext = services.GetRequiredService<HealthWellbeingDbContext>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // A. CONSTRUIR A CASA (CRIAR TABELAS)
                // Migrate para o Identity (Users)
                context.Database.Migrate();
                // EnsureCreated para o Negócio (Exames, Materiais)
                healthContext.Database.EnsureCreated();

                // B. MOBILAR A CASA (INSERIR DADOS)
                // Materiais e Profissionais
                SeedDataMaterialEquipamentoAssociado.Populate(healthContext);
                SeedDataProfissionalExecutante.Populate(healthContext);

                // C. CRIAR UTILIZADORES E ROLES
                // Usamos await para garantir que termina antes de arrancar
                HealthWellbeing.Data.SeedDataG6.Populate(healthContext, userManager, roleManager);
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "Ocorreu um erro ao inicializar a base de dados.");
            }
        }

        app.Run();
    }
}