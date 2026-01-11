using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace HealthWellbeing.Models
{
    public class UsoConsumivel
    {
        [Key]
        public int UsoConsumivelId { get; set; }

        // FK para Consumível
        [Required(ErrorMessage = "O consumível é obrigatório.")]
        [Display(Name = "Consumível")]
        public int ConsumivelId { get; set; }

        [ForeignKey(nameof(ConsumivelId))]
        public Consumivel? Consumivel { get; set; }

        // FK para Zona de Armazenamento
        [Required(ErrorMessage = "A zona de armazenamento é obrigatória.")]
        [Display(Name = "Zona de Armazenamento")]
        public int ZonaArmazenamentoID { get; set; }

        [ForeignKey(nameof(ZonaArmazenamentoID))]
        public ZonaArmazenamento? ZonaArmazenamento { get; set; }

        // FK para TreatmentRecord
        [Required(ErrorMessage = "O registo de tratamento é obrigatório.")]
        [Display(Name = "Registo de Tratamento")]
        public int TreatmentRecordId { get; set; }

        [ForeignKey(nameof(TreatmentRecordId))]
        public TreatmentRecord? TreatmentRecord { get; set; }

        // Quantidade retirada
        [Required(ErrorMessage = "A quantidade usada é obrigatória.")]
        [Range(1, int.MaxValue, ErrorMessage = "A quantidade usada deve ser pelo menos 1.")]
        [Display(Name = "Quantidade Usada")]
        public int QuantidadeUsada { get; set; }

        // Data do consumo
        [Required]
        [Display(Name = "Data do Consumo")]
        public DateTime DataConsumo { get; set; } = DateTime.Now;

        [Required]
        public string UserId { get; set; } = string.Empty;

    }
}
