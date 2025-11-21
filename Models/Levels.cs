using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
namespace HealthWellbeing.Models
{
    public class Level
    {
        public int LevelId { get; set; } // Primary Key
        [Range(1, 100, ErrorMessage = "Level must be 1-100")]
        public int LevelNumber { get; set; } // Level number 1-100
        //[ValidateNever]
        //public string LevelPoints { get; set; } // Total points of the level
        [ValidateNever]
        public string LevelCategory { get; set; } // Level Category (If X level then Y category and Z circle color)
        [ValidateNever]
        public string Description { get; set; }  // Description 


        

    }
}
