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
/// Represents a logical grouping of related preference entries within the application's configuration system.
/// </summary>
/// <remarks>
/// This class defines a section of related preferences that are displayed together in the UI.
/// Each section contains a collection of <see cref="PreferencesEntry"/> instances representing
/// individual configurable preferences.
/// 
/// Sections can be ordered through their Order property to control their display sequence in the UI.
/// Each section typically has a Title that describes the category of preferences it contains.
/// 
/// PreferencesSection objects are organized within a <see cref="Preferences.Common.PreferencesOptions"/> container
/// to form the complete preferences hierarchy of the application.
/// 
/// This class serves as part of the Model layer in the MVVM architecture pattern used throughout
/// the preferences system.
/// </remarks>
public class PreferencesSection
{
    public required string Name { get; init; }

    public required int Order { get; set; }

    public required List<PreferencesEntry> Entries { get; set; }
}
