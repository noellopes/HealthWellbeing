using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {
    public enum RequirementType {
        [Display(Name = "Participar em Qualquer Evento")]
        ParticipateAnyEvent = 1,

        [Display(Name = "Participar em Evento Específico")]
        ParticipateSpecificEvent = 2,

        [Display(Name = "Completar Atividade (Geral)")]
        CompleteActivityGeneral = 3,

        [Display(Name = "Completar Atividade Específica")]
        CompleteActivitySpecific = 4
    }
}
