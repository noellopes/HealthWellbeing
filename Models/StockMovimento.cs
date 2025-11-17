using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class StockMovimento
    {
        [Key]
        public int Id { get; set; }

        // Relacionado com a tabela Stock
        [Required]
        public int StockId { get; set; }

        [ForeignKey("StockId")]
        public Stock Stock { get; set; }

        // Quantidade movimentada (compra/entrada)
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        // "Entrada" ou "Saida"
        [Required]
        [StringLength(20)]
        public string Tipo { get; set; } = "Entrada";

        // Data do movimento
        public DateTime Data { get; set; } = DateTime.Now;

        // Descrição opcional
        [StringLength(200)]
        public string? Descricao { get; set; }
    }
}

