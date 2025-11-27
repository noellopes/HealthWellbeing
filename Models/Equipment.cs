using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Deve conter no min 3 letras e 100 no max.")]
        public required string Name { get; set; }

        public string? Description { get; set; }

        [Required(ErrorMessage = "Número de Série é obrigatório.")]
        public required string SerialNumber { get; set; }

        [Required(ErrorMessage = "Sala é obrigatório.")]
        public int RoomId { get; set; }

        public Room? Room { get; set; }
   
        [Required(ErrorMessage = "Data de compra é obrigatório.")]
        public required DateTime PurchaseDate { get; set; }

        [Required(ErrorMessage = "Tipo de Equipamento é obrigatório.")]
        public int EquipmentTypeId { get; set; }

        public EquipmentType? EquipmentType { get; set; }

        [Required(ErrorMessage = "Estado é obrigatório.")]
        public int EquipmentStatusId { get; set; }

        public EquipmentStatus? EquipmentStatus { get; set; }
    }
}
