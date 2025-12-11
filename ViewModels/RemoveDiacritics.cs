using System.Globalization;
using System.Text;

namespace HealthWellbeingRoom.ViewModels
{
    public static class RemoveDiacritics
    {
        public static string Normalize(string text)
        {
            if (string.IsNullOrWhiteSpace(text)) return text;

            // Normaliza o texto para decompor os acentos
            var normalized = text.Normalize(NormalizationForm.FormD);

            // Remove os caracteres de marcação (acentos)
            var chars = normalized.Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark);

            // Reconstrói o texto sem acentos e converte para minúsculas
            return new string(chars.ToArray()).Normalize(NormalizationForm.FormC).ToLowerInvariant();
        }
    }
}
