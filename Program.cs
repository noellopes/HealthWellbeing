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
    try
    {
        // 1. Criar Roles e Admin (Usa o SeedDataAccount.cs)
        // Nota: O método tem de ser aguardado (.Wait() ou await num contexto async, aqui usamos .Wait() para garantir)
        HealthWellbeing.Data.SeedData.SeedRolesAndAdminAsync(services).Wait();

        // 2. Criar Planos e Clientes (Usa o SeedDataGinasio.cs)
        var context = services.GetRequiredService<HealthWellbeingDbContext>();
        SeedDataGinasio.Populate(context);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "ERRO CRÍTICO: Falha ao criar dados iniciais (Seed).");
    }
}
// =========================================================

app.Run();