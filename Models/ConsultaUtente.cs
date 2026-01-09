namespace HealthWellbeing.Models
{
    public class ConsultaUtente
    {
        public int IdConsulta { get; set; }
        public Consulta? Consulta { get; set; }

        public int IdUtente { get; set; }
        public UtenteSaude? Utente { get; set; }
    }
}
