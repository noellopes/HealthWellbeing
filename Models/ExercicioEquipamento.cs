namespace HealthWellbeing.Models
{
    public class ExercicioEquipamento
    {
        // Chave Estrangeira para Exercicio
        public int ExercicioId { get; set; }
        public Exercicio? Exercicio { get; set; }

        // Chave Estrangeira para Genero
        public int EquipamentoId { get; set; }
        public Equipamento? Equipamento { get; set; }
    }
}
