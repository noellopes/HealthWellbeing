using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TipoTreino
    {
        public int TipoTreinoId { get; set; }

        [Required(ErrorMessage = "O nome do treino é obrigatório")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres")]
        public string Nome { get; set; } = default!;

        [StringLength(500, ErrorMessage = "A descrição não pode ter mais de 500 caracteres")]
        public string? Descricao { get; set; }

        [Range(10,300,ErrorMessage = "A duração deve estar entre 10 e 300 minutos")]
        public int DuracaoMinutos { get; set; } = 60;

        [Required(ErrorMessage = "A intensidade é obrigatória")]
        [StringLength(20)]
        public string Intensidade { get; set; } = "Moderada";

        public bool Ativo { get; set; } = true;
    }
}
