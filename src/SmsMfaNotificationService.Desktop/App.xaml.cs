using System;
using System.Windows;
using Hardcodet.Wpf.TaskbarNotification;
using Microsoft.Toolkit.Uwp.Notifications;
using NLog;
using SmsMfaNotificationService.Desktop.Models;
using TextCopy;

namespace SmsMfaNotificationService.Desktop
{
    public partial class App : Application
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly NotificationWatcher _watcher;
        private readonly TaskbarIcon _notifyIcon;

        public App()
        {
            Logger.Info("Application starting.");

            InitializeComponent();

            ToastNotificationManagerCompat.OnActivated += ToastNotificationManagerCompatOnOnActivated;

            var settings = SettingsHelper.CreateOrRead();

            _watcher = new NotificationWatcher(
                settings.ClientId ?? "example",
                settings.NotificationHost ?? "http://localhost:5000"
            );
            _watcher.MfaCodeReceived += OnMfaCodeReceived;

            _notifyIcon = (TaskbarIcon) FindResource("NotifyIcon")!;

            Logger.Info("Application is ready.");
        }

        private async void OnStartup(object sender, StartupEventArgs e)
        {
            Logger.Info("Startup complete, connecting to hub.");
            await _watcher.Connect();
        }

        private void OnMfaCodeReceived(object? sender, MfaCodeReceived payload)
        {
            Logger.Debug("Recieved mfa code event, sending notification to user.");

            Dispatcher.Invoke(() =>
            {
                new ToastContentBuilder()
                    .AddText("SMS MFA Code Detected", hintMaxLines: 1)
                    .AddText(payload.FullMessage)
                    .AddButton(new ToastButton()
                               .AddArgument("action", "copy")
                               .AddArgument("code", payload.Code)
                               .SetContent($"Copy '{payload.Code}'"))
                    .AddButton(new ToastButtonDismiss())
                    .SetToastScenario(ToastScenario.Reminder)
                    .AddAttributionText($"From {payload.SourcePhoneNumber}")
                    .Show(toast => { toast.ExpirationTime = DateTime.Now.AddMinutes(2); });
            });
        }

        private void ToastNotificationManagerCompatOnOnActivated(ToastNotificationActivatedEventArgsCompat toastArgs)
        {
            Logger.Debug("Got action from user via toast, handling.");

            var args = ToastArguments.Parse(toastArgs.Argument);

            if (args.TryGetValue("action", out var action)
                && action == "copy"
                && args.TryGetValue("code", out var code))
            {
                Logger.Debug("Got copy action, modifying clipboard with code.");
                ClipboardService.SetText(code);
            }
            else
            {
                Logger.Error("Unknown action was recieved, something went wrong.");
            }
        }

        private async void OnExit(object sender, ExitEventArgs e)
        {
            Logger.Info("Application is going down, gracefully disposing of resouces.");
            _notifyIcon.Dispose();
            await _watcher.Shutdown();
            base.OnExit(e);
        }

        private void ExitOnClick(object sender, RoutedEventArgs e)
        {
            Current.Shutdown();
        }

        private void OpenSettingsOnClick(object sender, RoutedEventArgs e)
        {
            Logger.Info("Got request for settings, invoking explorer.");
            ExplorerHelper.ExploreAppDataFile("settings.yaml");
        }

        private void OpenLogsOnClick(object sender, RoutedEventArgs e)
        {
            Logger.Info("Got request for logs, invoking explorer.");
            ExplorerHelper.ExploreAppDataFile("app.log");
        }
    }
}
