using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {
    public enum RequirementType {
        [Display(Name = "Participar em Qualquer Evento")]
        ParticipateAnyEvent = 1,

        [Display(Name = "Participar em Tipo de Evento Específico")]
        ParticipateSpecificEventType = 2,

        [Display(Name = "Completar Atividade (Geral)")]
        CompleteAnyActivity = 3,

        [Display(Name = "Completar Tipo de Atividade Específica")]
        CompleteSpecificActivityType = 4
    }
}
