using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class VoucherCliente
    {
        [Key]
        public int VoucherClienteId { get; set; }

        // =========================
        // DADOS DO VOUCHER
        // =========================

        [Required]
        [StringLength(100)]
        public string? Titulo { get; set; }

        [StringLength(250)]
        public string? Descricao { get; set; }

        [Required]
        [Range(1, 10000)]
        public int PontosNecessarios { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Valor { get; set; }

        public DateTime DataCriacao { get; set; } = DateTime.Now;

        public DateTime DataValidade { get; set; }

        public bool Usado { get; set; } = false;

        public DateTime? DataUtilizacao { get; set; }

        // =========================
        // RELAÇÃO
        // =========================

        public int ClienteBalnearioId { get; set; }
        public ClienteBalneario ClienteBalneario { get; set; }
    }
}
