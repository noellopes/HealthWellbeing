using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models
{
    public class CompraOpcao
    {
        [Key]
        public int CompraOpcaoId { get; set; }

        // 🔹 Consumível ao qual esta opção pertence
        public int ConsumivelId { get; set; }
        public Consumivel? Consumivel { get; set; }

        // 🔹 Fornecedor que vende esta opção
        public int FornecedorId { get; set; }
        public Fornecedor? Fornecedor { get; set; }

        // 🔹 Preço total para esta opção
        public decimal Preco { get; set; }

        // 🔹 Tempo de entrega previsto (dias)
        public int TempoEntrega { get; set; }

        // 🔹 Quantidade que este fornecedor vende nesta opção
        public int Quantidade { get; set; }

        // Data em que a opção foi registada
        public DateTime DataRegisto { get; set; } = DateTime.Now;
    }
}
