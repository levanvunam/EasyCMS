using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.CompanyTypes
{
    public class CompanyTypeManageModel
    {
        #region Constructors

        public CompanyTypeManageModel()
        {
        }

        public CompanyTypeManageModel(CompanyType companyType)
            : this()
        {
            Id = companyType.Id;
            Name = companyType.Name;
            Description = companyType.Description;
            RecordOrder = companyType.RecordOrder;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("CompanyType_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("CompanyType_Field_Description")]
        public string Description { get; set; }

        [LocalizedDisplayName("CompanyType_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
