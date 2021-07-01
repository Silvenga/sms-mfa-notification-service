using System;

namespace SmsMfaNotificationService.Api.Formatters.Tasker
{
    public class TaskerSmsInputParser
    {
        public static TaskerSmsReceived Parse(string str)
        {
            if (str == null)
            {
                throw new ArgumentNullException(nameof(str));
            }

            var parts = str.Split(",");
            if (parts.Length < 4)
            {
                throw new ArgumentException("Failed to parse input, invalid number of parts.");
            }

            var type = parts[0];
            var phoneNumber = parts[1];
            var message = string.Join(",", parts[2..^1]);

            return new TaskerSmsReceived(type, phoneNumber, message);
        }
    }
}
