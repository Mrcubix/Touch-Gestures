using System;
using OpenTabletDriver.Plugin;
using TouchGestures.Lib;
using TouchGestures.Lib.Entities;
using SharedLogLevel = TouchGestures.Lib.Enums.LogLevel;

namespace TouchGestures.Entities
{
    public class StableLogger : Logger
    {
        static StableLogger()
        {
            BindingBuilder.Current = new StableBindingBuilder();
        }
        
        public override void Write(string group, string message, SharedLogLevel level = SharedLogLevel.Info, bool createStackTrace = false, bool notify = false)
        {
            Log.Write(group, message, (LogLevel)level);
        }

        public override void WriteNotify(string group, string text, SharedLogLevel level = SharedLogLevel.Info)
        {
            throw new NotImplementedException();
        }

        public override void Debug(string group, string message)
        {
            Log.Debug(group, message);
        }

        public override void Exception(Exception ex, SharedLogLevel level = SharedLogLevel.Error)
        {
            Log.Exception(ex);
        }
    }
}