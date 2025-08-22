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

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Preferences.Common;
using Preferences.Common.Services;
using Polyglot.SpectreApp.Rendering;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Polyglot.SpectreApp.Pages;

public class HotKeyPage : IPage
{
    private readonly ILocalizationService _localizationService;
    private readonly ILogger<HotKeyPage> _logger;
    private readonly IOptionsMonitor<PreferencesOptions> _options;

    public HotKeyPage(
        ILogger<HotKeyPage> logger,
        IOptionsMonitor<PreferencesOptions> options,
        ILocalizationService localizationService)
    {
        _logger = logger;
        _options = options;
        _localizationService = localizationService;
    }

    public IRenderable Draw(ScreenLayout screenLayout)
    {
        var rows = new List<IRenderable>
            { screenLayout.GetHeader(_localizationService.GetLocalizedString("Preferences")) };

        var hotKeys = _options.CurrentValue.Sections.First(s => s.Name == "Preferences.HotKeys");
        rows.Add(screenLayout.GetSubHeader(_localizationService.GetLocalizedString(hotKeys.Name)));
        foreach (var entry in hotKeys.Entries)
            rows.Add(screenLayout.GetContent($"{_localizationService.GetLocalizedString(entry.Name)}: {entry.Value}"));

        return new Rows(rows);
    }
}
