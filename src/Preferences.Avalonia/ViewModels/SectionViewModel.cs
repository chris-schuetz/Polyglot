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

using System.Collections.ObjectModel;
using Preferences.Common;
using Preferences.Common.Services;
using ReactiveUI;

namespace Preferences.Avalonia.ViewModels;

/// <summary>
///     Represents a view model for a preferences section that provides reactive UI binding
///     and localization support for preference entries.
/// </summary>
/// <remarks>
///     This view model wraps a <see cref="PreferencesSection" /> model and exposes its properties
///     in a way that's suitable for UI binding in Avalonia applications. It implements
///     <see cref="IDisposable" /> to properly clean up event subscriptions and follows the
///     ReactiveUI pattern for property change notifications.
///     The view model automatically handles localization updates by subscribing to locale
///     change events and refreshing the Title property when the locale changes.
/// </remarks>
public class SectionViewModel : ReactiveObject, IDisposable
{
    private readonly ILocalizationService? _localizationService;
    private readonly PreferencesSection _model;
    private bool _isDisposed;

    public SectionViewModel(PreferencesSection model, ILocalizationService? localizationService = null)
    {
        _model = model;
        _localizationService = localizationService;
        Entries = new ObservableCollection<EntryViewModel>(model.Entries.Select(e =>
            new EntryViewModel(e, localizationService)));

        if (_localizationService != null) _localizationService.LocaleChanged += OnLocaleChanged;
    }

    public string Title => _localizationService != null
        ? _localizationService.GetLocalizedString(_model.Name)
        : _model.Name;

    public int Order => _model.Order;

    public ObservableCollection<EntryViewModel> Entries { get; }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~SectionViewModel()
    {
        Dispose(false);
    }

    private void OnLocaleChanged(object? sender, EventArgs e)
    {
        this.RaisePropertyChanged(nameof(Title));
    }

    private void Dispose(bool disposing)
    {
        if (_isDisposed) return;

        if (disposing)
            if (_localizationService != null)
                _localizationService.LocaleChanged -= OnLocaleChanged;

        _isDisposed = true;
    }
}
