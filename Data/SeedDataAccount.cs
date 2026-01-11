using Microsoft.AspNetCore.Identity;
using HealthWellbeing.Models;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // 1. Obter os serviços necessários
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var context = serviceProvider.GetRequiredService<HealthWellbeingDbContext>();

            // 2. Garantir que os Roles existem (Admin, Trainer, Client)
            string[] roleNames = { "Administrator", "Trainer", "Client" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // 3. Criar o SUPER ADMINISTRADOR (se não existir)
            string adminEmail = "admin@ginasio.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                // Criar a conta de Login (Identity)
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                // Senha forte obrigatória
                var result = await userManager.CreateAsync(adminUser, "Admin123!");

                if (result.Succeeded)
                {
                    // Atribuir Roles (Admin e Trainer para ter acesso a tudo)
                    await userManager.AddToRoleAsync(adminUser, "Administrator");
                    await userManager.AddToRoleAsync(adminUser, "Trainer");

                    // Criar o perfil de Trainer na base de dados de negócio
                    // (O Admin também precisa de estar na tabela Trainer para aparecer nas listas)
                    var trainerProfile = new Trainer
                    {
                        Name = "Administrador Principal",
                        Email = adminEmail,
                        Phone = "999999999",
                        // Ajuste os campos abaixo conforme o seu Model Trainer
                        // Address = "Gym Office", 
                        // BirthDate = DateTime.Now.AddYears(-30),
                        // Gender = "M" 
                    };

                    // Verifica se a tabela Trainer tem campos obrigatórios extra e adicione aqui se necessário
                    context.Trainer.Add(trainerProfile);
                    await context.SaveChangesAsync();
                }
            }

            // 4. Criar um TREINADOR normal para testes
            string trainerEmail = "treinador@ginasio.com";
            var trainerUser = await userManager.FindByEmailAsync(trainerEmail);

            if (trainerUser == null)
            {
                trainerUser = new IdentityUser { UserName = trainerEmail, Email = trainerEmail, EmailConfirmed = true };
                await userManager.CreateAsync(trainerUser, "Trainer123!");
                await userManager.AddToRoleAsync(trainerUser, "Trainer");

                context.Trainer.Add(new Trainer
                {
                    Name = "Treinador",
                    Email = trainerEmail,
                    Phone = "911111111"
                });
                await context.SaveChangesAsync();
            }
        }
    }
}