namespace HealthWellbeing.Models
{
    public class PathologyRepository
    {
        private static List<Pathology> Pathologys { get; set; } = new List<Pathology>();

        private static List<Pathology> PathologyList { get; } //(É A MESMA COISA Q A LINHA DE BAIXO)
        
        //public static IEnumerable<Patology> PathologyList => Patologys;

        public static void AddPathology(Pathology pathology)
        {
            Pathologys.Add(pathology);
        }

    }
}
