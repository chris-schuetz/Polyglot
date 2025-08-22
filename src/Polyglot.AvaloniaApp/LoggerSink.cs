using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Logging;
using Microsoft.Extensions.Logging;

namespace Polyglot.AvaloniaApp;

internal class LoggerSink : ILogSink
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
        else if (_loggers.TryGetValue(source.GetType(), out logger) == false)
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
        else if (_loggers.TryGetValue(source.GetType(), out logger) == false)
        {
            logger = _loggerFactory.CreateLogger(source.GetType());
            _loggers.Add(source.GetType(), logger);
        }

        object?[] parameters = propertyValues.Prepend(area).ToArray();
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
