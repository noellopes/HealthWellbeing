using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TerapeutaModel
    {
        [Key]
        public int TerapeutaId { get; set; }

        // Nome do terapeuta — obrigatório
        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome não pode ter mais de 100 caracteres.")]
        public string Nome { get; set; } = string.Empty;

        // Especialidade — obrigatório
        [Required(ErrorMessage = "A especialidade é obrigatória.")]
        [StringLength(40, ErrorMessage = "A especialidade não pode ter mais de 40 caracteres.")]
        public string Especialidade { get; set; } = string.Empty;

        // Telefone — obrigatório
        [Required(ErrorMessage = "O telefone é obrigatório.")]
        [Phone(ErrorMessage = "O número de telefone não é válido.")]
        [StringLength(9, ErrorMessage = "O número de telefone tem de ter 9 caracteres.")]
        public string Telefone { get; set; } = string.Empty;

        // Email — obrigatório
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do email não é válido.")]
        [StringLength(100, ErrorMessage = "O email não pode ter mais de 100 caracteres.")]
        public string Email { get; set; } = string.Empty;

        // Anos de experiência — obrigatório
        [Required(ErrorMessage = "Os anos de experiência são obrigatórios.")]
        [Range(0, 60, ErrorMessage = "Os anos de experiência devem estar entre 0 e 60.")]
        public int AnosExperiencia { get; set; }

        // Ativo
        public bool Ativo { get; set; }

        // Relação: um terapeuta pode ter vários agendamentos
        public ICollection<AgendamentoModel> Agendamentos { get; set; } = new List<AgendamentoModel>();
    }
}

