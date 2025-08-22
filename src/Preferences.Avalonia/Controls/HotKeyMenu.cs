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

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Platform;
using Avalonia.Input;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace Preferences.Avalonia.Controls;

/// <summary>
///     An enhanced Avalonia Menu control that automatically registers keyboard shortcuts (input gestures)
///     from menu items to the parent window. This eliminates the need for manual key binding registration
///     and ensures all menu commands are accessible via their configured keyboard shortcuts.
/// </summary>
/// <remarks>
///     The control recursively scans all menu items for Command and InputGesture properties, registers
///     them as KeyBindings on the parent window, and maintains these bindings when input gestures change.
///     Based on the approach described at: https://github.com/AvaloniaUI/Avalonia/issues/2441#issuecomment-2742347861
/// </remarks>
public class HotKeyMenu : Menu
{
    private readonly List<MenuItem> _items = [];

    public HotKeyMenu()
    {
    }

    public HotKeyMenu(IMenuInteractionHandler interactionHandler) : base(interactionHandler)
    {
    }

    protected override Type StyleKeyOverride => typeof(Menu);

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);

        UpdateHotKeys();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);

        var window = this.FindAncestorOfType<Window>();
        if (window != null)
        {
            foreach (var item in _items)
            {
                var existingBinding = window.KeyBindings
                    .FirstOrDefault(kb => kb.Command == item.Command && kb.Gesture == item.InputGesture);
                if (existingBinding != null)
                {
                    window.KeyBindings.Remove(existingBinding);
                }

                item.PropertyChanged -= HandleGestureChanged;
            }
        }

        _items.Clear();
    }

    private void HandleGestureChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
    {
        if (e.Property != MenuItem.InputGestureProperty)
        {
            return;
        }

        UpdateHotKeys();
    }

    private void UpdateHotKeys()
    {
        var window = this.FindAncestorOfType<Window>();
        if (window is null)
        {
            return;
        }

        foreach (var item in _items)
        {
            var existingBinding = window.KeyBindings
                .FirstOrDefault(kb => kb.Command == item.Command && kb.Gesture == item.InputGesture);
            if (existingBinding != null)
            {
                window.KeyBindings.Remove(existingBinding);
            }

            item.PropertyChanged -= HandleGestureChanged;
        }

        _items.Clear();
        foreach (var logicalChild in LogicalChildren.OfType<MenuItem>())
        {
            SearchKeyBinding(logicalChild);
        }

        foreach (var item in _items)
        {
            window.KeyBindings.Add(new KeyBinding { Command = item.Command!, Gesture = item.InputGesture! });
            item.PropertyChanged += HandleGestureChanged;
        }
    }

    private void SearchKeyBinding(MenuItem mi)
    {
        foreach (var logicalChild in mi.GetLogicalChildren().OfType<MenuItem>())
        {
            SearchKeyBinding(logicalChild);
        }

        if (mi.Command == null || mi.InputGesture == null)
        {
            return;
        }

        _items.Add(mi);
    }
}
