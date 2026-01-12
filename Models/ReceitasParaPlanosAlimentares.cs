namespace HealthWellbeing.Models
{
    public class ReceitasParaPlanosAlimentares
    {
        public int PlanoAlimentarId { get; set; }
        public PlanoAlimentar? PlanoAlimentar { get; set; }

        public int ReceitaId { get; set; }
        public Receita? Receita { get; set; }
    }
}
