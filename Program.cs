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

// 3. Configuração do Identity (Roles são obrigatórios)
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

app.UseAuthentication(); // Obrigatório antes do Authorization
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
        HealthWellbeing.Data.SeedData.SeedRolesAndAdminAsync(services).Wait();
    }
    catch (Exception ex)
    {
        Console.WriteLine("AVISO: Falha no Seed do Identity: " + ex.Message);
        // Não fazemos throw aqui para ele tentar o próximo bloco
    }
    try
    {
        var context = services.GetRequiredService<HealthWellbeingDbContext>();
        context.Database.Migrate(); // Garante que as tuas tabelas existem
        SeedDataGinasio.Populate(context);
    }
    catch (Exception ex)
    {
        // Se este falhar, queremos saber porquê!
        throw new Exception("ERRO NO SEED DO GINÁSIO: " + ex.Message);
    }
}
// =========================================================

app.Run();