namespace HealthWellbeing.Models
{
    public class ReceitaModel
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descricao { get; set; }

        public string ModoPreparo { get; set; }

        public int TempoPreparo { get; set; }

        public int Porcoes { get; set; }

        public decimal CaloriasPorPorcao { get; set; }

        public decimal Proteinas { get; set; }

        public decimal HidratosCarbono { get; set; }

        public decimal Gorduras { get; set; }

        public bool IsVegetariana { get; set; }

        public bool IsVegan { get; set; }

        public bool IsLactoseFree { get; set; }

        //public ICollection<ComponenteReceitaModel> Componentes { get; set; } --> Quando implementar componentes da receita

        //public ICollection<AlergiaModel> RestricaoAlergias { get; set; } --> Quando implementar restrições alimentares

        //public ICollection<CategoriaAlimentoModel> CategoriasAlimentos { get; set; } --> Quando implementar categorias

    }
}