using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class AuditoriaConsumivel
    {
        [Key]
        public int AuditoriaConsumivelId { get; set; }

        // FK para Sala
        public int? SalaId { get; set; }
        public Sala? Sala { get; set; }

        // FK para Consumivel
        public int? ConsumivelID { get; set; }
        public Consumivel? Consumivel { get; set; }

        public int QuantidadeUsada { get; set; }
        public DateTime DataConsumo { get; set; }
    }
}
