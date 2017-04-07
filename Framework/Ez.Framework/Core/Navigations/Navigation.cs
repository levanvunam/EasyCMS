namespace Ez.Framework.Core.Navigations
{
    public class Navigation
    {
        #region Public Properties

        public string Id { get; set; }

        public string ParentId { get; set; }

        public string Hierarchy { get; set; }

        public string Permissions { get; set; }

        public string Name { get; set; }

        public string Icon { get; set; }

        public string Url { get; set; }

        public string Controller { get; set; }

        public string Action { get; set; }

        public int Order { get; set; }

        public bool Visible { get; set; }

        #endregion
    }
}
