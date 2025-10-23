namespace HealthWellbeing.Models
{
    public class RepositoryTypeTreatment
    {
        private static List<TypeTreatment> TypeTreatments { get; set; } = new List<TypeTreatment>();

        public static IEnumerable<TypeTreatment> TypeTreatmentList => TypeTreatments;
        public static void AddTypeTreatment(TypeTreatment typeTreatment)
        {
            if (TypeTreatments.Any())
                typeTreatment.TypeTreatmentId = TypeTreatments.Max(p => p.TypeTreatmentId) + 1;
            else
                typeTreatment.TypeTreatmentId = 1;

            TypeTreatments.Add(typeTreatment);
        }
    }
}