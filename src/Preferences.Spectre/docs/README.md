# Preferences.Spectre

Spectre.Console-friendly pieces for working with Preferences. This package complements Preferences.Common and is
intended for console/TUI applications built with Spectre.Console.

Note: The sample app in this repository (Polyglot.SpectreApp) demonstrates how to render preferences and handle hotkeys
using the shared models and services.

## Install

NuGet package: `Preferences.Spectre`

```powershell
Install-Package Preferences.Spectre
```

## What it’s for

- Share the same preferences model (PreferencesOptions) between console and GUI apps
- Use HotKeyService from Preferences.Common to translate key presses to application commands
- Render preferences in Spectre.Console using your own layout with the sample as a reference

## Quickstart

1) Configure options and localization (same as other targets):

```csharp
services.Configure<PreferencesOptions>(cfg =>
{
    configuration.GetSection(PreferencesOptions.Preferences).Bind(cfg);
});
services.AddSingleton<ILocalizationService, AppLocalizationService>();
services.AddSingleton<HotKeyService>(); // IConsumer<KeyInputMessage>
```

2) In your Spectre.Console loop, publish KeyInputMessage when a key is pressed and let HotKeyService map it to commands
   defined by the Preferences.HotKeys section:

```csharp
var keyInfo = Console.ReadKey(intercept: true);
await bus.Publish(new KeyInputMessage(keyInfo));
```

3) Render preferences using Spectre.Console components. See Polyglot.SpectreApp/Pages/PreferencesPage.cs for an example
   building Rows with headers and entries.

## Sample

- Polyglot.SpectreApp shows:
    - How to wire SlimMessageBus, PreferencesOptions, and HotKeyService
    - A screen layout that renders preferences and a dedicated HotKey page
