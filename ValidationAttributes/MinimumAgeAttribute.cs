using System.ComponentModel.DataAnnotations;
using System;

namespace HealthWellbeing.ValidationAttributes
{
    // Criamos o nosso atributo personalizado, que herda de ValidationAttribute
    public class MinimumAgeAttribute : ValidationAttribute
    {
        private readonly int _minimumAge;

        // O construtor vai receber a idade mínima que queremos (ex: 16)
        public MinimumAgeAttribute(int minimumAge)
        {
            _minimumAge = minimumAge;
        }

        // Esta é a função principal que faz a validação
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Se o valor for nulo (não preenchido), não damos erro aqui.
            // Deixamos o [Required] tratar disso, se ele existir.
            if (value == null)
            {
                return ValidationResult.Success;
            }

            // Tentamos converter o valor para uma DateTime
            if (value is not DateTime birthDate)
            {
                // Se não for uma data, retornamos um erro genérico
                return new ValidationResult("Data de nascimento inválida.");
            }

            // --- A LÓGICA PRINCIPAL ---
            // 1. Calculamos a data "limite". 
            //    Ex: Se hoje é 17/11/2025, a data limite é 17/11/2009.
            DateTime cutoffDate = DateTime.Today.AddYears(-_minimumAge);

            // 2. Comparamos as datas.
            //    Se a data de nascimento for *DEPOIS* da data limite, a pessoa é muito nova.
            //    Ex: 18/11/2009 (é mais recente) > 17/11/2009 (limite) -> ERRO
            //    Ex: 17/11/2009 (é igual) > 17/11/2009 (limite) -> OK
            //    Ex: 16/11/2009 (é mais antiga) > 17/11/2009 (limite) -> OK
            if (birthDate.Date > cutoffDate.Date)
            {
                // Retorna a mensagem de erro que definimos ao usar o atributo
                return new ValidationResult(ErrorMessage ?? $"O cliente deve ter pelo menos {_minimumAge} anos.");
            }

            // Se chegou aqui, a data é válida
            return ValidationResult.Success;
        }
    }
}