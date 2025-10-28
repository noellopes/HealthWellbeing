namespace HealthWellbeing.Models
{
    public class Disponibilidade
    {

        public int IdDisponibilidade { get; set; } = default!;
        public int DiaSemana { get; set; } = default!;
        public DateTime Data { get; set; }
        public int Capacidade { get; set; } = default!;
        public int Ativo { get; set; } = default!;

    }
}
