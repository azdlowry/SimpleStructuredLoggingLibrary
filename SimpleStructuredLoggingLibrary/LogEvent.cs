using System;
using System.Collections.Generic;
using System.Linq;

namespace SimpleStructuredLoggingLibrary
{
    public enum LogLevel
    {
        Info,
        Warn,
        Error
    }

    public class LogEvent
    {
        private List<string> _tags = new List<string>();
        private List<object> _fields= new List<object>();

        public object Content { get; set; }
        public LogLevel Level { get; set; }
        public CallerInfo CallerInfo { get; set; }
        public DateTime Timestamp { get; set; }
        public IEnumerable<string> Tags { get { return _tags; } }
        public IEnumerable<object> AdditionalFields { get { return _fields; } }

        public void AddTag(string tag)
        {
            _tags.Add(tag);
        }

        public void AddField(object additionalFields)
        {
            _fields.Add(additionalFields);
        }
    }
}
