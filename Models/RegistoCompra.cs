namespace HealthWellbeing.Models.ViewModels
{
    public class RegistoCompra
    {
        public int ConsumivelId { get; set; }
        public int Quantidade { get; set; }

        public int? FornecedorId { get; set; }

        public List<Fornecedor_Consumivel> Fornecedores { get; set; }
            = new();
    }
}
