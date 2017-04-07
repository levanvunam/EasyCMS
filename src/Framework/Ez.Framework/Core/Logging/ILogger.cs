using System;

namespace Ez.Framework.Core.Logging
{
    /// <summary>
    /// Logger
    /// </summary>
    public interface ILogger
    {
        //Info log
        void Info(string message);

        //Warning log
        void Warn(string message);
        void Warn(Exception exception);
        void Warn(string message, Exception exception);

        //Debug log
        void Debug(string message);

        //Error log
        void Error(string message);
        void Error(Exception exception);
        void Error(string message, Exception exception);

        //Fatal log
        void Fatal(string message);
        void Fatal(Exception exception);
        void Fatal(string message, Exception exception);
    }
}
