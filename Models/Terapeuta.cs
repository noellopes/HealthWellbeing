using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Terapeuta
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
        [StringLength(20, ErrorMessage = "O número de telefone tem de ter menos de 20 dígitos.")]
        public string Telefone { get; set; } = string.Empty;

        // Email — obrigatório
        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "O formato do email não é válido.")]
        [StringLength(100, ErrorMessage = "O email não pode ter mais de 100 caracteres.")]
        public string Email { get; set; } = string.Empty;



        // Ano de entrada no serviço — obrigatório
        [Required(ErrorMessage = "O ano de entrada é obrigatório.")]
        [Range(1900, 2100, ErrorMessage = "O ano de entrada deve ser válido.")]
        public int AnoEntrada { get; set; }

        // Calculo dos anos de experiência
        public int AnosExperiencia
        {
            get
            {
                int anoAtual = DateTime.Now.Year;
                return anoAtual - AnoEntrada;
            }
        }

        // Ativo
        public bool Ativo { get; set; }

        // Relação: um terapeuta pode ter vários agendamentos
        public ICollection<AgendamentoModel> Agendamentos { get; set; } = new List<AgendamentoModel>();
    }
}
