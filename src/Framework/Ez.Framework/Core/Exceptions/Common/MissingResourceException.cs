using System;

namespace Ez.Framework.Core.Exceptions.Common
{
    public class MissingResourceException : ArgumentException
    {
        private readonly string _key;
        public MissingResourceException()
        {
        }

        public MissingResourceException(string key)
        {
            _key = key;
        }

        public override string Message
        {
            get
            {
                if (string.IsNullOrEmpty(_key))
                    return "The resource is missing";
                return string.Format("The resource: {0} is missing.", _key);
            }
        }
    }
}
