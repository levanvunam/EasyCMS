using Ez.Framework.Core.Navigations.Attributes;

namespace EzCMS.Core.Framework.Navigations
{
    public class AdministratorNavigationAttribute : NavigationAttribute
    {
        private const string AdministratorPrefix = "Administrator_";
        private const string AdministratorResourcePrefix = "Navigation_Administrator_";

        public AdministratorNavigationAttribute()
        {

        }

        public AdministratorNavigationAttribute(string id, string parentId, string name, string icon, int order, bool visible = true, bool isCategory = false)
            : base("Administrator",
                  AdministratorPrefix + id,
                  !string.IsNullOrEmpty(parentId) ? AdministratorPrefix + parentId : "",
                  AdministratorResourcePrefix + name,
                  icon, order, visible, isCategory)
        {
        }
    }
}
