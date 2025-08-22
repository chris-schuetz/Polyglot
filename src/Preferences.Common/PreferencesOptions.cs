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

namespace Preferences.Common;

/// <summary>
/// Represents the root container for all application preferences organized in sections.
/// </summary>
/// <remarks>
/// This class serves as the top-level model for the preferences system, containing a collection
/// of <see cref="PreferencesSection"/> objects that group related preferences together.
/// 
/// PreferencesOptions is typically used when initializing the preferences UI system and provides
/// the complete preferences hierarchy for the application. It defines a constant string identifier
/// for preferences configuration.
/// 
/// The class follows a simple structure where all preference sections are stored in a flat list,
/// with each section containing its own entries. This design allows for flexible organization
/// of application settings while maintaining a consistent UI representation.
/// </remarks>
public class PreferencesOptions
{
    public const string Preferences = "Preferences";

    public List<PreferencesSection> Sections { get; set; } = [];
}
