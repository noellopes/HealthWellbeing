using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class LocationMaterial
    {

        public int LocationMaterialID { get; set; }

        public string Sector { get; set; }
        [Required(ErrorMessage = "The sector type is required.")]
        [StringLength(100, ErrorMessage = "The sector cannot exceed 100 characters.")]
        public string Room { get; set; }
        [Required(ErrorMessage = "The room name is required.")]
        [StringLength(100, ErrorMessage = "The room name cannot exceed 100 characters.")]
        public string Cabinet { get; set; }
        [Required(ErrorMessage = "The cabinet name is required.")]
        [StringLength(100, ErrorMessage = "The cabinet name cannot exceed 100 characters.")]
        public string Drawer { get; set; }
        [Required(ErrorMessage = "The drawer number is required.")]
        [StringLength(50, ErrorMessage = "The drawer cannot exceed 50 characters.")]
        public string Shelf { get; set; }
        [Required(ErrorMessage = "The shelf number or name is required.")]
        [StringLength(50, ErrorMessage = "The shelf cannot exceed 50 characters.")]
        public string IdentificationCode { get; set; }
        [Required(ErrorMessage = "The identification code is required.")]
        [StringLength(50, ErrorMessage = "The identification code cannot exceed 50 characters.")]
        public string Observation { get; set; }




    }
}
