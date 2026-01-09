namespace HealthWellbeing.Models
{
    public class SpecialitiesDoctor
    {
        public int IdEspecialidade { get; set; }
        public Specialities? Especialidade { get; set; }

        public int IdMedico { get; set; }
        public Doctor? Medico { get; set; }
    }
}
