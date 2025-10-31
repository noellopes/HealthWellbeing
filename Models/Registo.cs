namespace HealthWellbeing.Models
{
    public class Registo
    {
        public int RegistoID { get; set; }
        public int UserId { get; set; }
        public DateTime MealDateTime { get; set; }
        public string MealType { get; set; }//Breakfest,lunch ,dinner
        public string FoodName { get; set; }
        public double Quantity { get; set; }//Portion or unit
        public string Notes { get; set; }
    }
}
