namespace HealthWellbeing.Models
{
    public class DoctorsBySpecialityViewModel
    {
        public string NomeEspecialidade { get; set; } = string.Empty;
        public IEnumerable<Doctor> Medicos { get; set; } = Enumerable.Empty<Doctor>();
    }
}