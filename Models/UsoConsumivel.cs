using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class UsoConsumivel
    {
        [Key]
        public int AuditoriaConsumivelId { get; set; }

        // FK para TreatmentRecord (novo)
        [Required(ErrorMessage = "É necessário associar o registo de tratamento.")]
        [Display(Name = "Registo de Tratamento")]
        public int TreatmentRecordId { get; set; }

        [ForeignKey(nameof(TreatmentRecordId))]
        public TreatmentRecord? TreatmentRecord { get; set; }

        // FK para Consumível
        [Required]
        [Display(Name = "Consumível")]
        public int ConsumivelID { get; set; }

        [ForeignKey(nameof(ConsumivelID))]
        public Consumivel? Consumivel { get; set; }

        [Required]
        [Display(Name = "Quantidade usada")]
        public int QuantidadeUsada { get; set; }

        [Required]
        [Display(Name = "Data do consumo")]
        public DateTime DataConsumo { get; set; } = DateTime.Now;
    }
}
