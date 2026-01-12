using HealthWellbeing.Models;
using HealthWellbeing.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellBeing.Models
{
    public class Exame
    {
        // --- CHAVE PRIMÁRIA ---
        public int ExameId { get; set; }

        // --- DATA E HORA ---
        [Required(ErrorMessage = "A data e hora da marcação são obrigatórias.")]
        [DataType(DataType.DateTime)]
        public DateTime DataHoraMarcacao { get; set; } = DateTime.Now;

        // --- ESTADO ---
        [Required(ErrorMessage = "O estado da marcação é obrigatório.")]
        [StringLength(50)]
        public string Estado { get; set; } = string.Empty;

        // --- NOTAS ---
        [StringLength(500)]
        public string? Notas { get; set; }

        // --- CHAVES ESTRANGEIRAS ---

        [Required(ErrorMessage = "O Utente é obrigatório.")]
        public int UtenteId { get; set; }
        public Utente? Utente { get; set; }

        [Required(ErrorMessage = "O Tipo de Exame é obrigatório.")]
        public int ExameTipoId { get; set; }
        public ExameTipo? ExameTipo { get; set; }

        [Required(ErrorMessage = "O Médico solicitante é obrigatório.")]
        public int MedicoSolicitanteId { get; set; }
        public Medicos? MedicoSolicitante { get; set; }

        [Required(ErrorMessage = "O Profissional Executante é obrigatório.")]
        public int ProfissionalExecutanteId { get; set; }
        public ProfissionalExecutante? ProfissionalExecutante { get; set; }

        [Required(ErrorMessage = "A Sala de Exames é obrigatória.")]
        public int SalaDeExameId { get; set; }
        public SalaDeExames? SalaDeExame { get; set; }

        [Required(ErrorMessage = "O Equipamento é obrigatório.")]

        public int MaterialEquipamentoAssociadoId { get; set; }
        public virtual ICollection<RegistoMateriais> RegistoMateriais { get; set; } = new List<RegistoMateriais>();

        // --- OPÇÕES FIXAS ---
        public static readonly IReadOnlyList<string> OpcoesEstado = new List<string>
        {
            "Marcado",
            "Remarcado",
            "Cancelado",
            "Realizado"
        };
    }
}
