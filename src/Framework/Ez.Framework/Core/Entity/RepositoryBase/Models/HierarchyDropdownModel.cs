namespace Ez.Framework.Core.Entity.RepositoryBase.Models
{
    public class HierarchyDropdownModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Hierarchy { get; set; }

        public int? RecordOrder { get; set; }

        public bool Selected { get; set; }
    }
}
