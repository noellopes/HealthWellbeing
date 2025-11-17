using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Plano
    {
        [Required]
        public int PlanoId { get; set; }

        [Required(ErrorMessage = "O nome do plano é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome do plano não pode ter mais de 100 caracteres")]
        public string Nome { get; set; } = default!;

        [StringLength(500, ErrorMessage ="A Descrição não pode ter mais de 500 caracteres")]
        public string? Descricao { get; set; }

        [Required(ErrorMessage = "O preço do plano é obrigatório")]
        [Range(1.00, 999.99, ErrorMessage = "O preço deve ser entre 1,00 e 999,99")]
        public decimal Preco { get; set; }

        [Required(ErrorMessage = "A duração do treino é obrigatória")]
        [Range(1, 365, ErrorMessage = "A duração deve estar entre 1 e 365 dias")]
        public int DuracaoDias { get; set; }
    }
}