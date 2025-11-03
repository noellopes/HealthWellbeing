using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
// ... outras usings ...

namespace HealthWellBeing.Models
{
    // Enum atualizado para refletir as opções pretendidas
    public enum EstadoExame
    {
        Pendente,
        Realizado,
        ARealizar, // Usamos 'ARealizar' para 'a realizar'
        Cancelado
    }

    public class Exame
    {
        // Chave Primária
        public int ExameId { get; set; }

        // Data e Hora de Marcação (propriedade do Controller/DbContext)
        [Required(ErrorMessage = "A data e hora da marcação são obrigatórias.")]
        public DateTime DataHoraMarcacao { get; set; }

        // Estado do Exame (propriedade do Controller)
        public EstadoExame Estado { get; set; } = EstadoExame.Pendente; // Valor por defeito

        // Notas ou Observações
        [StringLength(500)]
        public string? Notas { get; set; }

        //FOREIGN KEYS//

        // 1. Utente (Paciente)
        [Required(ErrorMessage = "O Utente é obrigatório.")]
        public int UtenteId { get; set; }
        public Utente? Utente { get; set; }

        // 2. Tipo de Exame (ExameTipo)
        [Required(ErrorMessage = "O Tipo de Exame é obrigatório.")]
        public int ExameTipoId { get; set; }
        public ExameTipo? ExameTipo { get; set; }

        // 3. Médico Solicitante
        [Required(ErrorMessage = "O Médico solicitante é obrigatório.")]
        public int MedicoId { get; set; }
        public Medicos? MedicoSolicitante { get; set; }

        // 4. Sala de Exames

        public int? SalaDeExameId { get; set; }
        public SalaDeExames? SalaDeExame { get; set; }


        // 5. Profissional Executante
        public int? ProfissionalExecutanteId { get; set; }
        public ProfissionalExecutante? ProfissionalExecutante { get; set; }

        // 6. Equipamento
        public int? MaterialEquipamentoAssociadoId { get; set; }
        public MaterialEquipamentoAssociado? MaterialEquipamentoAssociado { get; set; }


    }
}