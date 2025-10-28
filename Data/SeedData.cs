using HealthWellbeing.Data;
using HealthWellbeing.Models;

namespace HealthWellbeing.Data
{
    public static class SeedData
    {
        public static void Populate(HealthWellbeingDbContext? db)
        {
            if (db == null) throw new ArgumentNullException(nameof(db));

            // Garante que a BD existe
            db.Database.EnsureCreated();

            PopulateDoctor(db);
        }

        private static void PopulateDoctor(HealthWellbeingDbContext db)
        {

            var doctor = new[]
            {
                new Doctor { Nome = "Ana Martins",       Telemovel = "912345678", Email = "ana.martins@healthwellbeing.pt" },
                new Doctor { Nome = "Bruno Carvalho",    Telemovel = "913456789", Email = "bruno.carvalho@healthwellbeing.pt" },
                new Doctor { Nome = "Carla Ferreira",    Telemovel = "914567890", Email = "carla.ferreira@healthwellbeing.pt" },
                new Doctor { Nome = "Daniel Sousa",      Telemovel = "915678901", Email = "daniel.sousa@healthwellbeing.pt" },
                new Doctor { Nome = "Eduarda Almeida",   Telemovel = "916789012", Email = "eduarda.almeida@healthwellbeing.pt" },
                new Doctor { Nome = "Fábio Pereira",     Telemovel = "917890123", Email = "fabio.pereira@healthwellbeing.pt" },
                new Doctor { Nome = "Gabriela Rocha",    Telemovel = "918901234", Email = "gabriela.rocha@healthwellbeing.pt" },
                new Doctor { Nome = "Hugo Santos",       Telemovel = "919012345", Email = "hugo.santos@healthwellbeing.pt" },
                new Doctor { Nome = "Inês Correia",      Telemovel = "920123456", Email = "ines.correia@healthwellbeing.pt" },
                new Doctor { Nome = "João Ribeiro",      Telemovel = "921234567", Email = "joao.ribeiro@healthwellbeing.pt" },
                new Doctor { Nome = "Luísa Nogueira",    Telemovel = "922345678", Email = "luisa.nogueira@healthwellbeing.pt" },
                new Doctor { Nome = "Miguel Costa",      Telemovel = "923456789", Email = "miguel.costa@healthwellbeing.pt" },
                new Doctor { Nome = "Nádia Gonçalves",   Telemovel = "924567890", Email = "nadia.goncalves@healthwellbeing.pt" },
                new Doctor { Nome = "Óscar Figueiredo",  Telemovel = "925678901", Email = "oscar.figueiredo@healthwellbeing.pt" },
                new Doctor { Nome = "Patrícia Lopes",    Telemovel = "926789012", Email = "patricia.lopes@healthwellbeing.pt" },
            };

            db.Doctor.AddRange(doctor);
            db.SaveChanges();
        }
    }
}

