using HealthWellbeing.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("Funcoes")] // <-- Defina explicitamente o nome da tabela na DB
public class Funcao
{
    [Key]
    public int FuncaoId { get; set; }

    [Required]
    public string Descricao { get; set; }

    // Propriedade de navegação inversa
    public ICollection<ProfissionalExecutante> ProfissionaisExecutantes { get; set; }
}