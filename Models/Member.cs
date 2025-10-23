namespace HealthWellbeing.Models
{
    public class Member
    {
        public int MemberId { get; set; } = default!;

        public string ClientRecognizer { get; set; } = default!;

        // Sendo Membro já é Cliente, logo podemos ir buscar as informções como nome, etc a essa class

        // Plano de cada membro (adicionar futuramente)
        public bool? HaveAPlan { get; set; } = default!;
        public string MemberType { get; set;} = default!;

        public string HaveAPlanText => HaveAPlan switch
        {
            true => "Member plans are available",
            false => "Member does not have a plan",
            _ => "The member's plans are still being processed",
        };

    }
}
