using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models.FileMedicalDevices
{
    [Table("MedicalDevices")]
    public class MedicalDevices
    {
        //ID dos dispositivos móveis
        [Key]
        public int DevicesID { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Deve conter no min 2 letras e 100 no max.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Tipo de Dispositivo é Obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Deve conter no min 2 letras e 50 no max.")]
        public string Type { get; set; }

        [Required(ErrorMessage = "Especificação é obrigatório.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Deve conter no min 5 letras e 200 no max.")]
        public string Specification { get; set; }

        [Required(ErrorMessage = "Quantidade é obrigatório.")]
        public int Quantity { get; set; }

        public DateTime RegistrationDate { get; set; }

        public string? Status { get; set; }

        public string? Observation { get; set; }

        public int? SalaID { get; set; }

    }
}
