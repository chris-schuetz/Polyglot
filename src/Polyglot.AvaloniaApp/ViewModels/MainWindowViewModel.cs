// Copyright (c) 2025 Christopher Schuetz
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;
using Avalonia.Styling;
using Microsoft.Extensions.Options;
using Preferences.Avalonia.ViewModels;
using Preferences.Common;
using Polyglot.Common;
using Preferences.Common.Services;
using ReactiveUI;

namespace Polyglot.AvaloniaApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _greeting = "Welcome to Avalonia!";
    private PreferencesOptions _preferencesOptions;

    public MainWindowViewModel(IOptions<PreferencesOptions> hotKeyOptions, ILocalizationService localizationService)
    {
        _preferencesOptions = hotKeyOptions.Value;
        OpenPreferencesDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            PreferencesOptions =
                await ShowPreferencesDialog.Handle(new PreferencesViewModel(PreferencesOptions, localizationService));
            AppSettings.Update(PreferencesOptions, PreferencesOptions.Preferences);
        });
        OpenHotKeysDialog = ReactiveCommand.CreateFromTask(async () =>
        {
            await ShowHotKeysDialog.Handle(new SectionViewModel(
                PreferencesOptions.Sections.First(s => s.Name == "Preferences.HotKeys"),
                localizationService));
        });
        CloseOverlay = ReactiveCommand.CreateFromTask(async () => await HideOverlay.Handle(Unit.Default));
        ExecuteExitApplication = ReactiveCommand.CreateFromTask(async () => await ExitApplication.Handle(Unit.Default));
        ApplyTheme();
    }

    public ICommand OpenPreferencesDialog { get; }

    public KeyGesture? OpenPreferencesGesture
    {
        get
        {
            return PreferencesOptions.Sections.FirstOrDefault(s => s.Name == "Preferences.HotKeys")?.Entries
                .FirstOrDefault(hk => hk.Name == "Preferences.HotKeys.OpenPreferences") is { } open
                ? KeyGesture.Parse(open.Value)
                : null;
        }
    }

    public IInteraction<PreferencesViewModel, PreferencesOptions> ShowPreferencesDialog { get; } =
        new Interaction<PreferencesViewModel, PreferencesOptions>();


    public ICommand OpenHotKeysDialog { get; }

    public KeyGesture? ShowHotKeysGesture
    {
        get
        {
            return PreferencesOptions.Sections.FirstOrDefault(s => s.Name == "Preferences.HotKeys")?.Entries
                .FirstOrDefault(hk => hk.Name == "Preferences.HotKeys.ShowHotKeys") is { } open
                ? KeyGesture.Parse(open.Value)
                : null;
        }
    }

    public IInteraction<SectionViewModel, Unit> ShowHotKeysDialog { get; } =
        new Interaction<SectionViewModel, Unit>();


    public string Greeting
    {
        get => _greeting;
        set
        {
            _greeting = value;
            this.RaisePropertyChanged();
        }
    }

    public PreferencesOptions PreferencesOptions
    {
        get => _preferencesOptions;
        set
        {
            _preferencesOptions = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(OpenPreferencesGesture));
            ApplyTheme();
        }
    }

    public IInteraction<Unit, Unit> HideOverlay { get; } = new Interaction<Unit, Unit>();

    public ICommand CloseOverlay { get; }

    public IInteraction<Unit, Unit> ExitApplication { get; } = new Interaction<Unit, Unit>();

    public ICommand ExecuteExitApplication { get; }

    public KeyGesture? ExitApplicationGesture
    {
        get
        {
            return PreferencesOptions.Sections.FirstOrDefault(s => s.Name == "Preferences.HotKeys")?.Entries
                .FirstOrDefault(hk => hk.Name == "Preferences.HotKeys.Exit") is { } open
                ? KeyGesture.Parse(open.Value)
                : null;
        }
    }

    private void ApplyTheme()
    {
        if (Application.Current == null) return;

        Application.Current.RequestedThemeVariant = PreferencesOptions.Sections
                .FirstOrDefault(s => s.Name == "Preferences.General")?.Entries
                .FirstOrDefault(e => e.Name == "Preferences.General.Theme")?.Value switch
            {
                "Light" => ThemeVariant.Light,
                "Dark" => ThemeVariant.Dark,
                _ => ThemeVariant.Default
            };
    }
}
