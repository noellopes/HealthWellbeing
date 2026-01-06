using System.ComponentModel.DataAnnotations;

namespace HealthWellbeing.Models.Validation
{
    public class NifAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            var nif = value?.ToString();

            if (string.IsNullOrWhiteSpace(nif))
                return true; // Required trata disso

            if (nif.Length != 9 || !long.TryParse(nif, out _))
                return false;

            int[] pesos = { 9, 8, 7, 6, 5, 4, 3, 2 };
            int soma = 0;

            for (int i = 0; i < 8; i++)
            {
                soma += (nif[i] - '0') * pesos[i];
            }

            int resto = soma % 11;
            int digito = resto < 2 ? 0 : 11 - resto;

            return (nif[8] - '0') == digito;
        }
    }
}
