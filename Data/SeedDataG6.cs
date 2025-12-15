using HealthWellbeing.Models;
using Microsoft.AspNetCore.Identity;
using HealthWellbeing.Data;

namespace HealthWellbeing.Data
{
    public class SeedDataG6
    {
        // Método principal chamado no Program.cs
        internal static void Populate(HealthWellbeingDbContext? dbContext, UserManager<IdentityUser>? userManager, RoleManager<IdentityRole>? roleManager)
        {
            if (dbContext == null || userManager == null || roleManager == null)
                throw new ArgumentNullException("Contextos nulos");

            dbContext.Database.EnsureCreated();

            // 1. Criar Roles (Admin, Gestor, Medico, Utente)
            SeedRoles(roleManager).Wait();

            // 2. Criar Admin Padrão (O genérico)
            SeedDefaultAdmin(userManager).Wait();

            // 3. === NOVO: Criar os Membros do Grupo ===
            SeedGroupMembers(userManager).Wait();
        }

        internal static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { "Admin", "Gestor", "Medico", "Utente", "Rececionista", "Tecnico" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        internal static async Task SeedDefaultAdmin(UserManager<IdentityUser> userManager)
        {
            await CriarUtilizadorSeNaoExistir(userManager, "admin@healthwellbeing.com", "Secret123$", "Admin");
        }

        internal static async Task SeedGroupMembers(UserManager<IdentityUser> userManager)
        {
            // Lista dos colegas do grupo
            string[] emailsGrupo = {
                "diogomassano@ipg.pt",
                "dinisgomes@ipg.pt",
                "rafaelrodrigues@ipg.pt",
                "joaquimgoncalves@ipg.pt"
            };

            foreach (var email in emailsGrupo)
            {
                // Cria cada um com a role "Admin" e a password "Secret123$"
                await CriarUtilizadorSeNaoExistir(userManager, email, "Secret123$", "Admin");
            }
        }

        // --- MÉTODO AUXILIAR PARA EVITAR REPETIÇÃO DE CÓDIGO ---
        private static async Task CriarUtilizadorSeNaoExistir(UserManager<IdentityUser> userManager, string email, string password, string role)
        {
            var user = await userManager.FindByNameAsync(email);

            if (user == null)
            {
                user = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true // Importante para permitir login
                };

                var result = await userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, role);
                    // Se quiser que eles sejam também Gestores, descomente a linha abaixo:
                    // await userManager.AddToRoleAsync(user, "Gestor"); 
                }
            }
        }
    }
}