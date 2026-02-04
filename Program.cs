using HealthWellbeing.Data;
using HealthWellbeing.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =========================
// DATABASE
// =========================
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("HealthWellbeingConnection")));

// =========================
// IDENTITY 
// =========================
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>();

// =========================
// MVC + RAZOR
// =========================
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// =========================
// SERVICES
// =========================
builder.Services.AddScoped<ClienteService>();
builder.Services.AddScoped<VoucherService>();

var app = builder.Build();

// =========================
// PIPELINE
// =========================
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// 🔴 ESTES DOIS TÊM DE ESTAR OS DOIS
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
