using System.Diagnostics;

namespace WebApi.Filters
{
    public interface ILogger
    {
        void Write(string message, params object[] args);
    }

    public class Logger : ILogger
    {
        public void Write(string message, params object[] args) {
            Debug.WriteLine(message, args);
        }
    }
}