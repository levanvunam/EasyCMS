using System;

namespace Ez.Framework.Core.Navigations.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class NavigationAttribute : Attribute
    {
        public NavigationAttribute()
        {

        }

        public NavigationAttribute(string mode, string id, string parentId, string name, string icon, int order, bool visible, bool isCategory)
        {
            Mode = mode;
            Id = id;
            ParentId = parentId;
            Name = name;
            Icon = icon;
            Order = order;
            Visible = visible;
            IsCategory = isCategory;
        }

        #region Public Properties

        public string Id { get; set; }

        public string Mode { get; set; }

        public string ParentId { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public bool Visible { get; set; }

        public bool IsCategory { get; set; }

        public int Order { get; set; }

        #endregion
    }
}
