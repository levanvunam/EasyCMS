using System;

namespace Ez.Framework.Core.IoC.Attributes
{
    /// <summary>
    /// Register attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public sealed class RegisterAttribute : Attribute
    {

        public RegisterAttribute(Lifetime scopeLifetime)
        {
            ScopeLifetime = scopeLifetime;
        }

        public Lifetime ScopeLifetime { get; private set; }
    }
}
