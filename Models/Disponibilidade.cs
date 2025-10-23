namespace HealthWellbeing.Models
{
    public class Disponibilidade
    {

        public int idDisponibilidade { get; set; } = default!;
        public int diaSemana { get; set; } = default!;
        public DateOnly data { get; set; }
        public int capacidade { get; set; } = default!;
        public int ativo { get; set; } = default!;

    }
}
