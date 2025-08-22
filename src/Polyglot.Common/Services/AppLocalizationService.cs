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

using System.Globalization;
using System.Resources;
using Preferences.Common.Services;

namespace Polyglot.Common.Services;

public sealed class AppLocalizationService : ILocalizationService
{
    private readonly ResourceManager _resourceManager =
        new("Polyglot.Common.Resources.General",
            typeof(AppLocalizationService).Assembly);

    private CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

    public event EventHandler? LocaleChanged;

    public string GetLocalizedString(string resourceKey)
    {
        try
        {
            var value = _resourceManager.GetString(resourceKey, _currentCulture);
            return !string.IsNullOrEmpty(value) ? value : resourceKey;
        }
        catch
        {
            return resourceKey;
        }
    }

    public void SetLocale(string cultureName)
    {
        try
        {
            var newCulture = new CultureInfo(cultureName);
            if (!_currentCulture.Equals(newCulture))
            {
                _currentCulture = newCulture;
                // Raise the event to notify subscribers
                LocaleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
        catch (Exception)
        {
            // Handle invalid culture
        }
    }

    public void SetLocale(CultureInfo cultureInfo)
    {
        if (!_currentCulture.Equals(cultureInfo))
        {
            _currentCulture = cultureInfo;
            // Raise the event to notify subscribers
            LocaleChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
