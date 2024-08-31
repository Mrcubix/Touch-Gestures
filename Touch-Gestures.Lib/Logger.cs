using System;
using TouchGestures.Lib.Enums;

namespace TouchGestures.Lib;

public abstract class Logger
{
    public static Logger? Instance { get; set; }

    public abstract void Write(string group, string message, LogLevel level = LogLevel.Info, bool createStackTrace = false, bool notify = false);

    public abstract void WriteNotify(string group, string text, LogLevel level = LogLevel.Info);

    public abstract void Debug(string group, string message);

    public abstract void Exception(Exception ex, LogLevel level = LogLevel.Error);
}