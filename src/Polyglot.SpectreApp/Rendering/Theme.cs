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

namespace Polyglot.SpectreApp.Rendering;

public sealed class Theme
{
    public ConsoleColor PrimaryColor { get; set; } = ConsoleColor.Blue;
    public ConsoleColor SecondaryColor { get; set; } = ConsoleColor.Cyan;
    public ConsoleColor SuccessColor { get; set; } = ConsoleColor.Green;
    public ConsoleColor WarningColor { get; set; } = ConsoleColor.Yellow;
    public ConsoleColor ErrorColor { get; set; } = ConsoleColor.Red;
    public ConsoleColor InfoColor { get; set; } = ConsoleColor.White;
    public ConsoleColor MutedColor { get; set; } = ConsoleColor.Gray;
    public ConsoleColor BorderColor { get; set; } = ConsoleColor.Gray;
    public ConsoleColor BackgroundColor { get; set; } = ConsoleColor.Black;

    public static Theme CreateLightTheme()
    {
        return new Theme
        {
            PrimaryColor = ConsoleColor.Blue,
            SecondaryColor = ConsoleColor.DarkBlue,
            SuccessColor = ConsoleColor.DarkGreen,
            WarningColor = ConsoleColor.DarkYellow,
            ErrorColor = ConsoleColor.Red,
            InfoColor = ConsoleColor.Black,
            MutedColor = ConsoleColor.Gray,
            BorderColor = ConsoleColor.Gray,
            BackgroundColor = ConsoleColor.White
        };
    }

    public static Theme CreateDarkTheme()
    {
        return new Theme
        {
            PrimaryColor = ConsoleColor.Cyan,
            SecondaryColor = ConsoleColor.Blue,
            SuccessColor = ConsoleColor.Green,
            WarningColor = ConsoleColor.Yellow,
            ErrorColor = ConsoleColor.Red,
            InfoColor = ConsoleColor.White,
            MutedColor = ConsoleColor.Gray,
            BorderColor = ConsoleColor.Gray,
            BackgroundColor = ConsoleColor.Black
        };
    }
}
