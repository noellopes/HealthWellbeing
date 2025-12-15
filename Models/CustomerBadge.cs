using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models {
    public class CustomerBadge {
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }
        public virtual Customer? Customer { get; set; }

        [Display(Name = "Badge")]
        public int BadgeId { get; set; }
        public virtual Badge? Badge { get; set; }

        [Display(Name = "Date Awarded")]
        public DateTime DateAwarded { get; set; } = DateTime.Now;
    }
}
