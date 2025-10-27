using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TreatmentType
    {

        //Propriedades
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is Required!!")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Description is Required!!")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "EstimatedDuration is Required!!")]
        public required int EstimatedDuration { get; set; }

        [Required(ErrorMessage = "Priority is Required!!")]
        public string? Priority { get; set; }         //Se é Urgente, Normal, Rotina

    }


    }
