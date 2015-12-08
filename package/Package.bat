mkdir lib
mkdir lib\net46
mkdir tools
mkdir content
mkdir content\controllers

copy ..\src\SimpleStructuredLoggingLibrary\bin\Debug\SimpleStructuredLoggingLibrary.dll lib\net46
copy ..\src\SimpleStructuredLoggingLibrary.Formatting.Logstash\bin\Debug\SimpleStructuredLoggingLibrary.Formatting.Logstash.dll lib\net46

nuget pack
