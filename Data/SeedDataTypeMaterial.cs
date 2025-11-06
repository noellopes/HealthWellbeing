using System;
using System.Linq;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public class SeedDataTypeMaterial
    {
        internal static void Populate(HealthWellbeingDbContext? dbContext)
        {
            if (dbContext == null)
                throw new ArgumentNullException(nameof(dbContext));

           
            dbContext.Database.EnsureCreated();

           
            var typeMaterials = new[]
            {
                new TypeMaterial
                {
                    Name = "Consumível",
                    Description = "Materiais de uso único ou limitada duração, como gazes, seringas e luvas."
                },
                new TypeMaterial
                {
                    Name = "Equipamento",
                    Description = "Aparelhos fixos utilizados nas salas de consulta e tratamento."
                },
                new TypeMaterial
                {
                    Name = "Dispositivo Médico",
                    Description = "Instrumentos e aparelhos móveis usados para apoio ao diagnóstico e tratamento."
                },
                new TypeMaterial
                {
                    Name = "Instrumento Cirúrgico",
                    Description = "Ferramentas utilizadas em procedimentos invasivos e pequenas cirurgias."
                },
                new TypeMaterial
                {
                    Name = "Material de Esterilização",
                    Description = "Produtos e utensílios destinados à limpeza e esterilização de instrumentos."
                }
            };

            
            foreach (var tm in typeMaterials)
            {
                if (!dbContext.TypeMaterial.Any(x => x.Name == tm.Name))
                {
                    dbContext.TypeMaterial.Add(tm);
                }
            }

            dbContext.SaveChanges();
        }
    }
}
