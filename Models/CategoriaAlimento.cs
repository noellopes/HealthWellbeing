namespace HealthWellbeing.Models
{
    public class CategoriaAlimento
    {
        public int CategoriaAlimentoId { get; set; }

        public string Name { get; set; } = default!;

        public string? Description { get; set; }
    }
}
