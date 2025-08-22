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
using System.Linq;
using Avalonia.Logging;
using Microsoft.Extensions.Logging;

namespace Polyglot.AvaloniaApp;

internal sealed class LoggerSink : ILogSink
{
    private readonly ILogger<LoggerSink> _defaultLogger;
    private readonly ILoggerFactory _loggerFactory;
    private readonly Dictionary<Type, ILogger> _loggers = new();

    public LoggerSink(ILoggerFactory loggerFactory)
    {
        _loggerFactory = loggerFactory;
        _defaultLogger = loggerFactory.CreateLogger<LoggerSink>();
    }

    public bool IsEnabled(LogEventLevel level, string area)
    {
        return true;
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate)
    {
        ILogger? logger;
        if (source is null)
        {
            logger = _defaultLogger;
        }
        else if (!_loggers.TryGetValue(source.GetType(), out logger))
        {
            logger = _loggerFactory.CreateLogger(source.GetType());
            _loggers.Add(source.GetType(), logger);
        }

        logger.Log(ToLogLevel(level), "({Area}) " + messageTemplate, area);
    }

    public void Log(LogEventLevel level, string area, object? source, string messageTemplate,
        params object?[] propertyValues)
    {
        ILogger? logger;
        if (source is null)
        {
            logger = _defaultLogger;
        }
        else if (!_loggers.TryGetValue(source.GetType(), out logger))
        {
            logger = _loggerFactory.CreateLogger(source.GetType());
            _loggers.Add(source.GetType(), logger);
        }

        var parameters = propertyValues.Prepend(area).ToArray();
        logger.Log(ToLogLevel(level), "({Area}) " + messageTemplate, parameters);
    }

    public static LogLevel ToLogLevel(LogEventLevel level)
    {
        switch (level)
        {
            case LogEventLevel.Verbose:
                return LogLevel.Trace;
            case LogEventLevel.Debug:
                return LogLevel.Debug;
            case LogEventLevel.Information:
                return LogLevel.Information;
            case LogEventLevel.Warning:
                return LogLevel.Warning;
            case LogEventLevel.Error:
                return LogLevel.Error;
            case LogEventLevel.Fatal:
                return LogLevel.Critical;
            default:
                throw new ArgumentOutOfRangeException(nameof(level), level, null);
        }
    }
}
