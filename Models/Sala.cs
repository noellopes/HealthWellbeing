using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Sala
    {
        
        public int SalaId { get; set; }
        [Required (ErrorMessage = "O nome da sala é obrigatorio.")]
        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        public string Nome { get; set; }
         public enum TipoSala { Consultas, Tratamentos }
        [Required(ErrorMessage = "A especialidade é obrigatoria.")]
        [StringLength(100, ErrorMessage = "A Especialidade não pode exceder 100 caracteres")]
        public string Especialidade { get; set; }
        [Range(1, 3, ErrorMessage = "A capacidade deve ser entre 1 e 3")]
        public int Capacidade { get; set; }
        [Required(ErrorMessage = "A localização é obrigatoria.")]
        public string Localizacao { get; set; }
        public List<String> Equipamentos { get; set; }
        public List<String> DispositivosMoveis { get; set; }
        public List<string> Agendamentos { get; set; }
        public bool Disponibilidade { get; set; }
        [Required(ErrorMessage = "O horário de funcionamento é obrigatorio.")]
        public string HorarioFuncionamento { get; set; }
        [StringLength(500, ErrorMessage = "As observações não podem exceder 500 caracteres")]
        public string Observacoes { get; set; }


    }
}
