using HealthWellbeing.Models;
using HealthWellBeing.Models; // Usamos este namespace para as referências de navegação (ExameTipo, Utente, Medico, etc.)
using System.ComponentModel.DataAnnotations;

namespace HealthWellBeing.Models
{
    // Enum para o estado do exame, útil para o CRUD e agendamento
    public enum EstadoExame { Marcado, Realizado, Cancelado, PendenteResultado }

    public class Exame
    {
        
        public int ExameId { get; set; }

        
        [Required(ErrorMessage = "A data e hora da marcação são obrigatórias.")]
        public DateTime DataHoraMarcacao { get; set; }

       
        public EstadoExame Estado { get; set; } = EstadoExame.Marcado;

        
        [StringLength(500)]
        public string? Notas { get; set; }

        // ------------------------------------------------------------------
        // --- CHAVES ESTRANGEIRAS (FKs) E PROPRIEDADES DE NAVEGAÇÃO (1-N) ---
        // ------------------------------------------------------------------

        // 1. Utente (Instância marcada para um Utente)
        [Required(ErrorMessage = "O Utente é obrigatório.")]
        public int UtenteId { get; set; }
        public Utente? Utente { get; set; } // Propriedade de Navegação

        // 2. Tipo de Exame (Tipo de exame)
        [Required(ErrorMessage = "O Tipo de Exame é obrigatório.")]
        public int ExameTipoId { get; set; }
        public ExameTipo? ExameTipo { get; set; } // Propriedade de Navegação

        // 3. Médico Solicitante (Médico solicitante)
        [Required(ErrorMessage = "O Médico solicitante é obrigatório.")]
        public int MedicoId { get; set; }
        // Nota: Assumindo que o seu modelo de Médico está corrigido e se chama 'Medico'
        public Medicos? MedicoSolicitante { get; set; } // Propriedade de Navegação

        

        // 4. Sala de Exames (Necessário para o agendamento)
        public int? SalaId { get; set; } // Opcional se for agendado depois
        public Sala? SalaDeExames { get; set; }

        // 5. Profissional Executante (Técnico)
        public int? ProfissionalExecutanteId { get; set; } // Opcional, pode ser atribuído mais tarde
        public ProfissionalExecutante? ProfissionalExecutante { get; set; }

        // 6. Equipamento (Se houver uma relação 1-para-1 ou 1-para-N)
        public int? MaterialEquipamentoAssociadoId { get; set; }
        public MaterialEquipamentoAssociado? MaterialEquipamentoAssociado { get; set; }

        // Propriedade de Navegação para Resultados (1 Exame tem 0 ou N Resultados)
        //public ICollection<ResultadoExame>? Resultados { get; set; }
    }
}