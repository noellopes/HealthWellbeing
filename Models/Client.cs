using System;
using System.ComponentModel.DataAnnotations;
using HealthWellbeing.ValidationAttributes;
using Microsoft.EntityFrameworkCore;

namespace HealthWellbeing.Models
{
    [Index(nameof(Email), IsUnique = true)]
    public class Client
    {
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Please enter the client's name.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "{0} must have between {2} and {1} characters.")]
        [RegularExpression(@"^[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+(?:\s[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+)+$",
            ErrorMessage = "Please introduce at least a First and Last name.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the {0}.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        [StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "A phone number is required.")]
        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(20)]
        public string Phone { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please enter the {0}.")]
        [StringLength(200)]
        public string Address { get; set; } = string.Empty;

        [Required(ErrorMessage = "Birth date is mandatory.")]
        [DataType(DataType.Date)]
        [MinimumAge(16, ErrorMessage = "The client needs to be at least 16 years old.")]
        public DateTime BirthDate { get; set; }

        [Required(ErrorMessage = "Please specify the gender.")]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Registration Date")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;

        public Member? Membership { get; set; }
    }
}