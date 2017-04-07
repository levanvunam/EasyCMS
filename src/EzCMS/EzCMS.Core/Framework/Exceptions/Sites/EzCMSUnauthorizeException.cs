using System;

namespace EzCMS.Core.Framework.Exceptions.Sites
{
    public class EzCMSUnauthorizeException : Exception
    {
        public EzCMSUnauthorizeException(): base(string.Empty)
        {
        }

        public EzCMSUnauthorizeException(string message) : base(message)
        {
            
        }
    }
}