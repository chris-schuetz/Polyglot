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

using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Polyglot.AvaloniaApp.Models;

namespace Polyglot.AvaloniaApp.ViewModels;

public class WorkspaceViewModel : ViewModelBase
{
    private readonly ObservableCollection<MessageViewModel> _messages = new();

    public WorkspaceViewModel(Workspace model)
    {
        Model = model;
        Messages = new ReadOnlyObservableCollection<MessageViewModel>(_messages);

        // Initialize from existing model messages
        foreach (var m in Model.OutputMessages)
        {
            _messages.Add(new MessageViewModel(m));
        }

        // Keep VM collection in sync with model collection
        Model.OutputMessages.CollectionChanged += OnModelMessagesChanged;
    }

    public ReadOnlyObservableCollection<MessageViewModel> Messages { get; }

    public Workspace Model { get; }

    private void OnModelMessagesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                if (e.NewItems != null)
                {
                    foreach (Message m in e.NewItems)
                    {
                        _messages.Add(new MessageViewModel(m));
                    }
                }

                break;
            case NotifyCollectionChangedAction.Remove:
                if (e.OldItems != null)
                {
                    foreach (Message m in e.OldItems)
                    {
                        var vm = FindByModel(m);
                        if (vm != null)
                        {
                            _messages.Remove(vm);
                        }
                    }
                }

                break;
            case NotifyCollectionChangedAction.Reset:
                _messages.Clear();
                break;
            case NotifyCollectionChangedAction.Replace:
                if (e.OldItems != null)
                {
                    foreach (Message m in e.OldItems)
                    {
                        var vm = FindByModel(m);
                        if (vm != null)
                        {
                            _messages.Remove(vm);
                        }
                    }
                }

                if (e.NewItems != null)
                {
                    foreach (Message m in e.NewItems)
                    {
                        _messages.Add(new MessageViewModel(m));
                    }
                }

                break;
            case NotifyCollectionChangedAction.Move:
                // For simplicity, reset order when moves occur
                _messages.Clear();
                foreach (var m in Model.OutputMessages)
                {
                    _messages.Add(new MessageViewModel(m));
                }

                break;
        }
    }

    private MessageViewModel? FindByModel(Message model)
    {
        return _messages.FirstOrDefault(vm => ReferenceEquals(vm.Model, model));
    }
}
