using HealthWellbeing.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

// Add services to the container.
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<HealthWellbeingDbContext>(options =>
	options.UseSqlServer(builder.Configuration.GetConnectionString("HealthWellbeingConnection") ?? throw new InvalidOperationException("Connection string 'HealthWellbeingConnection' not found.")));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}
else
{
	using (var serviceScope = app.Services.CreateScope())
	{
		var dbContext = serviceScope.ServiceProvider.GetService<HealthWellbeingDbContext>();
		SeedDataGinasio.Populate(dbContext);
		SeedDataExercicio.Populate(dbContext);
		SeedDataTipoExercicio.Populate(dbContext);
		SeedDataProblemaSaude.Populate(dbContext);
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
