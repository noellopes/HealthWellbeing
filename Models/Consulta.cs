namespace HealthWellbeing.Models
{
    public class Consulta
    {
        int IdConsulta { get; set; }
        DateTime DataMarcacao { get; set; }
        DateTime DataConsulta { get; set; }
        DateTime DataCancelamento { get; set; }
        TimeOnly HoraInicio { get; set; }
        TimeOnly HoraFim { get; set; }
        
    }
}
