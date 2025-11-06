using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class AgendamentoBalneario
    {
        [Key]
        public int AgendamentoId { get; set; }

       
        [Display(Name = "Cliente")]
        public int UtenteBalnearioId { get; set; }
        [ForeignKey("UtenteBalnearioId")]
        public UtenteBalneario UtenteBalneario { get; set; }
        [Display(Name = "Terapeuta")]
        public int TerapeutaId { get; set; }
        [ForeignKey("TerapeutaId")]
        public TerapeutaModel Terapeuta { get; set; }

        [Display(Name = "Serviço")]
        public int ServicoId { get; set; }
        [ForeignKey("ServicoId")]
        public Servico Servico { get; set; }

        [Required(ErrorMessage = "A data e hora de início são obrigatórias.")]
        [Display(Name = "Início da Reserva")]
        public DateTime DataHoraInicio { get; set; }

        [Display(Name = "Fim da Reserva (Estimado)")]
        public DateTime DataHoraFim { get; set; }

        [Required(ErrorMessage = "O estado da reserva é obrigatório.")]
        [StringLength(50)]
        public string Estado { get; set; } // Ex: "Pendente", "Confirmado", "Cancelado"



    }
}

