using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;

namespace SimpleStructuredLoggingLibrary.Formatting.Logstash
{
    public static class LogstashJson
    {
        public static FormattedLogEvent Format(LogEvent logEvent)
        {
            var requiredFields = new Dictionary<string, object>()
            {
                { "@version", 1 },
                { "@timestamp", logEvent.Timestamp.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture) }
            };

            var jsonSerializer = new JsonSerializer() { };
            jsonSerializer.Converters.Add(new StringEnumConverter() { AllowIntegerValues = false });

            var json = JObject.FromObject(requiredFields, jsonSerializer);
            json.Merge(JObject.FromObject(logEvent.Content, jsonSerializer));

            foreach (var fields in logEvent.AdditionalFields)
            {
                json.Merge(JObject.FromObject(fields, jsonSerializer));
            }
            
            return new FormattedLogEvent() { Content = JsonConvert.SerializeObject(json) };
        }

        public static IObservable<LogEvent> AddTagsToAdditionalFields(this IObservable<LogEvent> logEvents)
        {
            return logEvents.Do(logEvent => logEvent.AddField(new { tags = logEvent.Tags }));
        }

        public static IObservable<LogEvent> AddCallerInfoToAdditionalFields(this IObservable<LogEvent> logEvents)
        {
            return logEvents.Do(logEvent => logEvent.AddField(new { CallerInfo = logEvent.CallerInfo }));
        }

        public static IObservable<LogEvent> AddLevelToAdditionalFields(this IObservable<LogEvent> logEvents)
        {
            return logEvents.Do(logEvent => logEvent.AddField(new { Level = logEvent.Level }));
        }

        public static IObservable<LogEvent> AddLevelToTags(this IObservable<LogEvent> logEvents)
        {
            return logEvents.Do(logEvent => logEvent.AddTag(logEvent.Level.ToString()));
        }
    }
}
