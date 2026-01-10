using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.ViewModels
{
    public class ClientesPerfilAlimentarViewModel
    {
        public List<ClienteResumoVm> Clientes { get; set; } = new();
    }

    public class ClienteResumoVm
    {
        public string ClientId { get; set; } = string.Empty;
        public string Nome { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    public class GerirPerfilAlimentarViewModel
    {
        public string ClientId { get; set; } = string.Empty;
        public string ClientNome { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;

        public List<ItemAssociadoVm> Alergias { get; set; } = new();
        public List<ItemAssociadoVm> Restricoes { get; set; } = new();
        public List<MetaResumoVm> Metas { get; set; } = new();

        public List<SelectOptionIntVm> OpcoesAlergias { get; set; } = new();
        public List<SelectOptionIntVm> OpcoesRestricoes { get; set; } = new();

        public string? Aviso { get; set; }
    }

    public class MeuPerfilAlimentarViewModel
    {
        public string ClientNome { get; set; } = string.Empty;

        public List<ItemAssociadoVm> Alergias { get; set; } = new();
        public List<ItemAssociadoVm> Restricoes { get; set; } = new();
        public List<MetaResumoVm> Metas { get; set; } = new();

        public List<SelectOptionIntVm> OpcoesAlergias { get; set; } = new();
        public List<SelectOptionIntVm> OpcoesRestricoes { get; set; } = new();

        public string? Aviso { get; set; }
    }

    public class ItemAssociadoVm
    {
        public int Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public string? Descricao { get; set; }
    }

    public class MetaResumoVm
    {
        public int MetaId { get; set; }
        public string MetaDescription { get; set; } = string.Empty;
        public int DailyCalories { get; set; }
        public int DailyProtein { get; set; }
        public int DailyFat { get; set; }
        public int DailyHydrates { get; set; }
        public int DailyVitamins { get; set; }
    }

    public class SelectOptionIntVm
    {
        public int Value { get; set; }
        public string Text { get; set; } = string.Empty;
    }

    public class AddAssociacaoPost
    {
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public int ItemId { get; set; }
    }

    public class EditAssociacaoPost
    {
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public int OldItemId { get; set; }

        [Required]
        public int NewItemId { get; set; }
    }

    public class DeleteAssociacaoPost
    {
        [Required]
        public string ClientId { get; set; } = string.Empty;

        [Required]
        public int ItemId { get; set; }
    }

    public class MetaEditViewModel
    {
        public int? MetaId { get; set; }

        public string ClientId { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }

        [Required(ErrorMessage = "A descrição da meta é obrigatória.")]
        [StringLength(200, ErrorMessage = "A descrição deve ter no máximo 200 caracteres.")]
        [Display(Name = "Descrição")]
        public string MetaDescription { get; set; } = string.Empty;

        [Range(0, 20000, ErrorMessage = "As calorias diárias devem ser um valor positivo.")]
        [Display(Name = "Calorias diárias")]
        public int DailyCalories { get; set; }

        [Range(0, 1000, ErrorMessage = "A proteína diária deve ser um valor positivo.")]
        [Display(Name = "Proteína diária")]
        public int DailyProtein { get; set; }

        [Range(0, 1000, ErrorMessage = "A gordura diária deve ser um valor positivo.")]
        [Display(Name = "Gordura diária")]
        public int DailyFat { get; set; }

        [Range(0, 1000, ErrorMessage = "Os hidratos diários devem ser um valor positivo.")]
        [Display(Name = "Hidratos diários")]
        public int DailyHydrates { get; set; }

        [Range(0, 1000, ErrorMessage = "As vitaminas diárias devem ser um valor positivo.")]
        [Display(Name = "Vitaminas diárias")]
        public int DailyVitamins { get; set; }
    }
}
