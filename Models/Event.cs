using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Precisamos disto

namespace HealthWellbeing.Models
{
    public class Event
    {
        public int EventId { get; set; }

        [Required(ErrorMessage = "O nome do evento é obrigatório.")]
        [StringLength(100, ErrorMessage = "O nome do evento não pode exceder 100 caracteres.")]
        public string EventName { get; set; } = string.Empty;

        [Required(ErrorMessage = "A descrição do evento é obrigatória.")]
        [StringLength(500, ErrorMessage = "A descrição do evento não pode exceder 500 caracteres.")]
        public string EventDescription { get; set; } = string.Empty;

        [Required(ErrorMessage = "O tipo de evento é obrigatório.")]
        [StringLength(50, ErrorMessage = "O tipo de evento não pode exceder 50 caracteres.")]
        public string EventType { get; set; } = string.Empty;

        [Required(ErrorMessage = "A data de início é obrigatória.")]
        [Display(Name = "Início do Evento")]
        [DataType(DataType.DateTime)]
        public DateTime EventStart { get; set; }

        [Required(ErrorMessage = "A data de fim é obrigatória.")]
        [Display(Name = "Fim do Evento")]
        [DataType(DataType.DateTime)]
        public DateTime EventEnd { get; set; }

        [Required(ErrorMessage = "Os pontos do evento são obrigatórios.")]
        [Display(Name = "Pontos")]
        [Range(0, 10000, ErrorMessage = "Os pontos devem estar entre 0 e 10000.")]
        public int EventPoints { get; set; }

        [Required(ErrorMessage = "O nível mínimo é obrigatório.")]
        [Display(Name = "Nível Mínimo")]
        [Range(1, 100, ErrorMessage = "O nível deve estar entre 1 e 100.")]
        public int MinLevel { get; set; }

        // --- MUDANÇA AQUI ---
        // 1. Removemos a propriedade "Status" que era guardada na BD.
        // public EventStatus Status { get; set; } = EventStatus.Agendado;

        // 2. Adicionamos uma propriedade "calculada" que não vai para a BD.
        [NotMapped] // Diz ao Entity Framework para ignorar esta propriedade
        [Display(Name = "Estado")]
        public EventStatus Status
        {
            get
            {
                var now = DateTime.Now;

                if (now > EventEnd)
                {
                    return EventStatus.Realizado; // Já acabou
                }

                if (now >= EventStart && now <= EventEnd)
                {
                    return EventStatus.Adecorrer; // Está a acontecer
                }

                // Se nenhuma das acima for verdade, ainda não começou
                return EventStatus.Agendado;
            }
        }
    }
}