using System;
using System.Collections.Generic;
using System.Linq;
using Ez.Framework.Core.Context;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Plugins;
using EzCMS.Entity.Core.SiteInitialize;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Plugins
{
    public class PluginService : ServiceHelper, IPluginService
    {
        #region Base

        /// <summary>
        /// Get all plugins
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PluginInformationModel> GetAll()
        {
            return PluginHelper.GetPlugins();
        }

        /// <summary>
        /// Get plugin by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PluginInformationModel GetByName(string name)
        {
            return GetAll().FirstOrDefault(m => m.Name.Equals(name));
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the Plugins.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchPlugins(JqSearchIn si)
        {
            var plugins = GetAll();

            return si.Search(plugins.AsQueryable());
        }

        /// <summary>
        /// Export Plugins
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var plugins = GetAll().AsQueryable();

            var exportData = si.Export(plugins, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get Plugin manage model for creating / editing
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public PluginInformationModel GetPluginManageModel(string name)
        {
            var plugin = GetByName(name);
            return plugin;
        }

        /// <summary>
        /// Save Plugin
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SavePluginManageModel(PluginInformationModel model)
        {
            try
            {
                var siteConfig = SiteInitializer.GetConfiguration();

                var plugin = siteConfig.Plugins.FirstOrDefault(m => m.Name.Equals(model.Name));
                if (plugin == null)
                {
                    if (model.IsInstalled)
                    {
                        var pluginInformation = GetByName(model.Name);
                        plugin = new Plugin
                        {
                            Name = pluginInformation.Name,
                            ConnectionString = model.ConnectionString,
                            Folder = pluginInformation.Folder
                        };

                        siteConfig.Plugins.Add(plugin);
                        EzWorkContext.IsSystemChanged = true;
                    }
                }
                else
                {
                    if (model.IsInstalled)
                    {
                        plugin.ConnectionString = model.ConnectionString;
                    }
                    else
                    {
                        siteConfig.Plugins.Remove(plugin);
                    }

                    EzWorkContext.IsSystemChanged = true;
                }

                SiteInitializer.SaveConfiguration(siteConfig, false);
            }
            catch (Exception exception)
            {
                EzWorkContext.IsSystemChanged = false;
                return new ResponseModel
                {
                    Success = true,
                    Message = T("Plugin_Message_UpdateFailure")
                };
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Plugin_Message_UpdateSuccessfully")
            };
        }

        #endregion
    }
}