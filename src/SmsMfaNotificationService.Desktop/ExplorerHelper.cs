using System;
using System.Diagnostics;
using System.IO;
using NLog;

namespace SmsMfaNotificationService.Desktop
{
    public static class ExplorerHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static bool ExploreAppDataFile(string fileName)
        {
            var filePath = Path.Combine(Windows.Storage.ApplicationData.Current.LocalCacheFolder.Path,
                "Roaming",
                "sms-mfa-notification-service",
                fileName
            );
            if (!File.Exists(filePath))
            {
                return false;
            }

            try
            {
                filePath = Path.GetFullPath(filePath);
                Process.Start("explorer.exe", $"/select,\"{filePath}\"");
            }
            catch (Exception e)
            {
                Logger.Error(e, $"Failed to open file '{filePath}'.");
            }

            return true;
        }
    }
}
