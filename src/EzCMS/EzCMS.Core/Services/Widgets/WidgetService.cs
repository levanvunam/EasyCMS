using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Extensions;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.Utilities.Reflection.Enums;
using Ez.Framework.Utilities.Reflection.Models;
using EzCMS.Core.Core.SiteSettings;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Framework.Widgets.Attributes;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Models.Widgets.WidgetBuilders;
using EzCMS.Core.Services.Widgets.WidgetResolver;
using EzCMS.Core.Services.SiteSettings;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Widgets
{
    public class WidgetService : ServiceHelper, IWidgetService
    {
        private readonly ISiteSettingService _siteSettingService;

        public WidgetService()
        {
            _siteSettingService = HostContainer.GetInstance<ISiteSettingService>();
        }

        /// <summary>
        /// Replace all widget in the content
        /// </summary>
        /// <param name="content"></param>
        /// <param name="maxLoop"></param>
        /// <returns></returns>
        public string Render(string content, int? maxLoop = null)
        {
            if (!maxLoop.HasValue)
                maxLoop = _siteSettingService.GetSetting<int>(SettingNames.WidgetMaxLoop);
            return content.ResolveContent(maxLoop.Value);
        }

        #region Validation

        /// <summary>
        /// Check if page template content is valid or not
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool IsPageTemplateValid(string content)
        {
            //Not contains render body widget
            if (content.Contains(EzCMSContants.RenderBody))
            {
                return false;
            }

            //Contains more than 1 render body widget
            if (content.LastIndexOf(EzCMSContants.RenderBody, StringComparison.Ordinal) !=
                content.IndexOf(EzCMSContants.RenderBody, StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Search widgets
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        private IEnumerable<WidgetSetupModel> SearchWidgets(WidgetSearchModel search)
        {
            return GetAllWidgets().Where(c => string.IsNullOrEmpty(search.Keyword)
                                                    ||
                                                    c.Name.Contains(search.Keyword,
                                                        StringComparison.CurrentCultureIgnoreCase)
                                                    ||
                                                    (!string.IsNullOrEmpty(c.Widget) &&
                                                     c.Widget.Contains(search.Keyword,
                                                         StringComparison.CurrentCultureIgnoreCase))
                                                    ||
                                                    (!string.IsNullOrEmpty(c.FullWidget) &&
                                                     c.FullWidget.Contains(search.Keyword,
                                                         StringComparison.CurrentCultureIgnoreCase))
                                                    ||
                                                    (!string.IsNullOrEmpty(c.DefaultTemplate) &&
                                                     c.DefaultTemplate.Contains(search.Keyword,
                                                         StringComparison.CurrentCultureIgnoreCase))
                                                    ||
                                                    (!string.IsNullOrEmpty(c.Description) &&
                                                     c.Description.Contains(search.Keyword,
                                                         StringComparison.CurrentCultureIgnoreCase)));
        }

        #region Grid

        /// <summary>
        /// Search widgets
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchWidgets(JqSearchIn si, WidgetSearchModel model)
        {
            var widgets = SearchWidgets(model).AsQueryable();

            return si.Search(widgets);
        }

        /// <summary>
        /// Search the widget parameters
        /// </summary>
        /// <param name="si"></param>
        /// <param name="widget"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchWidgetParameters(JqSearchIn si, string widget)
        {
            var data =
                GetAllWidgets()
                    .FirstOrDefault(c => c.Widget.Equals(widget, StringComparison.CurrentCultureIgnoreCase));

            if (data != null)
            {
                return si.Search(data.Parameters.AsQueryable());
            }
            return si.Search(new List<WidgetParameter>().AsQueryable());
        }

        /// <summary>
        /// Export the widgets
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, WidgetSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAllWidgets() : SearchWidgets(model);

            var widgets = data.AsQueryable();

            var exportData = si.Export(widgets, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #endregion

        #region Get widgets

        /// <summary>
        /// Get all widget in the system
        /// </summary>
        /// <returns></returns>
        public List<WidgetSetupModel> GetAllWidgets(bool useCache = true)
        {
            if (!useCache || WorkContext.Widgets == null)
            {
                var widgetResolvers = HostContainer.GetInstances<IWidgetResolver>().ToList();

                var widgets = new List<WidgetSetupModel>();

                foreach (var widgetResolver in widgetResolvers.ToList())
                {
                    var widget = widgetResolver.GetSetup();
                    var type = widgetResolver.GetType();

                    // Get all available parameters of template
                    var parameters = type.GetProperties()
                        .Where(p => p.GetAttribute<WidgetParamAttribute>() != null)
                        .ToList();

                    widget.Parameters = parameters.Select(p => new WidgetParameter
                    {
                        Name = p.GetAttribute<WidgetParamAttribute>().Name,
                        Description = p.GetAttribute<WidgetParamAttribute>().Description,
                        Order = p.GetAttribute<WidgetParamAttribute>().Order,
                        Type = p.PropertyType.GetTypeName()
                    }).ToList();

                    // Generate full widget from widget name and parameters
                    widget.FullWidget = GenerateFullWidget(widget.Widget,
                        widget.Parameters.Select(p => p.Name).ToList());

                    widgets.Add(widget);
                }

                WorkContext.Widgets = widgets.OrderBy(c => c.Name).ToList();
            }
            return WorkContext.Widgets;
        }

        /// <summary>
        /// Generate the widget string
        /// </summary>
        /// <param name="widget"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private string GenerateFullWidget(string widget, List<string> parameters)
        {
            var parameterString = string.Empty;
            if (parameters.Any())
            {
                parameterString = string.Format("_{0}", string.Join("_", parameters));
            }
            return string.Format("{{{0}{1}}}", widget, parameterString);
        }

        #endregion

        #region Build Properties Dropdown

        /// <summary>
        /// Get property list from widget
        /// </summary>
        /// <param name="widgetName"></param>
        /// <returns></returns>
        public List<PropertyModel> GetPropertyListFromWidget(string widgetName)
        {
            var widget =
                GetAllWidgets()
                    .FirstOrDefault(
                        c => c.Widget.Equals(widgetName, StringComparison.CurrentCultureIgnoreCase));
            if (widget != null)
                return widget.Type.GetPropertyListFromType(EzCMSContants.MaxPropertyLoop, "Model");

            return new List<PropertyModel>();
        }

        /// <summary>
        /// Get property list from widget
        /// </summary>
        /// <param name="widgetName"></param>
        /// <returns></returns>
        public List<string> GetPropertyValueListFromWidget(string widgetName)
        {
            var widget = GetAllWidgets()
                .FirstOrDefault(c => c.Widget.Equals(widgetName, StringComparison.CurrentCultureIgnoreCase));
            if (widget != null)
                return
                    widget.Type.GetPropertyListFromType(1, string.Empty, PropertyKind.Value)
                        .Select(p => p.Name)
                        .ToList();

            return new List<string>();
        }

        /// <summary>
        /// Get property list from type
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public List<PropertyModel> GetPropertyListFromType(string typeName)
        {
            var type = Type.GetType(typeName);
            return type.GetPropertyListFromType(3, "Model");
        }

        /// <summary>
        /// Generate template
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public ResponseModel GeneratePropertyTemplate(string typeName, string name)
        {
            try
            {
                var template = ReflectionUtilities.GeneratePropertyTemplate(typeName, name);

                return new ResponseModel
                {
                    Success = true,
                    Data = template
                };
            }
            catch (Exception exception)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("System_Message_InvalidType"),
                    DetailMessage = exception.BuildErrorMessage()
                };
            }
        }

        #endregion

        #region Widget builder

        /// <summary>
        /// Get favourite widgets
        /// </summary>
        /// <returns></returns>
        public IEnumerable<WidgetSetupModel> GetFavouriteWidgets()
        {
            return GetAllWidgets().Where(c => c.IsFavourite);
        }

        /// <summary>
        /// Get widget dropdown
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public WidgetDropdownModel GetWidgetDropdownModel(string callback)
        {
            return new WidgetDropdownModel(callback);
        }

        /// <summary>
        /// Get select widget model
        /// </summary>
        /// <returns></returns>
        public SelectWidgetModel GetSelectWidgetModel()
        {
            return new SelectWidgetModel();
        }

        /// <summary>
        /// Get manage model from widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public dynamic GetWidgetManageModel(string widget)
        {
            var widgetRenderingParams = widget.ParseParams();

            var widgetManageType = GetWidgetManageType(widgetRenderingParams.FirstOrDefault());
            if (widgetManageType != null)
            {
                var manageModel = FastActivator<string[]>.CreateInstance(widgetManageType, widgetRenderingParams);
                return manageModel;
            }

            return null;
        }

        /// <summary>
        /// Get preview model for widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        public WidgetPreviewModel GetPreviewModel(string widget)
        {
            return new WidgetPreviewModel(widget);
        }

        #region Private Methods

        /// <summary>
        /// Get the manage type for widget
        /// </summary>
        /// <param name="widgetName"></param>
        /// <returns></returns>
        private Type GetWidgetManageType(string widgetName)
        {
            var widget = GetWidgetModel(widgetName);

            if (widget != null)
            {
                return widget.ManageType;
            }

            return null;
        }

        /// <summary>
        /// Get widget
        /// </summary>
        /// <param name="widget"></param>
        /// <returns></returns>
        private WidgetSetupModel GetWidgetModel(string widget)
        {
            if (string.IsNullOrEmpty(widget)) return null;

            return WorkContext.Widgets.FirstOrDefault(c => c.Widget.ToLower().Equals(widget.ToLower()));
        }

        #endregion

        #endregion
    }
}