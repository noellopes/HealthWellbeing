using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models {
    public class CustomerActivity {
        public int CustomerActivityId { get; set; }

        [Display(Name = "Completion Date")]
        [Required]
        public DateTime CompletionDate { get; set; } = DateTime.Now;

        [Display(Name = "Points Earned")]
        [Required]
        public int PointsEarned { get; set; }


        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        public virtual Customer? Customer { get; set; }

        [Display(Name = "Activity")]
        public int ActivityId { get; set; }

        public virtual Activity? Activity { get; set; }

        [Display(Name = "Event")]
        public int? EventId { get; set; }

        public virtual Event? Event { get; set; }
    }
}