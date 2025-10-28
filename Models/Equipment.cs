using HealthWellbeing.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Deve conter no min 3 letras e 100 no max.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatório.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Deve conter no min 5 letras e 200 no max.")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Quatidade é obrigatório.")]
        public required int Quantity { get; set; }

        public String? Manufacturer { get; set; }

        [Required(ErrorMessage = "Número de Série é obrigatório.")]
        [StringLength(20, MinimumLength = 3, ErrorMessage = "Deve conter no min 3 letras e 20 no max.")]
        public required String SerialNumber { get; set; }

        public int RoomId { get; set; }

        public Room? Room { get; set; }

        [Required(ErrorMessage = "Data de compra é obrigatório.")]
        public required DateTime PurchaseDate { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
