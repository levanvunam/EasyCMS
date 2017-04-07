using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.AnonymousContacts;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.AnonymousContacts
{
    [Register(Lifetime.PerInstance)]
    public interface IAnonymousContactService : IBaseService<AnonymousContact>
    {
        #region Grid Search

        /// <summary>
        /// Search the anonymous contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchAnonymousContacts(JqSearchIn si, AnonymousContactSearchModel model);

        /// <summary>
        /// Export the anonymous contacts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, AnonymousContactSearchModel model);

        #endregion
    }
}