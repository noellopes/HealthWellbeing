using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class Stock
    {
        [Key]
        public int StockId { get; set; }

        // FK Consumivel
        [Required]
        [ForeignKey("Consumivel")]
        public int ConsumivelID { get; set; }
        public Consumivel Consumivel { get; set; }

        // FK Zona
        [Required]
        [ForeignKey("Zona")]
        public int ZonaID { get; set; }
        public ZonaArmazenamento Zona { get; set; }

        public int QuantidadeAtual { get; set; }
        public int QuantidadeMinima { get; set; }
        public int QuantidadeMaxima { get; set; }

        public DateTime DataUltimaAtualizacao { get; set; } = DateTime.Now;
    }
}
