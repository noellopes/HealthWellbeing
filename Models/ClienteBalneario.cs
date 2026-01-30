using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class ClienteBalneario
    {
        [Key]
        public int ClienteBalnearioId { get; set; }

        // =========================
        // DADOS PRINCIPAIS
        // =========================

        [Required(ErrorMessage = "O nome é obrigatório.")]
        [StringLength(120)]
        public string Nome { get; set; }

        [Required(ErrorMessage = "O email é obrigatório.")]
        [EmailAddress(ErrorMessage = "Introduza um email válido.")]
        [StringLength(150)]
        public string Email { get; set; }

        [Required(ErrorMessage = "O contacto é obrigatório.")]
        [RegularExpression(
            @"^(9[1236]\d{7})$",
            ErrorMessage = "Introduza um número de telemóvel válido em Portugal."
        )]
        public string Telemovel { get; set; }

        // =========================
        // ESTADO
        // =========================

        public DateTime DataRegisto { get; set; } = DateTime.Now;

        public bool Ativo { get; set; } = true;

        // =========================
        // FIDELIZAÇÃO
        // =========================

        [Range(0, int.MaxValue)]
        public int Pontos { get; set; } = 0;


        // =========================
        // SATISFAÇÃO
        // =========================
        public ICollection<SatisfacaoCliente> Satisfacoes { get; set; }
            = new List<SatisfacaoCliente>();


        // =========================
        // RELAÇÕES
        // =========================

        public ICollection<UtenteBalneario> Utentes { get; set; } = new List<UtenteBalneario>();
    }
}
