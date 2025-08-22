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

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Preferences.Common.Messages;
using Polyglot.Common;
using Polyglot.SpectreApp.Rendering;
using SlimMessageBus;

namespace Polyglot.SpectreApp;

public class InteractiveApplication : IHostedService
{
    private readonly IMessageBus _bus;
    private readonly ILogger<InteractiveApplication> _logger;
    private readonly IScreen _screen;
    private volatile bool _canRun;

    public InteractiveApplication(
        ILogger<InteractiveApplication> logger,
        IMessageBus bus,
        IScreen screen)
    {
        _logger = logger;
        _bus = bus;
        _screen = screen;
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogEntry();
        Initialize();
        _ = Task.Factory.StartNew(
            RunMainLoopAsync,
            CancellationToken.None,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _canRun = false;
        return Task.CompletedTask;
    }

    private void Initialize()
    {
        _logger.LogEntry();
        _canRun = true;

        try
        {
            _logger.LogTrace("Initializing screen layout...");
            _screen.InitializeLayout();
            _screen.Title = "Spectre Sample Application";
            _screen.StatusBarHintText = "Press F1 for help";
            _bus.Publish(new StatusMessage("Ready", StatusMessageType.Success));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error initializing");
        }
        finally
        {
            _logger.LogExit();
        }
    }

    private async Task RunMainLoopAsync()
    {
        while (_canRun)
        {
            _screen.RenderLayout();

            var keyInfo = Console.ReadKey(true);
            await _bus.Publish(new KeyInputMessage(keyInfo), cancellationToken: CancellationToken.None);
        }
    }
}
