using System;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Client
    {

        [Key]
        public int ClientId { get; set; }


        [Required(ErrorMessage = "First and last name are necessary.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "The name must have at least 6 chars and no more than 100.")]
        [RegularExpression(
            @"^[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+(?:\s[A-Za-zÁÉÍÓÚÂÊÔÃÕÇáéíóúâêôãõç]+)+$",
            ErrorMessage = "Please introduce at least a First and Last name!"
        )]
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
        public DateTime? BirthDate { get; set; }


        [StringLength(10)]
        public string Gender { get; set; } = string.Empty;


        [DataType(DataType.Date)]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;


        public bool? CreateMember { get; set; }

        public string CreateMemberText =>
            CreateMember switch
            {
                true => "Yes",
                false => "No",
                _ => "Pending Acceptation..",
            };

        [Range(0, 500, ErrorMessage = "Weight must be positive.")]
        public double WeightKg { get; set; } = 70.0;

        [Range(0, 300, ErrorMessage = "Height must be positive.")]
        public int HeightCm { get; set; } = 170;

        
        [Range(1.0, 2.5)]
        public double ActivityFactor { get; set; } = 1.5;

        // Helper: calcula idade usando BirthDate; se não houver BirthDate, assumimos 30 anos por defeito.
        public int Age
        {
            get
            {
                if (BirthDate.HasValue)
                {
                    var today = DateTime.Today;
                    var age = today.Year - BirthDate.Value.Year;
                    if (BirthDate.Value.Date > today.AddYears(-age)) age--;
                    return age;
                }
                return 30; // fallback razoável
            }
        }
    
        public Member? Membership { get; set; }


        public ICollection<ClientAlergy>? ClientAlergies { get; set; }

        public ICollection<Goal>? Goals { get; set; }

        public ICollection<NutritionistClientPlan>? NutritionistClientPlans { get; set; }
    }
}
