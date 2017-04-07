using System;

namespace EzCMS.Core.Framework.Exceptions.Sites
{
    public class EzCMSNotFoundException : Exception
    {
        public EzCMSNotFoundException()
        {
            
        }

        public EzCMSNotFoundException(string url, string message = null)
            : base(message)
        {
            Url = url;
        }

        #region Public Properties

        public string Url { get; set; }

        #endregion
    }
}