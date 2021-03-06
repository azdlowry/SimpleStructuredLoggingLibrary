﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;

namespace SimpleStructuredLoggingLibrary
{
    public class Logger
    {
        private Subject<LogEvent> _logEvents;

        public Logger(Action<IObservable<LogEvent>> loggingPipelineFactory)
        {
            _logEvents = new Subject<LogEvent>();
            loggingPipelineFactory(_logEvents);
        }

        public RequestLog StartRequestLog()
        {
            return new RequestLog(_logEvents);
        }

        public void Info(object logDetail, [CallerFilePath] string callerFilePath = "", [CallerLineNumber]int callerLineNumber = 0)
        {
            var callerInfo = new CallerInfo() { FilePath = callerFilePath, LineNum = callerLineNumber };
            Log(LogLevel.Info, logDetail, callerInfo);
        }

        public void Warn(object logDetail, [CallerFilePath] string callerFilePath = "", [CallerLineNumber]int callerLineNumber = 0)
        {
            var callerInfo = new CallerInfo() { FilePath = callerFilePath, LineNum = callerLineNumber };
            Log(LogLevel.Warn, logDetail, callerInfo);
        }

        public void Error(object logDetail, [CallerFilePath] string callerFilePath = "", [CallerLineNumber]int callerLineNumber = 0)
        {
            var callerInfo = new CallerInfo() { FilePath = callerFilePath, LineNum = callerLineNumber };
            Log(LogLevel.Error, logDetail, callerInfo);
        }

        private void Log(LogLevel level, object logDetail, CallerInfo callerInfo)
        {
            _logEvents.OnNext(new LogEvent() { Level = level, Content = logDetail, CallerInfo = callerInfo });
        }
    }
}
