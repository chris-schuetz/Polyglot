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

using Avalonia.Headless;
using Avalonia.Headless.XUnit;
using Avalonia.Input;
using Microsoft.Extensions.Options;
using Polyglot.AvaloniaApp.ViewModels;
using Polyglot.AvaloniaApp.Views;
using Polyglot.Common.Services;
using Preferences.Avalonia.Views;
using Preferences.Common;
using Shouldly;

namespace Polyglot.AvaloniaSpecs;

public class ShowHotKeysDialogSpec
{
    private static IOptions<PreferencesOptions> CreateOptions(PhysicalKey key, RawInputModifiers mods) =>
        Options.Create(new PreferencesOptions
        {
            Sections =
            [
                new PreferencesSection
                {
                    Name = "Preferences.HotKeys",
                    Order = 1,
                    Entries =
                    [
                        new PreferencesEntry { Name = "Preferences.HotKeys.ShowHotKeys", Value = key.GetHotKey(mods) },
                        new PreferencesEntry { Name = "Preferences.HotKeys.MotionBack", Value = "Escape" },
                    ]
                }
            ]
        });
    
    [AvaloniaTheory]
    [InlineData(PhysicalKey.F1, RawInputModifiers.None)]
    [InlineData(PhysicalKey.H, RawInputModifiers.Control | RawInputModifiers.Shift)]
    public void UsingHotKey_ShowsDialog(PhysicalKey physicalKey, RawInputModifiers modifiers)
    {
        var viewModel = new MainWindowViewModel(
            CreateOptions(physicalKey, modifiers),
            new AppLocalizationService());
        var window = new MainWindow
        {
            ViewModel = viewModel,
            DataContext = viewModel,
        };

        window.Show();
        window.OverlayContainer.Children.ShouldBeEmpty();

        window.KeyPressQwerty(physicalKey, modifiers);

        window.OverlayContainer.Children.ShouldNotBeEmpty();
        window.OverlayContainer.Children.First().ShouldBeOfType<SectionView>();

        window.KeyPressQwerty(PhysicalKey.Escape, RawInputModifiers.None);

        window.OverlayContainer.Children.ShouldBeEmpty();
    }
}
