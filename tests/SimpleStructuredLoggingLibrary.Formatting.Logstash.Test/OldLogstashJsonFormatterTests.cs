﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Reactive.Linq;
using Newtonsoft.Json.Linq;
using System.Runtime.CompilerServices;

namespace SimpleStructuredLoggingLibrary.Formatting.Logstash.Test
{
    public class OldLogstashJsonFormatterTests
    {
        [Fact]
        public void should_add_timestamp_in_correct_format()
        {
            string expectedTimestamp = "2015-12-07T18:05:22.352Z";
            var timestamp = DateTime.Parse(expectedTimestamp, CultureInfo.InvariantCulture);

            var output = new List<FormattedLogEvent>();

            Logger logger = new Logger(logEvents => logEvents
                .AddTimestamp(timestamp)
                .Select(OldLogstashJson.Format)
                .Subscribe(log => output.Add(log))
                );

            logger.Info(new { Val = 1 });

            JObject parsed = GetJObject(output);

            Assert.Equal(expectedTimestamp, parsed.Value<string>("@timestamp"));
        }

        [Fact]
        public void should_add_fields_from_message()
        {
            var message = new { Val = 1, Complex = new { Other = "string" } };

            var output = new List<FormattedLogEvent>();

            Logger logger = new Logger(logEvents => logEvents
                .Select(OldLogstashJson.Format)
                .Subscribe(log => output.Add(log))
                );

            logger.Info(message);

            JObject parsed = GetJObject(output);

            Assert.Equal(1, parsed.Value<JObject>("@fields").Value<int>("Val"));
            Assert.Equal("string", parsed.Value<JObject>("@fields").Value<JObject>("Complex").Value<string>("Other"));
        }

        [Fact]
        public void should_add_additional_fields_to_message()
        {
            var message = new { Val = 1 };

            var output = new List<FormattedLogEvent>();

            Logger logger = new Logger(logEvents => logEvents
                .AddFields(new { MyField = 34 })
                .AddFields(new { OtherField = "Hello" })
                .Select(OldLogstashJson.Format)
                .Subscribe(log => output.Add(log))
                );

            logger.Info(message);

            JObject parsed = GetJObject(output);

            Assert.Equal(34, parsed.Value<JObject>("@fields").Value<int>("MyField"));
            Assert.Equal("Hello", parsed.Value<JObject>("@fields").Value<string>("OtherField"));
        }

        [Fact]
        public void should_add_tags_to_message()
        {
            var message = new { Val = 1 };

            var output = new List<FormattedLogEvent>();

            Logger logger = new Logger(logEvents => logEvents
                .AddTag("mytag")
                .AddTag("mytag2")
                .Select(OldLogstashJson.Format)
                .Subscribe(log => output.Add(log))
                );

            logger.Info(message);

            JObject parsed = GetJObject(output);

            Assert.Equal(new[] { "mytag", "mytag2" }, parsed.Value<JArray>("@tags").ToObject<string[]>());
        }

        [Fact]
        public void should_add_callerinfo_to_message()
        {
            var message = new { Val = 1 };

            var output = new List<FormattedLogEvent>();

            Logger logger = new Logger(logEvents => logEvents
                .AddCallerInfoToAdditionalFields()
                .Select(OldLogstashJson.Format)
                .Subscribe(log => output.Add(log))
                );
            var callerFilePath = GetCallerFilePath();

            logger.Info(message);

            JObject parsed = GetJObject(output);

            Assert.Equal(callerFilePath, parsed.Value<JObject>("@fields").Value<JObject>("CallerInfo").Value<string>("FilePath"));
        }

        [Fact]
        public void should_add_level_to_message()
        {
            var message = new { Val = 1 };

            var output = new List<FormattedLogEvent>();

            Logger logger = new Logger(logEvents => logEvents
                .AddLevelToAdditionalFields()
                .Select(OldLogstashJson.Format)
                .Subscribe(log => output.Add(log))
                );
            var callerFilePath = GetCallerFilePath();

            logger.Warn(message);

            JObject parsed = GetJObject(output);

            Assert.Equal("Warn", parsed.Value<JObject>("@fields").Value<string>("Level"));
        }

        [Fact]
        public void should_add_level_to_tags()
        {
            var message = new { Val = 1 };

            var output = new List<FormattedLogEvent>();

            Logger logger = new Logger(logEvents => logEvents
                .AddLevelToTags()
                .AddTagsToAdditionalFields()
                .Select(OldLogstashJson.Format)
                .Subscribe(log => output.Add(log))
                );
            var callerFilePath = GetCallerFilePath();

            logger.Warn(message);

            JObject parsed = GetJObject(output);

            Assert.Contains("Warn", parsed.Value<JArray>("@tags").ToObject<string[]>());
        }

        private static JObject GetJObject(List<FormattedLogEvent> output)
        {
            var logOutput = output.First().Content;

            var parsed = (JObject)JsonConvert.DeserializeObject(logOutput, new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });
            return parsed;
        }

        private string GetCallerFilePath([CallerFilePath]string callerFilePath = "Shouldn't Get This")
        {
            return callerFilePath;
        }
    }
}