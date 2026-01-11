using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class HistoricoTransferencia
    {
        public int Id { get; set; }

        [Display(Name = "Data da Transferência")]
        public DateTime DataTransferencia { get; set; } = DateTime.Now;

        [Display(Name = "Realizado Por")]
        public string? Utilizador { get; set; } // O email/nome do user logado

        // O que foi transferido?
        public int ConsumivelId { get; set; }
        public Consumivel? Consumivel { get; set; }

        [Display(Name = "Quantidade")]
        public int Quantidade { get; set; }

        // De onde saiu?
        public int ZonaOrigemId { get; set; }
        [ForeignKey("ZonaOrigemId")]
        public ZonaArmazenamento? ZonaOrigem { get; set; }

        // Para onde foi?
        public int ZonaDestinoId { get; set; }
        [ForeignKey("ZonaDestinoId")]
        public ZonaArmazenamento? ZonaDestino { get; set; }
    }
}