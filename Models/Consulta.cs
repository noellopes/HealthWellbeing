namespace HealthWellbeing.Models
{
    public class Consulta
    {
        public int idConsulta { get; set; }
        public DateTime dataMarcacao { get; set; }
        public DateTime dataConsulta { get; set; }
        public DateTime dataCancelamento { get; set; }
        public TimeOnly horaInicio { get; set; }
        public TimeOnly horaFim { get; set; }
        
        
    }
}
