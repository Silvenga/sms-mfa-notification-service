using System;
using System.IO;
using System.Reflection;
using NLog;
using YamlDotNet.Serialization;

namespace SmsMfaNotificationService.Desktop
{
    public class Settings
    {
        public string? ClientId { get; set; }
        public string? NotificationHost { get; set; }
    };

    public class SettingsHelper
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public static Settings CreateOrRead()
        {
            var settingsPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "sms-mfa-notification-service",
                "settings.yaml"
            );
            var settingsFileInfo = new FileInfo(settingsPath);

            if (!settingsFileInfo.Exists)
            {
                Logger.Debug("Settings does not exist, creating from defaults.");
                var directory = settingsFileInfo.Directory?.FullName;
                if (directory != null
                    && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var defaultContent = GetDefaultSettingsContent();
                File.AppendAllText(settingsFileInfo.FullName, defaultContent);
            }

            var settingsYaml = File.ReadAllText(settingsFileInfo.FullName);

            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<Settings>(settingsYaml);
        }

        private static string GetDefaultSettingsContent()
        {
            var assembly = Assembly.GetExecutingAssembly();

            using var stream = assembly.GetManifestResourceStream("SmsMfaNotificationService.Desktop.DefaultSettings.yaml");
            using var reader = new StreamReader(stream!);
            return reader.ReadToEnd();
        }
    }
}
