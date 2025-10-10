using System.ComponentModel.DataAnnotations;

namespace HealthWellbeingRoom.Models.FileMobileDevices
{
    public class MobileDevices
    {
        //ID dos dispositivos móveis
        public int DevicesID { get; set; }

        [Required(ErrorMessage = "Nome é obrigatório.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Deve conter no min 2 letras e 100 no max.")]
        public string NomeDisp { get; set; }

        [Required(ErrorMessage = "Tipo de Dispositivo é Obrigatório.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Deve conter no min 2 letras e 50 no max.")]
        public string TipoDisp { get; set; }

        [Required(ErrorMessage = "Especificação é obrigatório.")]
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Deve conter no min 5 letras e 200 no max.")]
        public string EspecificacaoDisp { get; set; }

        [Required]
        public int QuantidadeDisp { get; set; }

        public DateTime DataRegisto { get; set; }
    }
}
