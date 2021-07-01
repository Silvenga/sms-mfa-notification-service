namespace SmsMfaNotificationService.Api.Hubs
{
    public record MfaCodeReceived(string SourcePhoneNumber, string Code, string FullMessage);
}
