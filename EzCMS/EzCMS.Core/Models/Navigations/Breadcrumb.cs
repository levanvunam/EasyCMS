using Ez.Framework.Core.Navigations;

namespace EzCMS.Core.Models.Navigations
{
    public class Breadcrumb
    {
        public Breadcrumb()
        {

        }

        public Breadcrumb(Navigation navigation)
            : this()
        {
            Name = navigation.Name;
            Url = navigation.Url;
            Action = navigation.Action;
            Controller = navigation.Controller;
            NavigationIcon = navigation.Icon;
        }

        #region Public Properties

        public string NavigationIcon { get; set; }

        public string Url { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public string Name { get; set; }

        #endregion
    }
}