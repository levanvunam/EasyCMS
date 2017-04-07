using Ez.Framework.Utilities;
using log4net;
using System;

namespace Ez.Framework.Core.Logging
{
    /// <summary>
    /// General implement of ILogger
    /// </summary>
    public class EzLogger : ILogger
    {
        private readonly ILog _logger;

        public EzLogger(Type type)
        {
            _logger = LogManager.GetLogger(type);
        }

        #region Info Log

        public void Info(string message)
        {
            _logger.Info(message);
        }

        #endregion

        #region Warning Log

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Warn(Exception exception)
        {
            Warn(exception.Message, exception);
        }

        public void Warn(string message, Exception exception)
        {
            _logger.Warn(message, exception);
        }

        #endregion

        #region Debug Log

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        #endregion

        #region Error Log

        public void Error(string message)
        {
            message = message.BuildMessageBody();
            _logger.Error(message);
        }

        public void Error(Exception exception)
        {
            Error(exception.Message, exception);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(message, exception);
        }

        #endregion

        #region Fatal Log

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(Exception exception)
        {
            Fatal(exception.Message, exception);
        }

        public void Fatal(string message, Exception exception)
        {
            Fatal(exception.BuildFullExceptionMessage());
        }

        #endregion
    }
}
