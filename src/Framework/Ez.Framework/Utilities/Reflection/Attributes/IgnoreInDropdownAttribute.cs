using System;

namespace Ez.Framework.Utilities.Reflection.Attributes
{
    public class IgnoreInDropdownAttribute : Attribute
    {
        public bool Ignore { get; set; }

        public IgnoreInDropdownAttribute()
        {
            Ignore = true;
        }

        public IgnoreInDropdownAttribute(bool ignore)
            : this()
        {
            Ignore = ignore;
        }
    }
}
