using Ez.Framework.Core.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Framework.Widgets;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Models.Associates.WidgetManageModels;
using EzCMS.Core.Models.Associates.Widgets;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Services.Associates;
using EzCMS.Core.Services.WidgetTemplates;
using EzCMS.Entity.Core.Enums;
using System.Collections.Generic;
using System.Linq;

namespace EzCMS.Core.Core.WidgetResolvers.Associates
{
    public class AssociateListingResolver : Widget
    {
        #region Setup

        /// <summary>
        /// Get widget information
        /// </summary>
        /// <returns></returns>
        public override WidgetSetupModel GetSetup()
        {
            return new WidgetSetupModel
            {
                Name = "Associate Listing",
                Widget = "Associates",
                WidgetType = WidgetType.EzCMSContent,
                Description = "Get associate list and render using template",
                DefaultTemplate = "Default.Associates",
                Type = typeof(AssociateListingWidget),
                ManageType = typeof(AssociateListingWidgetManageModel),
                IsFavourite = true
            };
        }

        #endregion

        #region Private Properties

        private readonly IWidgetTemplateService _widgetTemplateService;
        private readonly IAssociateService _associateService;

        #endregion

        #region Constructors

        public AssociateListingResolver()
        {
            _associateService = HostContainer.GetInstance<IAssociateService>();
            _widgetTemplateService = HostContainer.GetInstance<IWidgetTemplateService>();
        }

        #endregion

        #region Parse Params

        [WidgetParam(Name = "AssociateTypeId", Order = 1, Description = "The associate type to load")]
        public int? AssociateTypeId { get; set; }

        [WidgetParam(Name = "LocationId", Order = 2, Description = "The location to load")]
        public int? LocationId { get; set; }

        [WidgetParam(Name = "UsingCurrentUserCompanyType", Order = 3, Description = "Use the current user company type")]
        public bool UsingCurrentUserCompanyType { get; set; }

        [WidgetParam(Name = "CompanyTypeId", Order = 4, Description = "The company type to load")]
        public int? CompanyTypeId { get; set; }

        [WidgetParam(Name = "Status", Order = 5, Description = "The status of current associate: All = 1, New = 2, NotNew = 3")]
        public int Status { get; set; }
        private AssociateEnums.AssociateStatus AssociateStatus
        {
            get
            {
                return Status.ToEnum<AssociateEnums.AssociateStatus>();
            }
        }

        [WidgetParam(Name = "TemplateName", Order = 6, Description = "The template name of Template that used for rendering, this is optional parameter, if not defined then get the default WidgetTemplate ")]
        public string Template { get; set; }

        private void ParseParams(string[] parameters)
        {
            /*
             * Params:
             * * AssociateTypeId
             * * LocationId
             * * UsingCurrentUserCompanyType
             * * CompanyTypeId
             * * Status
             */

            //AssociateTypeId
            if (parameters.Length > 1)
            {
                AssociateTypeId = parameters[1].ToNullableInt();
            }

            //LocationId
            if (parameters.Length > 2)
            {
                LocationId = parameters[2].ToNullableInt();
            }

            //UsingCurrentUserCompanyType
            if (parameters.Length > 3)
            {
                UsingCurrentUserCompanyType = parameters[3].ToBool(false);
            }

            //CompanyTypeId
            if (UsingCurrentUserCompanyType)
            {
                if (WorkContext.CurrentUser != null)
                {
                    CompanyTypeId = WorkContext.CurrentUser.CompanyTypeIds.FirstOrDefault();
                }
            }
            else if (parameters.Length > 4)
            {
                CompanyTypeId = parameters[4].ToNullableInt();
            }

            //Status
            if (parameters.Length > 5)
            {
                Status = parameters[5].ToInt((int)AssociateEnums.AssociateStatus.All);
            }

            //Template 
            if (parameters.Length > 6)
            {
                Template = parameters[6];
            }
        }

        #endregion

        #region Generate Widget

        /// <summary>
        /// Generate full widget from params
        /// </summary>
        /// <returns></returns>
        public override string GenerateFullWidget()
        {
            var parameters = new List<object>
            {
                GetSetup().Widget
            };

            if (!string.IsNullOrEmpty(Template))
            {
                parameters.AddRange(new List<object> { AssociateTypeId, LocationId, UsingCurrentUserCompanyType, CompanyTypeId, Status, Template });
            }
            else if (Status != (int)AssociateEnums.AssociateStatus.All)
            {
                parameters.AddRange(new List<object> { AssociateTypeId, LocationId, UsingCurrentUserCompanyType, CompanyTypeId, Status });
            }
            else if (CompanyTypeId.HasValue && CompanyTypeId > 0)
            {
                parameters.AddRange(new List<object> { AssociateTypeId, LocationId, UsingCurrentUserCompanyType, CompanyTypeId });
            }
            else if (UsingCurrentUserCompanyType)
            {
                parameters.AddRange(new List<object> { AssociateTypeId, LocationId, UsingCurrentUserCompanyType });
            }
            else if (LocationId.HasValue && LocationId > 0)
            {
                parameters.AddRange(new List<object> { AssociateTypeId, LocationId });
            }
            else if (AssociateTypeId.HasValue && AssociateTypeId > 0)
            {
                parameters.AddRange(new List<object> { AssociateTypeId });
            }

            return string.Join(EzCMSContants.WidgetSeparator, parameters).ToWidgetFormat();
        }

        #endregion

        /// <summary>
        /// Render News widget
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public override string Render(string[] parameters)
        {
            ParseParams(parameters);

            var model = _associateService.GetAssociateListingWidget(AssociateTypeId, LocationId, CompanyTypeId, AssociateStatus);

            var template = _widgetTemplateService.GetTemplateByName(Template) ??
                           _widgetTemplateService.GetTemplateByName(GetSetup().DefaultTemplate);

            return _widgetTemplateService.ParseTemplate(template, model);
        }
    }
}
