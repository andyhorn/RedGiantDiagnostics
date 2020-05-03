using System.Linq;
using API.Contracts.Requests;
using API.Entities;

namespace API
{
    public static class LogFileExtensions
    {
        public static LogFile Update<T>(this LogFile log, T update) where T : ILogUpdateRequest
        {
            var logFileType = typeof(LogFile);
            var logFileProperties = logFileType.GetProperties();

            var updateType = typeof(T);
            var updateProperties = updateType.GetProperties();

            foreach (var property in updateProperties)
            {
                if (logFileProperties.Contains(property))
                {
                    var value = property.GetValue(update);

                    if (value != null)
                    {
                        property.SetValue(log, value);
                    }
                }
            }

            return log;
        }
    }
}