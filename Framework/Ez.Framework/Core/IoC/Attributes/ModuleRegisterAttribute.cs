using System;
using Ez.Framework.Core.IoC.Attributes;

namespace Ez.Framework.IoC.Attributes
{
    /// <summary>
    /// Module register attribute
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface, Inherited = true, AllowMultiple = false)]
    public sealed class ModuleRegisterAttribute : Attribute
    {

        public ModuleRegisterAttribute(Lifetime scopeLifetime)
        {
            ScopeLifetime = scopeLifetime;
        }

        public Lifetime ScopeLifetime { get; private set; }
    }
}
