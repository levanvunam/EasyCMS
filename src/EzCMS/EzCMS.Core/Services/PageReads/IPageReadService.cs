using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.PageReads;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.PageReads
{
    [Register(Lifetime.PerInstance)]
    public interface IPageReadService : IBaseService<PageRead>
    {
        #region Grid Search

        /// <summary>
        /// Search the page reads
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchPageReads(JqSearchIn si, PageReadSearchModel model);

        /// <summary>
        /// Export the page reads
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, PageReadSearchModel model);

        #endregion
    }
}