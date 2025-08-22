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
using Polyglot.Common;
using Preferences.Common.Messages;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Polyglot.SpectreApp.Rendering;

public sealed class ScreenLayout
{
    private readonly ILogger<ScreenLayout> _logger;

    public ScreenLayout(ILogger<ScreenLayout> logger)
    {
        _logger = logger;
        CurrentTheme = Theme.CreateDarkTheme();
    }

    public Theme CurrentTheme { get; set; }

    public void ClearScreen()
    {
        _logger.LogEntry();
        AnsiConsole.Clear();
        _logger.LogExit();
    }

    public void RenderMainHeader(string title)
    {
        _logger.LogEntry();
        Console.SetCursorPosition(0, 0);
        var rule = new Rule(title)
        {
            Style = GetPrimaryStyle(),
            Justification = Justify.Center
        };

        AnsiConsole.Write(rule);
        AddVerticalSpace(1);
        _logger.LogExit();
    }

    public IRenderable GetHeader(string title)
    {
        return new Rule(title) { Style = GetPrimaryStyle() };
    }

    public IRenderable GetSubHeader(string title)
    {
        return new Rule(title) { Style = GetSecondaryStyle() };
    }

    public Renderable GetContent(string title)
    {
        return new Text(title, GetInfoStyle());
    }

    public Renderable GetSelectedContent(string title)
    {
        return new Text(title, GetSelectedStyle());
    }

    public void RenderMainContent(IRenderable? currentPageContent)
    {
        Console.SetCursorPosition(0, 1);
        AnsiConsole.Write(currentPageContent ?? new Markup("No content", GetMutedStyle()));
        ;

        var (_, row) = Console.GetCursorPosition();
        for (var i = row; i < Console.WindowHeight - 5; i++)
        {
            AnsiConsole.WriteLine();
        }
    }

    public void ClearMainContent()
    {
        Console.SetCursorPosition(0, 1);
        for (var i = 0; i < Console.WindowHeight - 5; i++)
        {
            AnsiConsole.WriteLine();
        }
    }

    public void RenderStatusBar(string hint, string status, StatusMessageType statusType)
    {
        Console.SetCursorPosition(0, Console.WindowHeight - 4);
        var content = new Grid();
        content.AddColumn(new GridColumn().NoWrap());
        content.AddColumn(new GridColumn().NoWrap());

        content.AddRow(
            new Markup(hint, GetMutedStyle()),
            new Markup(status, GetStatusStyle(statusType))
        );

        var panel = new Panel(content)
        {
            Border = BoxBorder.Rounded,
            BorderStyle = GetBorderStyle(),
            Padding = new Padding(1, 0)
        };

        AnsiConsole.Write(panel);
    }

    public void UpdateTheme(string themeName)
    {
        _logger.LogDebug("Updating theme to: {ThemeName}", themeName);

        CurrentTheme = themeName.ToLowerInvariant() switch
        {
            "light" => Theme.CreateLightTheme(),
            "dark" => Theme.CreateDarkTheme(),
            _ => Theme.CreateDarkTheme()
        };

        AnsiConsole.Background = Color.FromConsoleColor(CurrentTheme.BackgroundColor);
        _logger.LogInformation("Theme updated to: {ThemeName}", themeName);
    }

    private void AddVerticalSpace(int lines)
    {
        for (var i = 0; i < lines; i++)
        {
            AnsiConsole.WriteLine();
        }
    }

    private Style GetPrimaryStyle()
    {
        return new Style(Color.FromConsoleColor(CurrentTheme.PrimaryColor),
            Color.FromConsoleColor(CurrentTheme.BackgroundColor));
    }

    private Style GetSecondaryStyle()
    {
        return new Style(Color.FromConsoleColor(CurrentTheme.SecondaryColor),
            Color.FromConsoleColor(CurrentTheme.BackgroundColor));
    }

    private Style GetMutedStyle()
    {
        return new Style(Color.FromConsoleColor(CurrentTheme.MutedColor),
            Color.FromConsoleColor(CurrentTheme.BackgroundColor));
    }

    private Style GetBorderStyle()
    {
        return new Style(Color.FromConsoleColor(CurrentTheme.BorderColor),
            Color.FromConsoleColor(CurrentTheme.BackgroundColor));
    }

    private Style GetInfoStyle()
    {
        return new Style(Color.FromConsoleColor(CurrentTheme.InfoColor),
            Color.FromConsoleColor(CurrentTheme.BackgroundColor));
    }

    private Style GetSelectedStyle()
    {
        return new Style(Color.FromConsoleColor(CurrentTheme.InfoColor),
            Color.FromConsoleColor(CurrentTheme.MutedColor));
    }

    private Style GetStatusStyle(StatusMessageType messageType)
    {
        var color = messageType switch
        {
            StatusMessageType.Info => Color.FromConsoleColor(CurrentTheme.InfoColor),
            StatusMessageType.Success => Color.FromConsoleColor(CurrentTheme.SuccessColor),
            StatusMessageType.Warning => Color.FromConsoleColor(CurrentTheme.WarningColor),
            StatusMessageType.Error => Color.FromConsoleColor(CurrentTheme.ErrorColor),
            _ => Color.FromConsoleColor(CurrentTheme.MutedColor)
        };

        return new Style(color, Color.FromConsoleColor(CurrentTheme.BackgroundColor));
    }
}
