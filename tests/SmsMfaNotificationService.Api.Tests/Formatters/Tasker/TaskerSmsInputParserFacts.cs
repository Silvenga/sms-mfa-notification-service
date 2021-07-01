using FluentAssertions;
using SmsMfaNotificationService.Api.Formatters.Tasker;
using Xunit;

namespace SmsMfaNotificationService.Api.Tests.Formatters.Tasker
{
    public class TaskerSmsInputParserFacts
    {
        [Fact]
        public void Can_parse_type()
        {
            const string input = "SMS,+15555555555,Test,T-Mobile";

            // Act
            var result = TaskerSmsInputParser.Parse(input);

            // Assert
            result.Type.Should().Be("SMS");
        }

        [Fact]
        public void Can_parse_phone_number()
        {
            const string input = "SMS,+15555555555,Test,T-Mobile";

            // Act
            var result = TaskerSmsInputParser.Parse(input);

            // Assert
            result.PhoneNumber.Should().Be("+15555555555");
        }

        [Fact]
        public void Can_parse_message()
        {
            const string input = "SMS,+15555555555,Test,T-Mobile";

            // Act
            var result = TaskerSmsInputParser.Parse(input);

            // Assert
            result.Message.Should().Be("Test");
        }

        [Fact]
        public void Can_parse_message_with_commas()
        {
            const string input = "SMS,+15555555555,Test,test,T-Mobile";

            // Act
            var result = TaskerSmsInputParser.Parse(input);

            // Assert
            result.Message.Should().Be("Test,test");
        }
    }
}
