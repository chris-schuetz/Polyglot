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

using System.Reactive;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.ReactiveUI;
using Polyglot.AvaloniaApp.ViewModels;
using Preferences.Avalonia.ViewModels;
using Preferences.Avalonia.Views;
using Preferences.Common;
using ReactiveUI;

namespace Polyglot.AvaloniaApp.Views;

public sealed partial class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
        this.WhenActivated(action =>
            action(ViewModel!.ShowPreferencesDialog.RegisterHandler(DoShowDialogAsync)));
        this.WhenActivated(action =>
            action(ViewModel!.ShowHotKeysDialog.RegisterHandler(DoShowHotKeysAsync)));
        this.WhenActivated(action =>
            action(ViewModel!.ExitApplication.RegisterHandler(DoExitApplicationAsync)));
        this.WhenActivated(action =>
            action(ViewModel!.HideOverlay.RegisterHandler(DoHideOverlayAsync)));
    }

    private Task DoExitApplicationAsync(IInteractionContext<Unit, Unit> interaction)
    {
        Close();
        interaction.SetOutput(Unit.Default);
        return Task.CompletedTask;
    }

    private Task DoShowHotKeysAsync(IInteractionContext<SectionViewModel, Unit> interaction)
    {
        var sectionEntriesView = new SectionView(interaction.Input, ViewModel!.CloseOverlay);
        var overlayContainer = this.FindControl<Grid>("OverlayContainer");

        if (overlayContainer != null)
        {
            overlayContainer.Children.Clear();
            overlayContainer.Children.Add(sectionEntriesView);
            overlayContainer.IsVisible = true;
        }

        interaction.SetOutput(Unit.Default);
        return Task.CompletedTask;
    }

    private Task DoHideOverlayAsync(IInteractionContext<Unit, Unit> interaction)
    {
        var overlayContainer = this.FindControl<Grid>("OverlayContainer");

        if (overlayContainer != null)
        {
            overlayContainer.Children.Clear();
            overlayContainer.IsVisible = false;
        }

        interaction.SetOutput(Unit.Default);
        return Task.CompletedTask;
    }

    private async Task DoShowDialogAsync(IInteractionContext<PreferencesViewModel, PreferencesOptions> interaction)
    {
        var dialog = new PreferencesView(interaction.Input);
        _ = await dialog.ShowDialog<PreferencesOptions>(this);
        interaction.SetOutput(interaction.Input.Options);
    }
}
