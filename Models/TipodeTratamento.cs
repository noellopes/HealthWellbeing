using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class TipodeTratamento
    {

        //Propriedades
        public int TipoTratamentoId { get; set; }

        [Required(ErrorMessage = "Name is Required!!")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is Required!!")]
        public string Description { get; set; }

        [Required(ErrorMessage = "EstimatedDuration is Required!!")]
        public int EstimatedDuration { get; set; }

        [Required(ErrorMessage = "Priority is Required!!")]
        public string Priority { get; set; }    //Se é Urgente, Normal, Rotina


    }


    }
