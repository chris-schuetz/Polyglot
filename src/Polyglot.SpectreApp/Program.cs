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

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Preferences.Common;
using Preferences.Common.Messages;
using Polyglot.Common.Services;
using Preferences.Common.Services;
using Polyglot.SpectreApp.Pages;
using Polyglot.SpectreApp.Rendering;
using SlimMessageBus;
using SlimMessageBus.Host;
using SlimMessageBus.Host.Memory;
using SlimMessageBus.Host.Serialization.SystemTextJson;
using Spectre.Console;
using ZLogger;

namespace Polyglot.SpectreApp;

internal sealed class Program
{
    public static async Task<int> Main(string[] args)
    {
        try
        {
            await Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((ctx, builder) =>
                {
                    builder
                        .AddJsonFile("appsettings.json", true, true)
                        .AddEnvironmentVariables()
                        .AddCommandLine(args);
                })
                .ConfigureServices((ctx, services) => { ConfigureServices(services, ctx.Configuration); })
                .Build()
                .RunAsync();
        }
        catch (Exception ex)
        {
            AnsiConsole.WriteException(ex);
            return -1;
        }

        return 0;
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddHostedService<InteractiveApplication>();
        services.AddSlimMessageBus(mbb =>
        {
            mbb.Produce<ShutdownCommand>(x => x.DefaultTopic(nameof(ShutdownCommand)));
            mbb.Consume<ShutdownCommand>(x => x.Topic(nameof(ShutdownCommand)));
            mbb.Produce<KeyInputMessage>(x => x.DefaultTopic(nameof(KeyInputMessage)));
            mbb.Consume<KeyInputMessage>(x => x.Topic(nameof(KeyInputMessage)));
            mbb.Produce<ShowHotKeysCommand>(x => x.DefaultTopic(nameof(ShowHotKeysCommand)));
            mbb.Consume<ShowHotKeysCommand>(x => x.Topic(nameof(ShowHotKeysCommand)));
            mbb.Produce<OpenPreferencesCommand>(x => x.DefaultTopic(nameof(OpenPreferencesCommand)));
            mbb.Consume<OpenPreferencesCommand>(x => x.Topic(nameof(OpenPreferencesCommand)));
            mbb.Produce<StatusMessage>(x => x.DefaultTopic(nameof(StatusMessage)));
            mbb.Consume<StatusMessage>(x => x.Topic(nameof(StatusMessage)));
            mbb.WithProviderMemory();
            mbb.AddJsonSerializer();
        });

        services.AddSingleton(configuration);

        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddZLoggerConsole(options =>
            {
                options.UsePlainTextFormatter(formatter =>
                {
                    formatter.SetPrefixFormatter($"[{0:HH:mm:ss}] ",
                        (in MessageTemplate template, in LogInfo info) =>
                            template.Format(info.Timestamp.Utc.DateTime));
                });
            });
        });

        services.Configure<PreferencesOptions>(options =>
            configuration.GetSection(PreferencesOptions.Preferences).Bind(options));

        services.AddSingleton<ILocalizationService, AppLocalizationService>();

        services.AddSingleton<ScreenLayout>();
        services.AddSingleton<Screen>();
        services.AddSingleton<IScreen>(x => x.GetRequiredService<Screen>());
        services.AddSingleton<IConsumer<StatusMessage>>(x => x.GetRequiredService<Screen>());
        services.AddSingleton<IConsumer<ShowHotKeysCommand>>(x => x.GetRequiredService<Screen>());
        services.AddSingleton<IConsumer<OpenPreferencesCommand>>(x => x.GetRequiredService<Screen>());
        services.AddSingleton<PreferencesPage>();
        services.AddSingleton<HotKeyPage>();
        services.AddSingleton<HotKeyService>();
        services.AddSingleton<IConsumer<KeyInputMessage>>(x => x.GetRequiredService<HotKeyService>());
        services.AddSingleton<IConsumer<ShutdownCommand>, ShutdownService>();
    }
}
