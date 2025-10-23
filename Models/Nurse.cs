using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Nurse
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "The name must have a maximum of 100 characters.")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "NIF is required.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "The NIF must contain 9 digits.")]
        public required string NIF { get; set; }

        [Required(ErrorMessage = "The professional license number is required.")]
        [StringLength(10, ErrorMessage = "The license must have a maximum of 10 characters.")]
        [Display(Name = "Professional License")]
        public required string ProfessionalLicense { get; set; }

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public required DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone")]
        public required string Phone { get; set; }

        public string? Specialty { get; set; }
    }
}
