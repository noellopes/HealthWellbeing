using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class HistoricoCompras
    {
        [Key]
        public int Id { get; set; }

        // Relacionado com a tabela Stock
        [Required(ErrorMessage = "Selecione um consumível.")]
        public int StockId { get; set; }

        [ForeignKey("StockId")]
        [ValidateNever]
        public Stock Stock { get; set; }

        // Quantidade movimentada (compra/entrada)
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade deve ser maior que zero.")]
        public int Quantidade { get; set; }

        // Novo: Fornecedor opcional
        public int? FornecedorId { get; set; }

        [ForeignKey("FornecedorId")]
        [ValidateNever]
        public Fornecedor? Fornecedor { get; set; }

        // Tipo do movimento - definido no Controller (NÃO pode ser Required)
        [StringLength(20)]
        public string Tipo { get; set; } = "Entrada";

        // Data do movimento
        public DateTime Data { get; set; } = DateTime.Now;

        // Descrição opcional
        [StringLength(200)]
        public string? Descricao { get; set; }
    }
}
