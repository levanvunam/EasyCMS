using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Links;
using EzCMS.Core.Models.Links.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Links
{
    [Register(Lifetime.PerInstance)]
    public interface ILinkService : IBaseService<Link>
    {
        #region Widget

        LinksWidget GetLinksWidget(int linkTypeId, int number);

        #endregion

        LinkDetailModel GetLinkDetailModel(int id);

        ResponseModel UpdateLinkData(XEditableModel model);

        #region Grid Search

        /// <summary>
        /// Search the links
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchLinks(JqSearchIn si, LinkSearchModel model);

        /// <summary>
        /// Export the links
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, LinkSearchModel model);

        #endregion

        #region Manage

        LinkManageModel GetLinkManageModel(int? id = null, int? linkTypeId = null);

        ResponseModel SaveLink(LinkManageModel model);

        ResponseModel DeleteLink(int id);

        #endregion
    }
}