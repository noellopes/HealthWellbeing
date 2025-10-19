using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;

namespace HealthWellbeing.Models
{
    public class Register
    {

        [Required(ErrorMessage = "Please enter the register id!")]
        public int registerId { get; set; } = default!;
        [Required(ErrorMessage = "Please enter the pacient id!")]
        public int pacientId { get; set; } = default!;
        [Required(ErrorMessage = "Please enter the doctor id!")]
        public int doctorId { get; set; } = default!;
        [Required(ErrorMessage = "Please enter the date!")]
        public string date { get; set; } = default!;
        [Required(ErrorMessage = "Please write the diagnostic!")]
        public string diagnostic { get; set; } = default!;
        [Required(ErrorMessage = "Please write the observations!")]
        public string observations { get; set; } = default!;

    }
}
