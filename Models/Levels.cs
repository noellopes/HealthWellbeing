using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace HealthWellbeing.Models
{
    public class Level
    {
        // Primary Key
        public int LevelId { get; set; }



        // Level number 1-100
        [Display(Name = "Level Number")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        [Range(1, 100, ErrorMessage = "{0} must be 1-100")]
        public int LevelNumber { get; set; }



        // Foreign key (Category ID)
        [Display(Name = "Category")]
        [Required(ErrorMessage = "Please select a category.")]
        public int LevelCategoryId { get; set; }

        // Navigation property
        [ForeignKey("LevelCategoryId")]
        [ValidateNever]
        public LevelCategory Category { get; set; }


        // Level Points Limit
        [Display(Name = "Points Limit")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public int LevelPointsLimit { get; set; }



        // Description 
        [Display(Name = "Description")]
        [Required(ErrorMessage = "{0} cannot be empty.")]
        public string Description { get; set; }
    }
}
