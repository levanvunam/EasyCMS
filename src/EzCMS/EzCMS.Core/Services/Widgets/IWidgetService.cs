using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Utilities.Reflection.Models;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Widgets
{
    [Register(Lifetime.PerInstance)]
    public interface IWidgetService
    {
        /// <summary>
        /// Replace all widgets in content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="maxLoop"></param>
        /// <returns></returns>
        string Render(string content, int? maxLoop = null);

        #region Validation

        bool IsPageTemplateValid(string content);

        #endregion

        /// <summary>
        /// Get all widgets
        /// </summary>
        /// <param name="useCache"></param>
        /// <returns></returns>
        List<WidgetSetupModel> GetAllWidgets(bool useCache = true);

        #region Grid

        /// <summary>
        /// Search widgets
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchWidgets(JqSearchIn si, WidgetSearchModel model);

        /// <summary>
        /// Search widget parameters
        /// </summary>
        /// <param name="si"></param>
        /// <param name="widget"></param>
        /// <returns></returns>
        JqGridSearchOut SearchWidgetParameters(JqSearchIn si, string widget);

        /// <summary>
        /// Export widgets
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, WidgetSearchModel model);

        #endregion

        #region Property dropdown

        /// <summary>
        /// Generate property template
        /// </summary>
        /// <param name="type"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        ResponseModel GeneratePropertyTemplate(string type, string name);

        /// <summary>
        /// Get property list from widget
        /// </summary>
        /// <param name="widgetName"></param>
        /// <returns></returns>
        List<PropertyModel> GetPropertyListFromWidget(string widgetName);

        /// <summary>
        /// Get property list from widget
        /// </summary>
        /// <param name="widgetName"></param>
        /// <returns></returns>
        List<string> GetPropertyValueListFromWidget(string widgetName);

        /// <summary>
        /// Get property list from type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        List<PropertyModel> GetPropertyListFromType(string type);

        #endregion

        #region Widget Builder

        /// <summary>
        /// Get favourite widgets
        /// </summary>
        /// <returns></returns>
        IEnumerable<WidgetSetupModel> GetFavouriteWidgets();

        #region Select Widget

        /// <summary>
        /// Get widget dropdown
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        WidgetDropdownModel GetWidgetDropdownModel(string callback);

        /// <summary>
        /// Get select widget model
        /// </summary>
        /// <returns></returns>
        SelectWidgetModel GetSelectWidgetModel();

        #endregion

        #region Configure Widget

        /// <summary>
        /// Get widget manage model
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        dynamic GetWidgetManageModel(string widget);

        /// <summary>
        /// Get preview model for widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        WidgetPreviewModel GetPreviewModel(string widget);

        #endregion

        #endregion
    }
}