using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Shared.Forms
{
    public class FormComponentFieldAttributeModel
    {
        #region Constructors

        public FormComponentFieldAttributeModel()
        {
            Types = EnumUtilities.GenerateSelectListItems<ForNavigationms.FormType>(GenerateEnumType.DescriptionValueAndDescriptionText);
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("FormComponentField_Field_Label")]
        public string label { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_Type")]
        public string type { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> Types { get; set; }

        public object value { get; set; }

        [LocalizedDisplayName("FormComponentField_Field_Value")]
        [ScriptIgnore]
        public string ValueString
        {
            get
            {
                if (value == null)
                {
                    return string.Empty;
                }

                if (value is string)
                {
                    return value.ToString();
                }

                return SerializeUtilities.Serialize(value);
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    this.value = string.Empty;
                }
                else
                {
                    this.value = SerializeUtilities.Deserialize<object>(value) ?? value;
                }
            }
        }

        #endregion
    }
}
