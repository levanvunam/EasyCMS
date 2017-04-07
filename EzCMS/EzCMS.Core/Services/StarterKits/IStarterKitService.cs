using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.StarterKits
{
    public interface IStarterKitService
    {
        /// <summary>
        /// Insert page
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        ResponseModel InsertPage(Page page);

        /// <summary>
        /// Insert page template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        ResponseModel InsertPageTemplate(PageTemplate template);

        /// <summary>
        /// Insert script
        /// </summary>
        /// <param name="script"></param>
        /// <returns></returns>
        ResponseModel InsertScript(Script script);

        /// <summary>
        /// Insert style
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        ResponseModel InsertStyle(Style style);

        /// <summary>
        /// Insert widget template
        /// </summary>
        /// <param name="template"></param>
        /// <returns></returns>
        ResponseModel InsertWidgetTemplate(WidgetTemplate template);
    }
}