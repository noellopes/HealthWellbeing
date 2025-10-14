using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace HealthWellbeing.Models
{
    public class Register
    {

        [Required(ErrorMessage = "Please enter the register id!")]
        public int registerId { get; set; } = default!;
        public int pacientId { get; set; } = default!;
        public int doctorId { get; set; } = default!;
        public string date { get; set; } = default!;
        public string diagnostic { get; set; } = default!;
        public string observations { get; set; } = default!;

    }
}
