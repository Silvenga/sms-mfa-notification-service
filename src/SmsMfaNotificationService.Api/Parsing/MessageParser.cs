using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace SmsMfaNotificationService.Api.Parsing
{
    public static class MessageParser
    {
        private static readonly Regex GenericPattern = new(@"\b(?<code>\d{4,8})\b", RegexOptions.Compiled);
        private static readonly Regex SeparatedPattern = new(@"\b(?<code>\d{3,4}[ -]+\d{3,4})\b", RegexOptions.Compiled);

        private static readonly Regex ZipCodePattern =
            new(
                @"(AL|NE|AK|NV|AZ|NH|AR|NJ|CA|NM|CO|NY|CT|NC|DE|ND|DC|OH|FL|OK|GA|OR|HI|PA|ID|PR|IL|RI|IN|SC|IA|SD|KS|TN|KY|TX|LA|UT|ME|VT|MD|VA|MA|VI|MI|WA|MN|WV|MS|WI|MO|WY|MT)[ ,]+\b\d{5}\b",
                RegexOptions.Compiled);

        private static readonly Regex RoadPattern = new(@"\b\d{3,4} [a-z]", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private static readonly Regex CreditCardPattern = new(@"ending (in )?\d{4}\b", RegexOptions.Compiled);
        private static readonly Regex PhoneNumberPattern = new(@"(\d{3}-){2}\d{4}", RegexOptions.Compiled);

        public static bool TryGetCode(string input, [NotNullWhen(true)] out string? code)
        {
            code = default;
            return !ZipCodePattern.IsMatch(input)
                   && !RoadPattern.IsMatch(input)
                   && !CreditCardPattern.IsMatch(input)
                   && !PhoneNumberPattern.IsMatch(input)
                   && (TryRegexMatch(GenericPattern, input, out code)
                       || TryRegexMatch(SeparatedPattern, input, out code));
        }

        private static bool TryRegexMatch(Regex regex, string input, [NotNullWhen(true)] out string? code)
        {
            var match = regex.Match(input);
            if (match.Success)
            {
                code = NormalizeCode(match.Groups["code"].Value);
                return true;
            }

            code = default;
            return false;
        }

        private static string NormalizeCode(string code)
        {
            var builder = new StringBuilder();
            foreach (var c in code)
            {
                if (char.IsDigit(c))
                {
                    builder.Append(c);
                }
            }

            return builder.ToString();
        }
    }
}
