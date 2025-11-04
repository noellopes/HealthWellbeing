using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class MemberPlan
    {
        public int MemberPlanId { get; set; }

        [Required]
        public int MemberId { get; set; }
        public Member? Member { get; set; }

        [Required]
        public int PlanId { get; set; }
        public Plan? Plan { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required(ErrorMessage = "The status is required")]
        [StringLength(20)]
        public string Status { get; set; } = "Active";
        // Possible values: Active, Pending, Completed, Cancelled
    }
}
