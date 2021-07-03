# sms-mfa-notification-service

![icon](./assets/icon.png)

Desktop notifications for insecure SMS MFA that companies force you to use.

## Signing Builds

Builds are un-signed coming from CI since they require trusted sigatures.

```
$msix = ".\SmsMfaNotificationService.Desktop.Packaging_*_x86.msix"
& $signtool sign /fd SHA256 /a /f "code-signing.pfx" /p <password> /debug $msix
& $signtool sign /tr http://timestamp.entrust.net/TSS/RFC3161sha2TS /td sha256 /fd sha256 $msix
```

(You need both signtool and makeappx inside the same folder)

```
cp (Get-AppxPackage |
 Where Name -Contains "Microsoft.MsixPackagingTool").InstallLocation + "/sdk" ./
```

## Attributions

Icon is _sms security_ by _popcornarts_ from the Noun Project.
