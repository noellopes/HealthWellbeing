using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class UtentesSaude
    {
        [Key]
        public int idUtente { get; set; }
        // Nome completo
        [Required(ErrorMessage ="O nome do Utente é Obrigatorio")]
        [StringLength(100,ErrorMessage ="o nome deve ter no maximo 100 caracteres")]
        [Display(Name ="Nome do Utente")]
        public string Nome { get; set; } = default!;
        //Data de Nascimento
        [Required(ErrorMessage = "A DataDeNascimento é obrigatoria")]
        [Display(Name = "Data de Nascimento")]
        public DateTime DataDeNascimento { get; set; } = default!;  // Usando DateTime para manipulação de datas para poder marcar a data de nascimento
        // Identificação fiscal 
        public int NIF { get; set; } = default!;
        // Numero de segurança social
        public int NSS { get; set; } = default!;
        // Numero do Utente de Saude
        public int NUS { get; set; } = default!;   
        public int Idade { get; set; } = default!;              // ter em conta que pode ser calculada a partir da DataDeNascimento 
        // Email como string
        public string Email { get; set; } = default!;
        //Telefone
        public string Telefone { get; set; } = default!;      // String para preservar zeros iniciais e códigos
        // Endereço
        public string Morada { get; set; } = default!;          
        public string Descrição { get; set; } = default!; // Descrição para o Utente adicionar alguns dados extras
    }
}

