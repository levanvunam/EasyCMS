using Ez.Framework.Models;

namespace EzCMS.Core.Models.Companies
{
    public class CompanyModel : BaseGridModel
    {
        #region Public Properties

        public string Name { get; set; }

        public int? CompanyTypeId { get; set; }

        public string CompanyTypeName { get; set; }

        #endregion
    }
}
