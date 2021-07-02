namespace SmsMfaNotificationService.Desktop.Models
{
    public record MfaCodeReceived(string SourcePhoneNumber, string Code, string FullMessage);
}
