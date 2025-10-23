using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ProfissionalExecutante
    {
        [Key]
        public int ProfissionalExecutanteId { get; set; } // Chave primária

        [Required(ErrorMessage = "O nome do profissional é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do profissional não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "A função/cargo é obrigatória.")]
        [StringLength(100, ErrorMessage = "A função/cargo não pode ter mais de 100 caracteres.")]
        public string Funcao { get; set; } // Futuramente poderá ser uma entidade (e.g. tabela Função)

        [Phone(ErrorMessage = "O número de telefone é inválido.")]
        [StringLength(20, ErrorMessage = "O número de telefone não pode ter mais de 20 caracteres.")]
        public string Telefone { get; set; }

        [EmailAddress(ErrorMessage = "O email é inválido.")]
        [StringLength(100, ErrorMessage = "O email não pode ter mais de 100 caracteres.")]
        public string Email { get; set; }
    }
}
