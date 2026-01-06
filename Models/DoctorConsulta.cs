namespace HealthWellbeing.Models
{
    public class DoctorConsulta{

        public int IdMedico {  get; set; }
        public Doctor? Medico { get; set; }

        public int IdConsulta { get; set; }
        public Consulta? Consulta { get; set; }

    }
}
