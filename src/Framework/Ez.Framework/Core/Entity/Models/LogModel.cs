using System;

namespace Ez.Framework.Core.Entity.Models
{
    public class LogModel
    {
        #region Constructors

        public LogModel()
        {

        }

        public LogModel(string message)
            : this()
        {
            Time = DateTime.UtcNow;
            Message = message;
        }

        #endregion

        #region Public Properties

        public DateTime Time { get; set; }

        public string Message { get; set; }

        #endregion
    }
}
