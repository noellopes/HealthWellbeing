namespace HealthWellbeing.Models
{
    public class Register
    {
        public int registerId { get; set; }
        public int pacientId { get; set; }
        public int doctorId { get; set; }
        public string date { get; set; }
        public string diagnostic { get; set; }
        public string observations { get; set; }

    }
}
