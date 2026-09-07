using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Core_project_BusinessLogic.Entity
{
    /// <summary>
    /// Rejects ~, !, and ^ in editor HTML/plain text content.
    /// </summary>
    public sealed class NoEditorSpecialCharsAttribute : ValidationAttribute
    {
        private static readonly Regex ForbiddenChars = new(@"[~!^]", RegexOptions.Compiled);
        private static readonly Regex HtmlTagPattern = new(@"<[^>]*>", RegexOptions.Compiled);

        public NoEditorSpecialCharsAttribute()
        {
            ErrorMessage = "Characters ~, !, and ^ are not allowed.";
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is not string text || string.IsNullOrWhiteSpace(text))
            {
                return ValidationResult.Success;
            }

            string plainText = HtmlTagPattern.Replace(text, string.Empty);
            plainText = System.Net.WebUtility.HtmlDecode(plainText);

            if (ForbiddenChars.IsMatch(plainText))
            {
                return new ValidationResult(ErrorMessage, new[] { validationContext.MemberName! });
            }

            return ValidationResult.Success;
        }
    }
}
