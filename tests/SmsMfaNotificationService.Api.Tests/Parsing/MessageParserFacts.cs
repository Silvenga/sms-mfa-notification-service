using FluentAssertions;
using FluentAssertions.Execution;
using SmsMfaNotificationService.Api.Parsing;
using Xunit;

namespace SmsMfaNotificationService.Api.Tests.Parsing
{
    public class MessageParserFacts
    {
        [Theory]
        [InlineData("Your Discord security code is: 000 000", "000000")]
        [InlineData("ALERT! DO NOT share this code with anyone. We will never ask you for this code. Verification Code: \r\n\r\n000000 (expires in 3 minutes)", "000000")]
        [InlineData("Your Betterment auth code is 000000. If you did not make this request, please email support@betterment.com", "000000")]
        [InlineData("Your T-Mobile ID verification code is 000000. For your security never share your verification code. T-Mobile will never contact you to ask for your code.", "000000")]
        [InlineData("CIT Bank Messages. The one-time code is 00000. Please enter and submit it online. Msg&Data rates may apply/STOP if unexpected/HELP for support", "00000")]
        [InlineData("G-000000 is your Google verification code.", "000000")]
        [InlineData("PayPal: Your security code is: 000000. It expires in 10 minutes. Don't share this code with anyone.", "000000")]
        [InlineData("From: SoFi\nReminder: SoFi will never ask for your code on a call not initiated by you\nOne-Time Code: 000000", "000000")]
        [InlineData("Example1 000-000", "000000")]
        [InlineData("Example2 000 - 000", "000000")]
        public void Given_examples_parse_mfa_code(string input, string expected)
        {
            // Act
            var result = MessageParser.TryGetCode(input, out var code);

            // Assert
            using (new AssertionScope())
            {
                result.Should().BeTrue();
                code.Should().Be(expected);
            }
        }
    }
}
