using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class Member
    {   
        public int MemberId { get; set; } = default!;

        public int ClientId { get; set; } = default!;
        public Client? Client { get; set; } = default;

        public ICollection<MemberPlan> MemberPlans { get; set; } = new List<MemberPlan>();

        public virtual ICollection<PhysicalAssessment> PhysicalAssessments { get; set; } = new List<PhysicalAssessment>();
    }
}
