using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.ViewModels
{
    public class FoodHabitsPlanFormVM
    {
        public int? FoodHabitsPlanId { get; set; }

        [Required(ErrorMessage = "Client is required.")]
        public int ClientId { get; set; }

        [Required(ErrorMessage = "Goal is required.")]
        public int GoalId { get; set; }

        [Required(ErrorMessage = "Starting date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartingDate { get; set; }

        [Required(ErrorMessage = "Ending date is required.")]
        [DataType(DataType.Date)]
        public DateTime EndingDate { get; set; }

        public List<FoodLineVM> Foods { get; set; } = new();

        public class FoodLineVM
        {
            public bool Selected { get; set; }
            public int FoodId { get; set; }
            public int PortionId { get; set; }

            public string FoodName { get; set; } = "";
            public string CategoryName { get; set; } = "";
        }
    }
}
