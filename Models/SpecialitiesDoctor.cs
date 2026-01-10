using System;

namespace HealthWellbeing.Models
{
    public class SpecialitiesDoctor
    {
        public int IdEspecialidade { get; set; }
        public Specialities? Speciality { get; set; }

        public int IdMedico { get; set; }
        public Doctor? Doctor { get; set; }
    }
}
