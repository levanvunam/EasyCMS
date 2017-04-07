using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Entity.Core.Enums;
using Ez.Framework.Utilities;

namespace EzCMS.Core.Models.Subscriptions.WidgetManageModels
{
    public class SubscriptionWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public SubscriptionWidgetManageModel()
        {
        }

        public SubscriptionWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            Modules = EnumUtilities.GenerateSelectListItems<SubscriptionEnums.SubscriptionModule>();

            ParseParams(parameters);
        }

        #endregion

        #region Parse Params


        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * Module
             * * Template 
             */

            //Module
            if (parameters.Length > 1)
            {
                Module = parameters[1].ToInt((int)SubscriptionEnums.SubscriptionModule.Page);
            }

            //Template 
            if (parameters.Length > 2)
            {
                Template = parameters[2];
            }
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("Widget_Subscription_Field_Module")]
        public int Module { get; set; }

        public IEnumerable<SelectListItem> Modules { get; set; } 

        #endregion
    }
}
