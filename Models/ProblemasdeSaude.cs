namespace HealthWellbeing.Models
{
    public class ProblemasdeSaude
    {
        public int ProblemaId { get; set; }
        public string ProblemaCategoria { get; set; }
        public string ProblemaNome { get; set; }
        public string ZonaAtingida { get; set; }
        public string ProfissionaisDeApoio { get; set; }
        public int Gravidade { get; set; }
    }
}
