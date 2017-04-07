using System;

namespace EzCMS.Core.Framework.StarterKits.Attributes
{
    public class StarterKitAttribute : Attribute
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string ImageName { get; set; }
    }
}
