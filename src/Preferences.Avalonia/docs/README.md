# Preferences.Avalonia

A lightweight, flexible preferences UI framework for Avalonia applications.

- MVVM-friendly view models for sections and entries
- Ready-made PreferencesView window for browsing and editing settings
- HotKeyMenu control that auto-registers MenuItem shortcuts as window KeyBindings
- Optional ILocalizationService integration for multi-language UI

## Install

NuGet package: `Preferences.Avalonia`

```powershell
Install-Package Preferences.Avalonia
```

## Get started

1. Define your preferences model (from Preferences.Common):

```csharp
using Preferences.Common;

var options = new PreferencesOptions
{
    Sections =
    [
        new PreferencesSection
        {
            Name = "Preferences.Appearance",
            Order = 0,
            Entries =
            [
                new PreferencesEntry { Name = "Preferences.Appearance.Theme", Value = "Light", Options = ["Light", "Dark"] },
                new PreferencesEntry { Name = "Preferences.Appearance.FontSize", Value = "14" }
            ]
        },
        new PreferencesSection
        {
            Name = "Preferences.HotKeys",
            Order = 1,
            Entries =
            [
                new PreferencesEntry { Name = "Preferences.HotKeys.OpenPreferences", Value = "Ctrl+Comma" },
                new PreferencesEntry { Name = "Preferences.HotKeys.ShowHotKeys", Value = "Ctrl+Shift+K" },
                new PreferencesEntry { Name = "Preferences.HotKeys.Exit", Value = "Ctrl+Q" }
            ]
        }
    ]
};
```

2. Show the preferences dialog using the provided view and view model:

```csharp
using Preferences.Avalonia.ViewModels;
using Preferences.Avalonia.Views;
using Preferences.Common.Services;

var vm = new PreferencesViewModel(options, localizationService: myLocalizationService /* or null */);
var dialog = new PreferencesView { DataContext = vm };
var result = await dialog.ShowDialog<PreferencesOptions>(ownerWindow);
```

- The dialog binds to PreferencesViewModel.Sections and SelectedSection.
- Each entry is displayed with a ComboBox if Options are provided; otherwise, a TextBox is used.

## HotKeyMenu control

Use the HotKeyMenu control to automatically wire MenuItem InputGesture to the parent Window KeyBindings:

```xml
<controls:HotKeyMenu xmlns:controls="clr-namespace:Preferences.Avalonia.Controls;assembly=Preferences.Avalonia">
  <MenuItem Header="_File">
    <MenuItem Header="Preferences" Command="{Binding OpenPreferencesCommand}"
              InputGesture="Ctrl+Comma" />
    <MenuItem Header="Exit" Command="{Binding ExitCommand}"
              InputGesture="Ctrl+Q" />
  </MenuItem>
</controls:HotKeyMenu>
```

Any change to a MenuItem.InputGesture is detected and the corresponding KeyBinding on the Window is updated
automatically.

## Localization

Implement Preferences.Common.Services.ILocalizationService to localize section and entry names. The view models call
GetLocalizedString(name) and listen to LocaleChanged to update the UI.

```csharp
public class AppLocalizationService : ILocalizationService
{
    public string GetLocalizedString(string key) => _resources.TryGetValue(key, out var v) ? v : key;
    public event EventHandler? LocaleChanged;
}
```

## Samples

See the Polyglot.AvaloniaApp project in this repository for a runnable example, including DI setup, options binding, and
showing the dialog via ReactiveUI interactions.
