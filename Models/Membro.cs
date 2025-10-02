namespace HealthWellbeing.Models
{
    public class Membro
    {
        public int MembroId { get; set; } = default!;
        
        // Sendo Membro já é Cliente, logo podemos ir buscar as informções como nome, etc a essa class

        // Plano de cada membro (adicionar futuramente)
        public bool? HaveAPlan { get; set; } = default!;
        public string MembroType { get; set;} = default!;

        public string HaveAPlanText => HaveAPlan switch
        {
            true => "Member plans are available",
            false => "Member does not have a plan",
            _ => "The member's plans are still being processed",
        };

    }
}
