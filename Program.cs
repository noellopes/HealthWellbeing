using HealthWellbeing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// 1. Configuração da Base de Dados Principal (Negócio: Clientes, Membros, Planos)
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HealthWellbeingConnection")
    ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.")));

// 2. Configuração da Base de Dados de Identidade (Login: Users, Roles)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// 3. Configuração do Identity (Login)
// IMPORTANTE: .AddRoles<IdentityRole>() é obrigatório para ter Admin e Trainer
builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    // Seeding de Dados de Teste (Planos, etc.) - O seu código existente
    using (var serviceScope = app.Services.CreateScope())
    {
        var dbContext = serviceScope.ServiceProvider.GetService<HealthWellbeingDbContext>();
        // Verifique se a classe SeedDataGinasio existe mesmo, senão comente esta linha
        // SeedDataGinasio.Populate(dbContext); 
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

// Este bloco corre sempre que a aplicação inicia para garantir que o Admin existe.
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        
        // Vai criar as Roles (Admin, Trainer, Client) e os utilizadores admin@ginasio.com e treinador@ginasio.com
        await SeedData.SeedRolesAndAdminAsync(services);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database (Admin/Roles).");
    }
}

app.Run();