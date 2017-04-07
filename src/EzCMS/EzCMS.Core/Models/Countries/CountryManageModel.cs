using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Countries
{
    public class CountryManageModel
    {
        #region Constructors

        public CountryManageModel()
        {
        }

        public CountryManageModel(Country country)
            : this()
        {
            Id = country.Id;
            Name = country.Name;
            RecordOrder = country.RecordOrder;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [LocalizedDisplayName("Country_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("Country_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion
    }
}
