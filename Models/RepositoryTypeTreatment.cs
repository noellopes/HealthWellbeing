namespace HealthWellbeing.Models
{
    public class RepositoryTypeTreatment
    {
        private static List<TypeTreatment> TypeTreatments { get; set; } = new List<TypeTreatment>();

        public static IEnumerable<TypeTreatment> TypeTreatmentList => TypeTreatments;


        public static void AddTypeTreatment(TypeTreatment typeTreatment) => TypeTreatments.Add(typeTreatment);
    }
}