using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;

namespace SimpleStructuredLoggingLibrary.Test
{
    public class LoggerTests
    {
        [Fact]
        public void should_pass_object_in_logevent()
        {
            var output = new List<LogEvent>();

            var logger = new Logger(logEvents => logEvents
                .Subscribe(log => output.Add(log))
                );

            var obj = new { message = "Hi" };
            logger.Info(obj);
            logger.Warn(obj);
            logger.Error(obj);

            Assert.Collection(output,
                log => Assert.Equal(obj, log.Content),
                log => Assert.Equal(obj, log.Content),
                log => Assert.Equal(obj, log.Content));
        }

        [Fact]
        public void should_pass_level_in_logevent()
        {
            var output = new List<LogEvent>();

            var logger = new Logger(logEvents => logEvents
                .Subscribe(log => output.Add(log))
                );

            var obj = new { message = "Hi" };
            logger.Info(obj);
            logger.Warn(obj);
            logger.Error(obj);

            Assert.Collection(output,
                log => Assert.Equal(LogLevel.Info, log.Level),
                log => Assert.Equal(LogLevel.Warn, log.Level),
                log => Assert.Equal(LogLevel.Error, log.Level));
        }

        [Fact]
        public void should_capture_callerfilepath()
        {
            var output = new List<LogEvent>();

            var logger = new Logger(logEvents => logEvents
                .Subscribe(log => output.Add(log))
                );
            var callerFilePath = GetCallerFilePath();

            var obj = new { message = "Hi" };
            logger.Info(obj);
            logger.Warn(obj);
            logger.Error(obj);

            Assert.Collection(output,
                log => Assert.Equal(callerFilePath, log.CallerInfo.FilePath),
                log => Assert.Equal(callerFilePath, log.CallerInfo.FilePath),
                log => Assert.Equal(callerFilePath, log.CallerInfo.FilePath));
        }

        [Fact]
        public void should_capture_callerlinenumber()
        {
            var output = new List<LogEvent>();

            var logger = new Logger(logEvents => logEvents
                .Subscribe(log => output.Add(log))
                );
            var callerLineNumber = GetCallerLineNumber();

            var obj = new { message = "Hi" };
            logger.Info(obj);
            logger.Warn(obj);
            logger.Error(obj);

            Assert.Collection(output,
                log => Assert.Equal(callerLineNumber + 3, log.CallerInfo.LineNum),
                log => Assert.Equal(callerLineNumber + 4, log.CallerInfo.LineNum),
                log => Assert.Equal(callerLineNumber + 5, log.CallerInfo.LineNum));
        }

        private string GetCallerFilePath([CallerFilePath]string callerFilePath = "Shouldn't Get This")
        {
            return callerFilePath;
        }

        private int GetCallerLineNumber([CallerLineNumber]int callerLineNumber = -999)
        {
            return callerLineNumber;
        }
    }
}
