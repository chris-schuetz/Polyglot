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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Polyglot.Common.Models;
using ReactiveUI;

namespace Polyglot.AvaloniaApp.ViewModels;

public sealed class WorkspaceViewModel : ViewModelBase
{
    private int _focusedColumn;
    private bool _middleFocusOnTemplates = true;
    private int _selectedDeviceIndex;
    private int _selectedTemplateIndex;
    private int _selectedRunIndex;

    private DeviceViewModel? _selectedDevice;
    private RunViewModel? _selectedRun;
    private MessageViewModel? _selectedTemplate;

    public WorkspaceViewModel(Workspace model)
    {
        Model = model;

        Devices = new ReadOnlyObservableCollection<DeviceViewModel>(
            new ObservableCollection<DeviceViewModel>(Model.Devices.Select(d => new DeviceViewModel(d))));

        SelectedDevice = Devices.FirstOrDefault() ?? NoDeviceViewModel.Instance;
        _selectedDeviceIndex = SelectedDevice != null && Devices.Contains(SelectedDevice)
            ? Devices.IndexOf(SelectedDevice)
            : 0;
    }

    public Workspace Model { get; }

    public ReadOnlyObservableCollection<DeviceViewModel> Devices { get; }

    public DeviceViewModel? SelectedDevice
    {
        get => _selectedDevice;
        set
        {
            if (_selectedDevice == value)
            {
                return;
            }

            _selectedDevice = value ?? NoDeviceViewModel.Instance;
            _selectedDeviceIndex = Devices.Contains(_selectedDevice) ? Devices.IndexOf(_selectedDevice) : 0;
            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(Templates));
            this.RaisePropertyChanged(nameof(Runs));
            SelectedTemplate = null;
            SelectedRun = null;
            _selectedTemplateIndex = 0;
            _selectedRunIndex = 0;
            this.RaisePropertyChanged(nameof(SelectedDetail));
        }
    }

    public IReadOnlyList<MessageViewModel> Templates =>
        SelectedDevice?.Model.OutputMessages.Select(m => new MessageViewModel(m)).ToList()
        ?? new List<MessageViewModel>();

    public IReadOnlyList<RunViewModel> Runs =>
        SelectedDevice?.Model.Runs.Select(r => new RunViewModel(r)).ToList()
        ?? new List<RunViewModel>();

    public MessageViewModel? SelectedTemplate
    {
        get => _selectedTemplate;
        set
        {
            if (_selectedTemplate == value)
            {
                return;
            }

            _selectedTemplate = value;
            if (value != null)
            {
                _selectedRun = null;
                this.RaisePropertyChanged(nameof(SelectedRun));
            }

            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(SelectedDetail));
        }
    }

    public RunViewModel? SelectedRun
    {
        get => _selectedRun;
        set
        {
            if (_selectedRun == value)
            {
                return;
            }

            _selectedRun = value;
            if (value != null)
            {
                _selectedTemplate = null;
                this.RaisePropertyChanged(nameof(SelectedTemplate));
            }

            this.RaisePropertyChanged();
            this.RaisePropertyChanged(nameof(SelectedDetail));
        }
    }

    public object? SelectedDetail => (object?)SelectedTemplate ?? SelectedRun;

    // Navigation methods to mirror SpectreApp behavior
    public void MoveLeft()
    {
        if (_focusedColumn > 0)
        {
            _focusedColumn--;
        }
    }

    public void MoveRight()
    {
        if (_focusedColumn < 2)
        {
            _focusedColumn++;
        }
    }

    public void MoveUp()
    {
        switch (_focusedColumn)
        {
            case 0:
                if (_selectedDeviceIndex > 0)
                {
                    _selectedDeviceIndex--;
                    SelectedDevice = Devices.ElementAtOrDefault(_selectedDeviceIndex);
                }
                break;
            case 1:
                var templateCount = Templates.Count;

                if (_middleFocusOnTemplates)
                {
                    if (_selectedTemplateIndex > 0)
                    {
                        _selectedTemplateIndex--;
                        SelectedTemplate = Templates.ElementAtOrDefault(_selectedTemplateIndex);
                    }
                    else
                    {
                        // stay at first template
                    }
                }
                else
                {
                    if (_selectedRunIndex > 0)
                    {
                        _selectedRunIndex--;
                        SelectedRun = Runs.ElementAtOrDefault(_selectedRunIndex);
                    }
                    else
                    {
                        // First run -> jump to last template if any
                        if (templateCount > 0)
                        {
                            _middleFocusOnTemplates = true;
                            _selectedTemplateIndex = templateCount - 1;
                            SelectedTemplate = Templates.ElementAtOrDefault(_selectedTemplateIndex);
                        }
                    }
                }
                break;
            case 2:
                // no-op for details for now
                break;
        }
    }

    public void MoveDown()
    {
        switch (_focusedColumn)
        {
            case 0:
                if (Devices.Count > 0)
                {
                    _selectedDeviceIndex = Math.Min(Math.Max(0, Devices.Count - 1), _selectedDeviceIndex + 1);
                    SelectedDevice = Devices.ElementAtOrDefault(_selectedDeviceIndex);
                }
                break;
            case 1:
                var templateCount = Templates.Count;
                var runCount = Runs.Count;

                if (_middleFocusOnTemplates)
                {
                    if (templateCount == 0)
                    {
                        if (runCount > 0)
                        {
                            _middleFocusOnTemplates = false;
                            _selectedRunIndex = 0;
                            SelectedRun = Runs.ElementAtOrDefault(_selectedRunIndex);
                        }
                    }
                    else if (_selectedTemplateIndex < templateCount - 1)
                    {
                        _selectedTemplateIndex++;
                        SelectedTemplate = Templates.ElementAtOrDefault(_selectedTemplateIndex);
                    }
                    else
                    {
                        if (runCount > 0)
                        {
                            _middleFocusOnTemplates = false;
                            _selectedRunIndex = 0;
                            SelectedRun = Runs.ElementAtOrDefault(_selectedRunIndex);
                        }
                    }
                }
                else
                {
                    if (runCount == 0)
                    {
                        // nothing to do
                    }
                    else if (_selectedRunIndex < runCount - 1)
                    {
                        _selectedRunIndex++;
                        SelectedRun = Runs.ElementAtOrDefault(_selectedRunIndex);
                    }
                    else
                    {
                        // at last run: stay
                    }
                }
                break;
            case 2:
                // no-op
                break;
        }
    }

    public void Select()
    {
        switch (_focusedColumn)
        {
            case 0:
                _focusedColumn = 1;
                _selectedTemplateIndex = 0;
                _selectedRunIndex = 0;

                var templateCount = Templates.Count;
                var runCount = Runs.Count;
                if (templateCount > 0)
                {
                    _middleFocusOnTemplates = true;
                    SelectedTemplate = Templates.ElementAtOrDefault(_selectedTemplateIndex);
                }
                else if (runCount > 0)
                {
                    _middleFocusOnTemplates = false;
                    SelectedRun = Runs.ElementAtOrDefault(_selectedRunIndex);
                }
                else
                {
                    _middleFocusOnTemplates = true;
                    SelectedTemplate = null;
                    SelectedRun = null;
                }
                break;
            case 1:
                _focusedColumn = 2;
                break;
            case 2:
                // details no-op for now
                break;
        }
    }
}
