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
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Logging;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polyglot.AvaloniaApp.ViewModels;
using Polyglot.AvaloniaApp.Views;
using Preferences.Common;
using Polyglot.Common.Services;
using Preferences.Common.Services;
using ZLogger;

namespace Polyglot.AvaloniaApp;

public class App(IConfiguration configuration) : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var collection = new ServiceCollection();

        collection.AddLogging(conf =>
                conf.ClearProviders()
                    .SetMinimumLevel(LogLevel.Trace)
                    .AddZLoggerConsole(options => { options.OutputEncodingToUtf8 = false; }))
            .AddSingleton(configuration)
            .AddSingleton<ILocalizationService>(new AppLocalizationService())
            .Configure<PreferencesOptions>(options =>
                configuration.GetSection(PreferencesOptions.Preferences).Bind(options))
            .AddSingleton<MainWindowViewModel>();

        var services = collection.BuildServiceProvider();

        Logger.Sink = services.GetService<LoggerSink>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            desktop.MainWindow = new MainWindow
            {
                DataContext = services.GetRequiredService<MainWindowViewModel>()
            };

        base.OnFrameworkInitializationCompleted();
    }
}
