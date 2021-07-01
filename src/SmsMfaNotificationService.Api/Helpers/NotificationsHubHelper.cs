using System;
using System.Security.Cryptography;
using System.Text;

namespace SmsMfaNotificationService.Api.Helpers
{
    public class NotificationsHubHelper
    {
        public static string ClientIdToGroupName(string clientId)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(Encoding.UTF8.GetBytes(clientId));
            var groupName = Convert.ToBase64String(hash);
            return groupName;
        }
    }
}
