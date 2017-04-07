using Ez.Framework.Core.Attributes;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.AssociateTypes;
using EzCMS.Core.Services.CompanyTypes;
using EzCMS.Core.Services.Locations;
using EzCMS.Entity.Core.Enums;
using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Associates.WidgetManageModels
{
    public class AssociateListingWidgetManageModel : WidgetManageModel
    {
        #region Constructors

        public AssociateListingWidgetManageModel()
        {
        }

        public AssociateListingWidgetManageModel(string[] parameters)
            : base(parameters)
        {
            var associateTypeService = HostContainer.GetInstance<IAssociateTypeService>();
            var companyTypeService = HostContainer.GetInstance<ICompanyTypeService>();
            var locationService = HostContainer.GetInstance<ILocationService>();
            AssociateTypes = associateTypeService.GetAssociateTypes();
            CompanyTypes = companyTypeService.GetCompanyTypes();
            StatusList = EnumUtilities.GenerateSelectListItems<AssociateEnums.AssociateStatus>();
            Locations = locationService.GetLocations();

            ParseParams(parameters);
        }

        #endregion

        #region Parse Params

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

            if (parameters.Length > 4)
            {
                CompanyTypeId = parameters[4].ToInt(0);
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

        #region Public Properties

        [LocalizedDisplayName("Widget_Associates_Field_AssociateTypeId")]
        public int? AssociateTypeId { get; set; }

        public IEnumerable<SelectListItem> AssociateTypes { get; set; }

        [LocalizedDisplayName("Widget_Associates_Field_LocationId")]
        public int? LocationId { get; set; }

        public IEnumerable<SelectListItem> Locations { get; set; }

        [LocalizedDisplayName("Widget_Associates_Field_UsingCurrentUserCompanyType")]
        public bool UsingCurrentUserCompanyType { get; set; }

        [LocalizedDisplayName("Widget_Associates_Field_CompanyTypeId")]
        public int? CompanyTypeId { get; set; }

        public IEnumerable<SelectListItem> CompanyTypes { get; set; }

        [LocalizedDisplayName("Widget_Associates_Field_Status")]
        public int Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        #endregion
    }
}
