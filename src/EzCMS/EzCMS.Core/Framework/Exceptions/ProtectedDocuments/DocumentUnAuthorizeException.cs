using System;

namespace EzCMS.Core.Framework.Exceptions.ProtectedDocuments
{
    public class DocumentUnauthorizeException : Exception
    {
        public DocumentUnauthorizeException()
        {
            
        }

        public DocumentUnauthorizeException(string message)
            : base(message)
        {
            
        }
    }
}