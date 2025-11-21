using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace HealthWellbeing.Models
{
    public class Level
    {
        public int LevelId { get; set; } // Primary Key

        [Display(Name = "Level Number")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [Range(1, 100, ErrorMessage = "{0} must be 1-100")]
        public int LevelNumber { get; set; } // Level number 1-100

        //public string LevelPoints { get; set; } // Total points of the level
        [Display(Name = "Level Category")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public string LevelCategory { get; set; } // Level Category (If X level then Y category and Z circle color)

        [Display(Name = "Description")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public string Description { get; set; }  // Description 
    }
}
