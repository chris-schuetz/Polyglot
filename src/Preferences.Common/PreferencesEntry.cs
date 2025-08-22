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
/// Represents a model for an individual preference entry in the application's configuration system.
/// </summary>
/// <remarks>
/// This sealed class encapsulates a single configurable preference value, including its key, title,
/// current value, and optional set of predefined choices.
/// 
/// Preferences entries are typically organized within <see cref="PreferencesSection"/> collections to form
/// a complete preferences hierarchy. The entry's properties are exposed to the UI through the
/// <see cref="EntryViewModel"/> wrapper class which provides additional UI-specific functionality
/// and reactive binding support.
/// 
/// This class serves as part of the Model layer in the MVVM architecture pattern used throughout
/// the preferences system.
/// </remarks>
public sealed class PreferencesEntry
{
    public required string Name { get; init; }

    public required string Value { get; set; }
    
    public List<string>? Options { get; set; }
}
