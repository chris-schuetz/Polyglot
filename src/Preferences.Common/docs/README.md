# Preferences.Common

Common building blocks for the Preferences ecosystem, shared by UI packages (Avalonia, Spectre, etc.).

Includes:

- Core models: PreferencesOptions, PreferencesSection, PreferencesEntry
- Localization contract: ILocalizationService
- Messaging primitives for hotkeys and actions
- Console hotkey processing service (HotKeyService)

## Install

NuGet package: `Preferences.Common`

```powershell
Install-Package Preferences.Common
```

## Core concepts

- PreferencesOptions: top-level container for all sections. Use the constant PreferencesOptions.Preferences as the
  configuration root name if you bind from IConfiguration.
- PreferencesSection: a named group with an Order and a list of Entries.
- PreferencesEntry: a single setting with Name, Value, and optional Options for pick lists.

Example:

```csharp
var options = new PreferencesOptions
{
    Sections =
    [
        new PreferencesSection
        {
            Name = "Preferences.General",
            Order = 0,
            Entries = [ new PreferencesEntry { Name = "Preferences.General.Language", Value = "en" } ]
        }
    ]
};
```

## Localization

UI layers can localize titles via ILocalizationService:

```csharp
public interface ILocalizationService
{
    string GetLocalizedString(string resourceKey);
    event EventHandler LocaleChanged;
}
```

Return the key if no translation exists to keep the UI resilient.

## Messaging and Hotkeys

The package defines simple message records and a HotKeyService that listens to KeyInputMessage and publishes
higher-order commands:

- KeyInputMessage(ConsoleKeyInfo key)
- OpenPreferencesCommand
- ShowHotKeysCommand
- ShutdownCommand

HotKeyService maps key combinations (e.g., Ctrl+Q) to commands based on the Preferences.HotKeys section in
PreferencesOptions.

```csharp
services.AddSingleton<HotKeyService>(); // also registered as IConsumer<KeyInputMessage>
```

## Configuration binding

Use Microsoft.Extensions.Options to bind preferences from configuration:

```csharp
services.Configure<PreferencesOptions>(cfg =>
{
    configuration.GetSection(PreferencesOptions.Preferences).Bind(cfg);
});
```

This allows both UI and services to consume the same options via IOptionsMonitor<PreferencesOptions>.
