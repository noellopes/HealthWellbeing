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
public static class Repository
{
    private static List<Consumivel> consumiveis = new();

    public static IEnumerable<Consumivel> Consumiveis => consumiveis;

    public static void AddResponse(Consumivel consumivel) => consumiveis.Add(consumivel);
}
