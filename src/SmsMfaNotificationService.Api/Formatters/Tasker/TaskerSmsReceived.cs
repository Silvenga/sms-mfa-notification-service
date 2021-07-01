namespace SmsMfaNotificationService.Api.Formatters.Tasker
{
    public record TaskerSmsReceived(string Type, string PhoneNumber, string Message);
}
