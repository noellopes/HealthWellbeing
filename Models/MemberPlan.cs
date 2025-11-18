using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HealthWellbeing.Models
{
    public class MemberPlan
    {
        public int MemberPlanId { get; set; }

        // Chaves Estrangeiras (Foreign Keys)
        public int MemberId { get; set; }
        public int PlanId { get; set; }

        public Member? Member { get; set; }
        public Plan? Plan { get; set; }

        // Atributos da Classe
        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Ex: "Active", "Expired", "Cancelled"
    }
}