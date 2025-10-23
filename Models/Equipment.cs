using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models
{
    public class Equipment
    {
        public int EquipmentId { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Deve conter no min 3 letras e 100 no max.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Descrição é obrigatório.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Deve conter no min 5 letras e 200 no max.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Quatidade é obrigatório.")]
        public int Quantity { get; set; }

        public int SalaId { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}
