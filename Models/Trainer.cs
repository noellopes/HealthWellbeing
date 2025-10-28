using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }

        [Required(ErrorMessage = "First and last name are necessary.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The name must have at least 6 chars and no more than 100.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+(?:\s[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+)+$", ErrorMessage = "Please introduce at least a First and Last name!")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50)]
        public string Speciality { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A phone number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;
    }
}
