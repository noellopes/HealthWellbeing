namespace HealthWellbeing.Models;

public class Consumivel
{
    public int ConsumivelId { get; set; }
    public string Categoria { get; set; }
    public string Nome { get; set; }
    public string ZonaArmazenamento { get; set; }
    public List<string> Fornecedores { get; set; }
    public int Stock { get; set; }
    public int SalaId { get; set; }

}
