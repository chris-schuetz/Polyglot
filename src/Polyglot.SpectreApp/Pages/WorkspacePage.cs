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

using Polyglot.Common.Models;
using Polyglot.SpectreApp.Rendering;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Polyglot.SpectreApp.Pages;

public sealed class WorkspacePage(Workspace workspace) : IPage
{
    private int _focusedColumn; // 0=Devices, 1=Middle (Templates/Runs), 2=Detail
    private bool _middleFocusOnTemplates = true; // within column 1
    private int _selectedDeviceIndex;
    private int _selectedRunIndex;
    private int _selectedTemplateIndex;

    public IRenderable Draw(ScreenLayout screenLayout)
    {
        var rows = new List<IRenderable>
        {
            screenLayout.GetHeader("Workspace")
        };

        // Build three columns using Grid
        var grid = new Grid();
        grid.AddColumn(new GridColumn());
        grid.AddColumn(new GridColumn());
        grid.AddColumn(new GridColumn());

        // Column 1: Devices
        var devices = workspace.Devices.ToList();
        var col1Rows = new List<IRenderable> { screenLayout.GetSubHeader("Devices") };
        for (var i = 0; i < devices.Count; i++)
        {
            var device = devices[i];
            var display = device.ConnectionParameters switch
            {
                TcpIpConnectionParameters tcp => $"TCP {tcp.Host}:{tcp.Port}",
                _ => device.ConnectionParameters.GetType().Name
            };
            var item = _focusedColumn == 0 && i == _selectedDeviceIndex
                ? (IRenderable)screenLayout.GetSelectedContent(display)
                : screenLayout.GetContent(display);
            col1Rows.Add(item);
        }

        var col1 = new Rows(col1Rows);

        // Column 2: Templates and Runs
        var selectedDevice = devices.ElementAtOrDefault(_selectedDeviceIndex);
        var templates = selectedDevice?.OutputMessages?.ToList() ?? new List<Message>();
        var runs = selectedDevice?.Runs?.ToList() ?? new List<Run>();

        var col2Rows = new List<IRenderable>
        {
            screenLayout.GetSubHeader("Templates")
        };
        for (var i = 0; i < templates.Count; i++)
        {
            var m = templates[i];
            var isSelected = _focusedColumn == 1 && _middleFocusOnTemplates && i == _selectedTemplateIndex;
            col2Rows.Add(isSelected ? screenLayout.GetSelectedContent(m.Text) : screenLayout.GetContent(m.Text));
        }

        col2Rows.Add(screenLayout.GetSubHeader("Runs"));
        for (var i = 0; i < runs.Count; i++)
        {
            var r = runs[i];
            var display = r.ConnectionParameters switch
            {
                TcpIpConnectionParameters tcp => $"Run {tcp.Host}:{tcp.Port}",
                _ => "Run"
            };
            var isSelected = _focusedColumn == 1 && !_middleFocusOnTemplates && i == _selectedRunIndex;
            col2Rows.Add(isSelected ? screenLayout.GetSelectedContent(display) : screenLayout.GetContent(display));
        }

        var col2 = new Rows(col2Rows);

        // Column 3: Detail (empty for now)
        var col3Rows = new List<IRenderable> { screenLayout.GetSubHeader("Details") };
        // We highlight header when focused on details
        if (_focusedColumn == 2)
        {
            col3Rows[0] = screenLayout.GetSelectedContent("Details");
        }

        var col3 = new Rows(col3Rows);

        grid.AddRow(col1, col2, col3);

        rows.Add(grid);
        return new Rows(rows);
    }

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
                _selectedDeviceIndex = Math.Max(0, _selectedDeviceIndex - 1);
                break;
            case 1:
                var devices = workspace.Devices.ToList();
                var selectedDevice = devices.ElementAtOrDefault(_selectedDeviceIndex);
                var templateCount = selectedDevice?.OutputMessages?.Count ?? 0;
                var runCount = selectedDevice?.Runs?.Count ?? 0;

                if (_middleFocusOnTemplates)
                {
                    if (_selectedTemplateIndex > 0)
                    {
                        _selectedTemplateIndex--;
                    }
                    else
                    {
                        // At first template: stay (do not move to devices on Up)
                    }
                }
                else
                {
                    if (_selectedRunIndex > 0)
                    {
                        _selectedRunIndex--;
                    }
                    else
                    {
                        // First run: move to last template if any
                        if (templateCount > 0)
                        {
                            _middleFocusOnTemplates = true;
                            _selectedTemplateIndex = templateCount - 1;
                        }
                    }
                }

                break;
            case 2:
                // no-op for detail for now
                break;
        }
    }

    public void MoveDown()
    {
        switch (_focusedColumn)
        {
            case 0:
                _selectedDeviceIndex = Math.Min(Math.Max(0, workspace.Devices.Count - 1), _selectedDeviceIndex + 1);
                break;
            case 1:
                var devices = workspace.Devices.ToList();
                var selectedDevice = devices.ElementAtOrDefault(_selectedDeviceIndex);
                var templateCount = selectedDevice?.OutputMessages?.Count ?? 0;
                var runCount = selectedDevice?.Runs?.Count ?? 0;

                if (_middleFocusOnTemplates)
                {
                    if (templateCount == 0)
                    {
                        // No templates; jump to first run if available
                        if (runCount > 0)
                        {
                            _middleFocusOnTemplates = false;
                            _selectedRunIndex = 0;
                        }
                    }
                    else if (_selectedTemplateIndex < templateCount - 1)
                    {
                        _selectedTemplateIndex++;
                    }
                    else
                    {
                        // At last template -> move to first run if any
                        if (runCount > 0)
                        {
                            _middleFocusOnTemplates = false;
                            _selectedRunIndex = 0;
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
                    }
                    else
                    {
                        // At last run: stay (no wrap)
                    }
                }

                break;
            case 2:
                // no-op for detail for now
                break;
        }
    }

    public void Select()
    {
        switch (_focusedColumn)
        {
            case 0:
                // selecting device moves focus to middle
                _focusedColumn = 1;
                // reset indices when selecting a new device
                _selectedTemplateIndex = 0;
                _selectedRunIndex = 0;

                // choose first available section
                var devices = workspace.Devices.ToList();
                var selectedDevice = devices.ElementAtOrDefault(_selectedDeviceIndex);
                var templateCount = selectedDevice?.OutputMessages?.Count ?? 0;
                var runCount = selectedDevice?.Runs?.Count ?? 0;
                if (templateCount > 0)
                {
                    _middleFocusOnTemplates = true;
                }
                else if (runCount > 0)
                {
                    _middleFocusOnTemplates = false;
                }
                else
                {
                    // No items at all; default to templates
                    _middleFocusOnTemplates = true;
                }
                break;
            case 1:
                // selecting template or run moves to details
                _focusedColumn = 2;
                break;
            case 2:
                // details select no-op for now
                break;
        }
    }
}
