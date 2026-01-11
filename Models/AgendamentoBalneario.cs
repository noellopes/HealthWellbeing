using HealthWellbeing.Models;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    // Enum para o estado da reserva
    public enum EstadoAgendamento
    {
        [Display(Name = "Pendente")]
        Pendente = 0,
        [Display(Name = "Confirmado")]
        Confirmado = 1,
        [Display(Name = "Cancelado")]
        Cancelado = 2,
        [Display(Name = "Realizado")]
        Realizado = 3
    }

    public class AgendamentoBalneario
    {
        [Key]
        public int AgendamentoId { get; set; }

        //[Required(ErrorMessage = "O Utente é obrigatório.")]
        [Display(Name = "Utente")]
        public int? UtenteBalnearioId { get; set; }

        [ForeignKey(nameof(UtenteBalnearioId))]
        public UtenteBalneario? Utentes { get; set; }


        [Required(ErrorMessage = "O Terapeuta é obrigatório.")]
        [Display(Name = "Terapeuta")]
        public int? TerapeutaId { get; set; } 
        [ForeignKey("TerapeutaId")]
        public Terapeuta? Terapeuta { get; set; }


        [Required(ErrorMessage = "O Serviço é obrigatório.")]
        [Display(Name = "Serviço")]
        public int ServicoId { get; set; }
        [ForeignKey("ServicoId")]
        public Servico? Servico { get; set; }

        [Required(ErrorMessage = "O tipo de serviço é obrigatório.")]
        [DisplayName("Tipo de Serviço")]
        public int TipoServicosId { get; set; }
        [ForeignKey("TipoServicosId")]
        public TipoServicos? TipoServico { get; set; }

        [Required(ErrorMessage = "A data e hora de início são obrigatórias.")]
        [Display(Name = "Hora de Início")]
        [DataType(DataType.DateTime)]
        public DateTime HoraInicio { get; set; }

        [Required(ErrorMessage = "A duração em minutos é obrigatória.")]
        [DisplayName("Duração (minutos)")]
        [Range(1, 480, ErrorMessage = "A duração deve estar entre 1 e 480 minutos.")]
        public int DuracaoMinutos { get; set; }

        [NotMapped]
        public DateTime HoraFimCalculada
        {
            get => HoraInicio.AddMinutes(DuracaoMinutos);
        }

        [NotMapped] 
        [Display(Name = "Data da Reserva")]
        public DateTime DataReserva
        {
            get => HoraInicio.Date;
        }

        [Required(ErrorMessage = "O preço é obrigatório.")]
        [Column(TypeName = "decimal(18, 2)")]
        [DisplayName("Preço (€)")]
        [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "O preço deve ser superior a zero.")]
        public decimal Preco { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(500)]
        public string? Descricao { get; set; } = string.Empty;

        [Required(ErrorMessage = "O estado da reserva é obrigatório.")]
        [Display(Name = "Estado")]
        public EstadoAgendamento Estado { get; set; } = EstadoAgendamento.Pendente;
    }
}