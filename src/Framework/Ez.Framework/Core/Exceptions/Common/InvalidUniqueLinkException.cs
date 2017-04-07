using System;

namespace Ez.Framework.Core.Exceptions.Common
{
    public class InvalidUniqueLinkException : Exception
    {
        public InvalidUniqueLinkException()
        {

        }

        public InvalidUniqueLinkException(string message)
            : base(message)
        {

        }
    }
}