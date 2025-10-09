using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TypeTreatment
    {

        //Propriedades
        public int TypeTreatmentId { get; set; }

        [Required(ErrorMessage = "Name is Required!!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is Required!!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "EstimatedDuration is Required!!")]
        public int EstimatedDuration { get; set; }

        public bool? Priority { get; set; }         //Se é Urgente, Normal, Rotina

        public string IsPriorityText => Priority switch
        {
            true => "Urgent",
            false => "Normal",
            _ => "Routine",
        };    


    }


    }
