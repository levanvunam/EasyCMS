using System.Collections.Generic;
using Ez.Framework.Utilities.Reflection.Enums;

namespace Ez.Framework.Utilities.Reflection.Models
{
    public class PropertyModel
    {
        public PropertyModel()
        {
            Children = new List<PropertyModel>();
        }

        public string Name { get; set; }

        public string Value { get; set; }

        public string Type { get; set; }

        public PropertyKind Kind { get; set; }

        public List<PropertyModel> Children { get; set; }
    }
}