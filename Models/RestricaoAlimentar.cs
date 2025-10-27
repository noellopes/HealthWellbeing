namespace HealthWellbeing.Models
{
    public enum TipoRestricao
    {
        Vegetariana,
        Vegana,
        IntoleranciaLactose,
        IntoleranciaGluten,
        AlergiaSoja,
        AlergiaOleaginosas,
        AlergiaFrutosDoMar,
        SemAcucar,
        BaixoSodio,
        Outro
    }

    public enum GravidadeRestricao
    {
        Leve,
        Moderada,
        Grave
    }

    public class RestricaoAlimentar
    {
        public int Id { get; set; }

        
        public string Nome { get; set; }

       
        public TipoRestricao Tipo { get; set; }

        
        public GravidadeRestricao Gravidade { get; set; }

        
        public string Sintomas { get; set; }
    }
}
