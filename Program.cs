using HealthWellbeing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Base de Dados de Negócio (Ginásio)
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HealthWellbeingConnection")
    ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.")));

// 2. Base de Dados de Login (Identity)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. Configuração do Identity
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configuração do Pipeline
if (!app.Environment.IsDevelopment())
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

// =========================================================
// BLOCO DE SEEDING (CRIAÇÃO DE DADOS AUTOMÁTICA)
// =========================================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    // --- BLOCO 1: IDENTITY (LOGINS) ---
    try
    {
        var identityContext = services.GetRequiredService<ApplicationDbContext>();
        identityContext.Database.Migrate();
        // Verifica se a classe SeedData existe no teu projeto, caso contrário comenta a linha abaixo
        HealthWellbeing.Data.SeedData.SeedRolesAndAdminAsync(services).Wait();
    }
    catch (Exception ex)
    {
        Console.WriteLine("AVISO: Falha no Seed do Identity: " + ex.Message);
    }

    // --- BLOCO 2: GINÁSIO (DADOS DE NEGÓCIO) ---
    try
    {
        var context = services.GetRequiredService<HealthWellbeingDbContext>();
        context.Database.Migrate(); // Garante que as tabelas existem via Migrations
        SeedDataGinasio.Populate(context);
    }
    catch (Exception ex)
    {
        // CORREÇÃO 3: Captura a InnerException para mostrar o erro REAL do SQL (ex: FK constraint)
        var msg = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
        throw new Exception("ERRO NO SEED DO GINÁSIO: " + msg);
    }
}
// =========================================================

app.Run();