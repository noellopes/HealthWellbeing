namespace HealthWellbeing.Models
{
    public class PathologyRepository
    {
        private static List<Pathology> Pathologys { get; set; } = new List<Pathology>();

        //private static List<Pathology> PathologyList { get; } //(É A MESMA COISA Q A LINHA DE BAIXO)
        
        public static IEnumerable<Pathology> PathologyList => Pathologys;

        public static void AddPathology(Pathology pathology)
        {
            if (Pathologys.Any())
                pathology.PathologyId = Pathologys.Max(p => p.PathologyId) + 1;
            else
                pathology.PathologyId = 1;

            Pathologys.Add(pathology);
        }

    }
}
