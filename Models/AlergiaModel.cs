namespace HealthWellbeing.Models
{
    public class AlergiaModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public string Gravidade { get; set; }

        public string Sintomas { get; set; }

        //public ICollection<AlimentoModel> AlimentosRelacionados { get; set; } --> Quando implementar alimentos

    }
}
