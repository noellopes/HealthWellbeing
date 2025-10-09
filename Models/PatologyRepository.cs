namespace HealthWellbeing.Models
{
    public class PatologyRepository
    {
        private static List<Patology> Patologys { get; set; } = new List<Patology>();

        private static List<Patology> PatologyList { get; } //(É A MESMA COISA Q A LINHA DE BAIXO)
        
        //public static IEnumerable<Patology> PatologyList => Patologys;

        public static void AddGuest(Patology patology)
        {
            Patologys.Add(patology);
        }

    }
}
