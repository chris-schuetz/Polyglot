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
///     Represents a view model for an individual preference entry, providing reactive UI binding
///     and localization support.
/// </summary>
/// <remarks>
///     This sealed class wraps an <see cref="PreferencesEntry" /> and exposes its properties in a UI-friendly way.
///     It supports localization of entry titles through an optional <see cref="ILocalizationService" />.
///     The view model reactively updates when the locale changes, and implements <see cref="IDisposable" />
///     to properly clean up event subscriptions when no longer needed.
///     Entry view models can represent various types of preferences, including those with predefined
///     options (dropdown selections) or free-form input values.
/// </remarks>
public sealed class EntryViewModel : ReactiveObject, IDisposable
{
    private readonly ILocalizationService? _localizationService;
    private readonly PreferencesEntry _model;
    private bool _isDisposed;

    public EntryViewModel(PreferencesEntry model, ILocalizationService? localizationService = null)
    {
        _model = model;
        _localizationService = localizationService;

        if (_localizationService != null) _localizationService.LocaleChanged += OnLocaleChanged;
    }

    public string Title =>
        _localizationService != null
            ? _localizationService.GetLocalizedString(_model.Name)
            : _model.Name;

    public string Value
    {
        get => _model.Value;
        set => _model.Value = value;
    }

    public List<string>? Options => _model.Options;

    public bool HasOptions => Options is { Count: > 0 };

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~EntryViewModel()
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
