using System;
using System.ComponentModel.DataAnnotations;
using HealthWellbeing.ValidationAttributes;

namespace HealthWellbeing.Models
{
    public class Client
    {
        public string ClientId { get; set; } = string.Empty;

        [Required(ErrorMessage = "First and last name are necessary.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The name must have at least 6 chars and no more than 100.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+(?:\s[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+)+$", ErrorMessage = "Please introduce at least a First and Last name!")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A phone number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(15)]
        public string Phone { get; set; } = string.Empty;

        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [DataType(DataType.Date)]

        // PASSO 2: (Recomendado) Tornar a data obrigatória
        [Required(ErrorMessage = "BirthDate is mandatory")]
        [MinimumAge(16, ErrorMessage = "The client needs to be at least 16 years old.")]
        public DateTime? BirthDate { get; set; }

        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
        public Member? Membership { get; set; }
    }
}