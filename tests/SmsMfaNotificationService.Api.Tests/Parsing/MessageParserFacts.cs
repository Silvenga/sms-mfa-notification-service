﻿using FluentAssertions;
using FluentAssertions.Execution;
using SmsMfaNotificationService.Api.Parsing;
using Xunit;

namespace SmsMfaNotificationService.Api.Tests.Parsing
{
    public class MessageParserFacts
    {
        [Theory]
        [InlineData("Your Discord security code is: 000 000", "000000")]
        [InlineData("ALERT! DO NOT share this code with anyone. We will never ask you for this code. Verification Code: \r\n\r\n000000 (expires in 3 minutes)",
            "000000")]
        [InlineData("Your Betterment auth code is 000000. If you did not make this request, please email support@betterment.com", "000000")]
        [InlineData(
            "Your T-Mobile ID verification code is 000000. For your security never share your verification code. T-Mobile will never contact you to ask for your code.",
            "000000")]
        [InlineData(
            "CIT Bank Messages. The one-time code is 00000. Please enter and submit it online. Msg&Data rates may apply/STOP if unexpected/HELP for support",
            "00000")]
        [InlineData("G-000000 is your Google verification code.", "000000")]
        [InlineData("PayPal: Your security code is: 000000. It expires in 10 minutes. Don't share this code with anyone.", "000000")]
        [InlineData("From: SoFi\nReminder: SoFi will never ask for your code on a call not initiated by you\nOne-Time Code: 000000", "000000")]
        [InlineData("00000 is your Shop Pay verification code", "00000")]
        [InlineData("If someone asks for the code, it's a scam. Your code is 0000.", "0000")]
        [InlineData("Example1 000-000", "000000")]
        [InlineData("Example2 000 - 000", "000000")]
        [InlineData("ADP: To finish signing in, enter 000000. Never share this code with anyone, even if they say they are from ADP. If you did not request it, reply X.", "000000")]
        [InlineData("Circle verification code: 000000", "000000")]
        public void Given_example_containing_sms_code_then_return_true(string input, string expected)
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

        [Theory]
        [InlineData("Example2 CA 11111")]
        [InlineData("at place, 6850 N Road.")]
        public void Given_message_containing_an_address_then_return_false(string input)
        {
            // Act
            var result = MessageParser.TryGetCode(input, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("We declined $000.00 with card ending 0000 at")]
        [InlineData("000-000-0000")]
        public void Given_message_containing_a_credit_card_then_return_false(string input)
        {
            // Act
            var result = MessageParser.TryGetCode(input, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("000-000-0000")]
        public void Given_message_containing_a_phone_number_then_return_false(string input)
        {
            // Act
            var result = MessageParser.TryGetCode(input, out _);

            // Assert
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("Thank you for ordering with us at Five Guys. We would love to hear your feedback! https://app.getwisely.com/webviews/surveys/01f927a9-cd5e-444a-bf5e-d1ad90929049?u=01f927a9-cd5e-444a-bf5e-d1ad90929049&m=312500&ut=143072000")]
        [InlineData("Thank you for ordering with us at Five Guys. We would love to hear your feedback! http://app.getwisely.com/webviews/surveys/01f927a9-cd5e-444a-bf5e-d1ad90929049?u=01f927a9-cd5e-444a-bf5e-d1ad90929049&m=312500&ut=143072000")]
        public void Given_message_containing_a_url_then_return_false(string input)
        {
            // Act
            var result = MessageParser.TryGetCode(input, out _);

            // Assert
            result.Should().BeFalse();
        }
    }
}
