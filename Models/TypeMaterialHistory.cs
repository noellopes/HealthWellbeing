using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class TypeMaterialHistory
    {
        public int TypeMaterialHistoryId { get; set; }

        [Required]
        public int TypeMaterialID { get; set; }

        [ForeignKey("TypeMaterialID")]
        public TypeMaterial TypeMaterial { get; set; }

        [Required]
        public string OldName { get; set; }

        [Required]
        public string NewName { get; set; }

        public string OldDescription { get; set; }
        public string NewDescription { get; set; }

        [Required]
        public string ChangedBy { get; set; }

        [Required]
        public DateTime ChangedAt { get; set; }
    }
}

