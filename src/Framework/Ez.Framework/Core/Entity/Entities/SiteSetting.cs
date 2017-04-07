using Ez.Framework.Core.Entity.Enums;
using Ez.Framework.Core.Entity.Models;
using System.ComponentModel.DataAnnotations;

namespace Ez.Framework.Core.Entity.Entities
{
    public class SiteSetting : BaseModel
    {
        [StringLength(512)]
        public string Name { get; set; }

        public string Description { get; set; }

        public string FieldName { get; set; }

        public string Value { get; set; }

        public SettingEnums.EditorType EditorType { get; set; }

        public string SettingType { get; set; }
    }
}