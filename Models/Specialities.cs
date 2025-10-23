namespace HealthWellbeing.Models
{
    public class Specialities
    {

        

        public int EspecialidadeId { get; set; }               
        public int MedicoId { get; set; }
        public string Nome { get; set; } = "";      // Nome da especilidade    
        public string Descricao { get; set; } = "";

    }

    
}
