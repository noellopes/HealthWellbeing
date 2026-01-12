using HealthWellbeing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// --- CONFIGURAÇÃO DE BASES DE DADOS ---
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("HealthWellbeingConnection")));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// --- CONFIGURAÇÃO DE IDENTIDADE ---
builder.Services.AddDefaultIdentity<IdentityUser>(options => {
    options.SignIn.RequireConfirmedAccount = true; 
    options.Password.RequireDigit = true;          
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>();

// --- AUTENTICAÇÃO E AUTORIZAÇÃO GLOBAL ---
// Esta configuração garante que o site bloqueia o acesso anónimo mal é executado
/*builder.Services.AddControllersWithViews(options =>
{
    var policy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build();
    options.Filters.Add(new AuthorizeFilter(policy));
});*/
builder.Services.AddControllersWithViews();

var app = builder.Build();

// --- INICIALIZAÇÃO DE DADOS (SEED DATA) ---
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<HealthWellbeingDbContext>();

    // Executa o preenchimento automático de dados em ambiente de desenvolvimento
    if (app.Environment.IsDevelopment())
    {
        SeedDataExercicio.Populate(dbContext);
        SeedDataTipoExercicio.Populate(dbContext);
        SeedDataProblemaSaude.Populate(dbContext);
    }
}

// --- PIPELINE DE PEDIDOS (MIDDLEWARE) ---
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// A ordem destes dois é vital para o redirecionamento para o Log in
app.UseAuthentication();
app.UseAuthorization();

// --- MAPEAMENTO DE ROTAS ---
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages(); // Essencial para as páginas do Identity

app.Run();