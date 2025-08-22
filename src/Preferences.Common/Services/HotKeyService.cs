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
using Preferences.Common.Messages;
using SlimMessageBus;

namespace Preferences.Common.Services;

public class HotKeyService : IConsumer<KeyInputMessage>
{
    private readonly IMessageBus _bus;
    private readonly ILogger<HotKeyService> _logger;
    private readonly IOptionsMonitor<PreferencesOptions> _options;

    public HotKeyService(
        ILogger<HotKeyService> logger,
        IOptionsMonitor<PreferencesOptions> options,
        IMessageBus bus)
    {
        _logger = logger;
        _options = options;
        _bus = bus;
    }

    public async Task OnHandle(KeyInputMessage message, CancellationToken cancellationToken)
    {
        try
        {
            var hotKeys = _options.CurrentValue.Sections.First(section =>
                section.Name.Equals("Preferences.HotKeys", StringComparison.Ordinal));
            var hotKey = hotKeys.Entries.FirstOrDefault(e =>
                e.Value.Equals(CreateHotkeyString(message.KeyInfo), StringComparison.Ordinal));
            if (hotKey == null) return;

            await _bus.Publish(CreateHotKeyMessage(hotKey.Name), cancellationToken: cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error handling key input");
            await _bus.Publish(
                new StatusMessage($"Error: {ex.Message}", StatusMessageType.Error),
                cancellationToken: CancellationToken.None);
        }
    }

    private Message CreateHotKeyMessage(string hotKeyName)
    {
        // TODO inject factory instead of hardcoding
        return hotKeyName switch
        {
            "Preferences.HotKeys.Exit" => new ShutdownCommand(),
            "Preferences.HotKeys.OpenPreferences" => new OpenPreferencesCommand(),
            "Preferences.HotKeys.ShowHotKeys" => new ShowHotKeysCommand(),
            _ => throw new ArgumentException("Unknown hot key name: " + hotKeyName + "", nameof(hotKeyName))
        };
    }

    private string CreateHotkeyString(ConsoleKeyInfo keyInfo)
    {
        var parts = new List<string>();

        if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Control)) parts.Add("Ctrl");

        if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Alt)) parts.Add("Alt");

        if (keyInfo.Modifiers.HasFlag(ConsoleModifiers.Shift)) parts.Add("Shift");

        parts.Add(keyInfo.Key.ToString());

        return string.Join("+", parts);
    }
}
