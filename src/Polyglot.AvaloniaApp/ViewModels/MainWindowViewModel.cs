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
using Polyglot.Common;
using Polyglot.Common.Models;
using Polyglot.Common.Services;
using Preferences.Avalonia.ViewModels;
using Preferences.Common;
using Preferences.Common.Services;
using ReactiveUI;

namespace Polyglot.AvaloniaApp.ViewModels;

public sealed class MainWindowViewModel : ViewModelBase
{
    private readonly Workspace _workspaceModel = WorkspaceHelpers.CreateFakeWorkspace();
    private PreferencesOptions _preferencesOptions;

    // Parameterless constructor for Avalonia Designer
    // Creates lightweight default options and a basic localization service
    public MainWindowViewModel() : this(
        Options.Create(CreateDesignTimePreferences()),
        new AppLocalizationService())
    {
    }

    private static PreferencesOptions CreateDesignTimePreferences()
    {
        return new PreferencesOptions
        {
            Sections =
            [
                new PreferencesSection
                {
                    Name = "Preferences.General",
                    Order = 0,
                    Entries =
                    [
                        new PreferencesEntry
                        {
                            Name = "Preferences.General.Theme",
                            Value = "Dark",
                            Options = ["Default", "Light", "Dark"]
                        }
                    ]
                },
                new PreferencesSection
                {
                    Name = "Preferences.HotKeys",
                    Order = 1,
                    Entries =
                    [
                        new PreferencesEntry { Name = "Preferences.HotKeys.OpenPreferences", Value = "Ctrl+P" },
                        new PreferencesEntry { Name = "Preferences.HotKeys.ShowHotKeys", Value = "Ctrl+H" },
                        new PreferencesEntry { Name = "Preferences.HotKeys.Exit", Value = "Ctrl+Q" },
                        new PreferencesEntry { Name = "Preferences.HotKeys.MotionLeft", Value = "Left" },
                        new PreferencesEntry { Name = "Preferences.HotKeys.MotionRight", Value = "Right" },
                        new PreferencesEntry { Name = "Preferences.HotKeys.MotionUp", Value = "Up" },
                        new PreferencesEntry { Name = "Preferences.HotKeys.MotionDown", Value = "Down" },
                        new PreferencesEntry { Name = "Preferences.HotKeys.MotionSelect", Value = "Enter" }
                    ]
                }
            ]
        };
    }

    public MainWindowViewModel(IOptions<PreferencesOptions> hotKeyOptions, ILocalizationService localizationService)
    {
        _preferencesOptions = hotKeyOptions.Value;

        // Initialize Workspace model and its ViewModel
        Workspace = new WorkspaceViewModel(_workspaceModel);

        // Commands
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

        // Motion commands
        MotionLeftCommand = ReactiveCommand.Create(() => Workspace.MoveLeft());
        MotionRightCommand = ReactiveCommand.Create(() => Workspace.MoveRight());
        MotionUpCommand = ReactiveCommand.Create(() => Workspace.MoveUp());
        MotionDownCommand = ReactiveCommand.Create(() => Workspace.MoveDown());
        MotionSelectCommand = ReactiveCommand.Create(() => Workspace.Select());

        ApplyTheme();
    }

    public ICommand OpenPreferencesDialog { get; }

    // Motion commands
    public ICommand MotionLeftCommand { get; }
    public ICommand MotionRightCommand { get; }
    public ICommand MotionUpCommand { get; }
    public ICommand MotionDownCommand { get; }
    public ICommand MotionSelectCommand { get; }

    // Motion gestures from preferences (normalized for Avalonia)
    public KeyGesture? MotionLeftGesture => GetGesture("Preferences.HotKeys.MotionLeft");
    public KeyGesture? MotionRightGesture => GetGesture("Preferences.HotKeys.MotionRight");
    public KeyGesture? MotionUpGesture => GetGesture("Preferences.HotKeys.MotionUp");
    public KeyGesture? MotionDownGesture => GetGesture("Preferences.HotKeys.MotionDown");
    public KeyGesture? MotionSelectGesture => GetGesture("Preferences.HotKeys.MotionSelect");

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

    public PreferencesOptions PreferencesOptions
    {
        get => _preferencesOptions;
        set
        {
            _preferencesOptions = value;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(OpenPreferencesGesture));
            this.RaisePropertyChanged(nameof(ShowHotKeysGesture));
            this.RaisePropertyChanged(nameof(ExitApplicationGesture));
            this.RaisePropertyChanged(nameof(MotionLeftGesture));
            this.RaisePropertyChanged(nameof(MotionRightGesture));
            this.RaisePropertyChanged(nameof(MotionUpGesture));
            this.RaisePropertyChanged(nameof(MotionDownGesture));
            this.RaisePropertyChanged(nameof(MotionSelectGesture));
            ApplyTheme();
        }
    }

    public IInteraction<Unit, Unit> HideOverlay { get; } = new Interaction<Unit, Unit>();

    public WorkspaceViewModel Workspace { get; }


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

    private KeyGesture? GetGesture(string name)
    {
        var value = PreferencesOptions.Sections.FirstOrDefault(s => s.Name == "Preferences.HotKeys")?.Entries
            .FirstOrDefault(hk => hk.Name == name)?.Value;
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var normalized = NormalizeGestureValue(value);
        try
        {
            return KeyGesture.Parse(normalized);
        }
        catch
        {
            return null;
        }
    }

    private static string NormalizeGestureValue(string value)
    {
        return value switch
        {
            "LeftArrow" => "Left",
            "RightArrow" => "Right",
            "UpArrow" => "Up",
            "DownArrow" => "Down",
            "Spacebar" => "Space",
            _ => value
        };
    }

    private void ApplyTheme()
    {
        if (Application.Current == null)
        {
            return;
        }

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
