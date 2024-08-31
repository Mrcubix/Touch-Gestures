using System;
using OpenTabletDriver.Plugin;
using TouchGestures.Lib;
using TouchGestures.Lib.Entities;
using SharedLogLevel = TouchGestures.Lib.Enums.LogLevel;

namespace TouchGestures.Entities
{
    public class BulletproofLogger : Logger
    {
        static BulletproofLogger()
        {
            BindingBuilder.Current = new BulletproofBindingBuilder();
        }

        public override void Write(string group, string message, SharedLogLevel level = SharedLogLevel.Info, bool createStackTrace = false, bool notify = false)
        {
            Log.Write(group, message, (LogLevel)level, createStackTrace, notify);
        }

        public override void WriteNotify(string group, string text, SharedLogLevel level = SharedLogLevel.Info)
        {
            Log.WriteNotify(group, text, (LogLevel)level);
        }

        public override void Debug(string group, string message)
        {
            Log.Debug(group, message);
        }

        public override void Exception(Exception ex, SharedLogLevel level = SharedLogLevel.Error)
        {
            Log.Exception(ex, (LogLevel)level);
        }
    }
}