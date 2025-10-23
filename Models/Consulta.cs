namespace HealthWellbeing.Models
{
    public class Consulta
    {
        int idConsulta { get; set; }
        DateTime dataMarcacao { get; set; }
        DateTime dataConsulta { get; set; }
        DateTime dataCancelamento { get; set; }
        TimeOnly horaInicio { get; set; }
        TimeOnly horaFim { get; set; }
        
    }
}
