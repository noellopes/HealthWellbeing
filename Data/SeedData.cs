using Microsoft.AspNetCore.Identity;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        // 1. Cria os Roles (Perfis)
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            await EnsureRoleIsCreatedAsync(roleManager, "Administrator");
            await EnsureRoleIsCreatedAsync(roleManager, "ProblemasSaude");
            await EnsureRoleIsCreatedAsync(roleManager, "TipoExercicio");
            await EnsureRoleIsCreatedAsync(roleManager, "Exercicio");
            await EnsureRoleIsCreatedAsync(roleManager, "GrupoMuscular");
            await EnsureRoleIsCreatedAsync(roleManager, "Utente");
        }

        // 2. Cria o Admin Principal
        public static async Task SeedDefaultAdminAsync(UserManager<IdentityUser> userManager)
        {
            await EnsureUserIsCreatedAsync(userManager, "admin@ipg.pt", "Secret123$", new string[] { "Administrator" });
        }

        // 3. Cria outros Users
        public static async Task SeedUsersAsync(UserManager<IdentityUser> userManager)
        {
            await EnsureUserIsCreatedAsync(userManager, "miguelcosta@ipg.pt", "Secret123$", new string[] { "ProblemasSaude" });
            await EnsureUserIsCreatedAsync(userManager, "andrekhandoha@ipg.pt", "Secret123$", new string[] { "GrupoMuscular" });
            await EnsureUserIsCreatedAsync(userManager, "rodrigomartinho@ipg.pt", "Secret123$", new string[] { "TipoExercicio" });
            await EnsureUserIsCreatedAsync(userManager, "hugopereira@ipg.pt", "Secret123$", new string[] { "Exercicio" });
            await EnsureUserIsCreatedAsync(userManager, "maria@ipg.pt", "Secret123$", new string[] { "Utente" });
            await EnsureUserIsCreatedAsync(userManager, "paulo@ipg.pt", "Secret123$", new string[] { "Utente" });
        }

        // --- Métodos Auxiliares ---

        private static async Task EnsureRoleIsCreatedAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private static async Task EnsureUserIsCreatedAsync(UserManager<IdentityUser> userManager, string username, string password, string[] roles)
        {
            var user = await userManager.FindByNameAsync(username);

            if (user == null)
            {
                user = new IdentityUser(username);

                // A CORREÇÃO CRUCIAL ESTÁ AQUI:
                user.Email = username;       // O Email tem de ser igual ao username
                user.EmailConfirmed = true;  // O Email conta como confirmado

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new Exception($"Erro ao criar user {username}: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }

            foreach (var role in roles)
            {
                if (!await userManager.IsInRoleAsync(user, role))
                {
                    await userManager.AddToRoleAsync(user, role);
                }
            }
        }
    }
}