using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace HealthWellbeing.Models
{
    public class RegistoConsulta
    {

        [Required(ErrorMessage = "Por favor introduza o id de registo")]
        public int IdRegisto { get; set; } = default!;

    }
}
