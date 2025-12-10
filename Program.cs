using HealthWellbeing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Contexto de Negócio (HealthWellbeing - Exercícios, Profissionais, etc.)
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HealthWellbeingConnection")
    ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.")));

// 2. Contexto de Segurança (Identity - Users, Logins)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. Configuração do Identity (IMPORTANTE: Adicionado .AddRoles)
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = true;
    // Podes ajustar regras de password aqui se quiseres
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
})
    .AddRoles<IdentityRole>() // <--- LINHA OBRIGATÓRIA PARA AS ROLES FUNCIONAREM
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
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// --- BLOCO DE SEEDING ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    try
    {
        // 1. Migrar a BD de Users (garante que AspNetUsers existe)
        var contextUsers = services.GetRequiredService<ApplicationDbContext>();
        contextUsers.Database.Migrate();

        // 2. Migrar a BD de Negócio
        var healthContext = services.GetRequiredService<HealthWellbeingDbContext>();
        healthContext.Database.Migrate();

        // 3. Popular Dados Estáticos (Exercícios, Músculos, etc.)
        // Isto vai chamar os teus métodos PopulateGeneros, PopulateExercicios, etc.
        SeedData.Populate(healthContext);

        // 4. Popular Dados de Utilizadores e Roles
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

        // A. Criar Roles (Admin, Profissional, Utente)
        await SeedData.SeedRoles(roleManager);

        // B. Criar Admin Padrão
        await SeedData.SeedDefaultAdmin(userManager);

        // C. Criar Profissional de Saúde (Login na BD Users + Perfil na BD HealthWellbeing)
        await SeedData.SeedProfissional(userManager, healthContext);
        SeedData.Populate(healthContext);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ocorreu um erro ao fazer o Seeding da BD.");
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