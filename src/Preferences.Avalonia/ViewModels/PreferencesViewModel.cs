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

using Preferences.Common;
using Preferences.Common.Services;
using ReactiveUI;

namespace Preferences.Avalonia.ViewModels;

/// <summary>
///     Represents a view model for managing preferences UI, coordinating sections and their entries.
/// </summary>
/// <remarks>
///     This class serves as the main view model for the preferences window, maintaining a collection of
///     <see cref="SectionViewModel" /> instances organized by their order property. It provides reactive
///     property change notifications through ReactiveUI for UI binding.
///     The view model is initialized with a <see cref="PreferencesOptions" /> that contains the underlying
///     preference data structure, and an optional <see cref="ILocalizationService" /> for localizing
///     section and entry titles.
///     It tracks the currently selected section to facilitate navigation between different preference
///     categories in the UI.
/// </remarks>
public class PreferencesViewModel : ReactiveObject
{
    private SectionViewModel? _selectedSection;

    public PreferencesViewModel(PreferencesOptions preferencesOptions, ILocalizationService? localizationService = null)
    {
        Options = preferencesOptions;

        // Convert model sections to view model sections
        Sections = new List<SectionViewModel>();
        foreach (var section in Options.Sections) Sections.Add(new SectionViewModel(section, localizationService));

        Sections = Sections.OrderBy(s => s.Order).ToList();
        SelectedSection = Sections.FirstOrDefault();
    }

    public List<SectionViewModel> Sections { get; }

    public SectionViewModel? SelectedSection
    {
        get => _selectedSection;
        set => this.RaiseAndSetIfChanged(ref _selectedSection, value);
    }

    public PreferencesOptions Options { get; }
}
