using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeingRoom.Models
{
    [Table("MedicalDevices")]
    public class MedicalDevice
    {
        //ID dos dispositivos móveis
        [Display(Name = "ID do dispositivo")]
        [Key]
        public int MedicalDeviceID { get; set; }

        [Display(Name = "Nome")]
        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Deve conter no min 3 letras e 100 no max.")]
        public string Name { get; set; }

        [Display(Name = "Tipo")]
        //Posteriormente vou mudar o nome do tipo de dispositivos
        [Required(ErrorMessage = "Tipo de Dispositivo é Obrigatório.")]
        public string Type { get; set; }

        [Display(Name = "Especificação")]
        [Required(ErrorMessage = "Especificação é obrigatório.")]
        public string Specification { get; set; }

        [Display(Name = "Data de Registo")]
        public DateTime RegistrationDate { get; set; }

        [Display(Name = "Observação")]
        public string? Observation { get; set; }

    }
}
