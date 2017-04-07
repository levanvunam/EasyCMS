using Ez.Framework.Core.Attributes;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.SubscriptionTemplates
{
    public class SubscriptionTemplateManageModel
    {
        #region Constructors
        public SubscriptionTemplateManageModel()
        {
        }

        public SubscriptionTemplateManageModel(SubscriptionTemplate subscriptionTemplate)
            : this()
        {
            Id = subscriptionTemplate.Id;
            Name = subscriptionTemplate.Name;
            Body = subscriptionTemplate.Body;
            Module = subscriptionTemplate.Module;
        }

        #endregion

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("SubscriptionTemplate_Field_Name")]
        public string Name { get; set; }

        [Required]
        [LocalizedDisplayName("SubscriptionTemplate_Field_Body")]
        public string Body { get; set; }

        [LocalizedDisplayName("SubscriptionTemplate_Field_Module")]
        public SubscriptionEnums.SubscriptionModule Module { get; set; }

        #endregion
    }
}
