using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace SmsMfaNotificationService.Api.Parsing
{
    public static class MessageParser
    {
        private static readonly Regex GenericPattern = new(@"\b(?<code>\d{5,8})\b", RegexOptions.Compiled);
        private static readonly Regex SeparatedPattern = new(@"\b(?<code>\d{3,4}[ -]+\d{3,4})\b", RegexOptions.Compiled);

        public static bool TryGetCode(string input, [NotNullWhen(true)] out string? code)
        {
            return TryRegexMatch(GenericPattern, input, out code)
                || TryRegexMatch(SeparatedPattern, input, out code);
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
