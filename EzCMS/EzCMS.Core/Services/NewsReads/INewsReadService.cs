using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.NewsReads;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.NewsReads
{
    [Register(Lifetime.PerInstance)]
    public interface INewsReadService : IBaseService<NewsRead>
    {
        #region Grid Search

        /// <summary>
        /// Search the news reads
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchNewsReads(JqSearchIn si, NewsReadSearchModel model);

        /// <summary>
        /// Export the news reads
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, NewsReadSearchModel model);

        #endregion
    }
}