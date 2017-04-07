using Ez.Framework.Core.Attributes;
using Ez.Framework.Utilities.Excel.Attributes;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Enums;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace EzCMS.Core.Models.Widgets
{
    public class WidgetSetupModel
    {
        public WidgetSetupModel()
        {
            WidgetType = WidgetType.EzCMSContent;
        }

        #region Public Properties

        #region Basic information

        public string Name { get; set; }

        public string Widget { get; set; }

        public string FullWidget { get; set; }

        public string Description { get; set; }

        public string DefaultTemplate { get; set; }

        #endregion

        [ExcelExport(Ignore = true)]
        public List<WidgetParameter> Parameters { get; set; }

        [ExcelExport(Ignore = true)]
        public Type Type { get; set; }

        [ExcelExport(Ignore = true)]
        public Type ManageType { get; set; }

        #endregion

        #region Builder

        public bool IsFavourite { get; set; }

        public string Image
        {
            get
            {
                var path = string.Format(EzCMSContants.WidgetImagePathFormat, Widget);
                var physicalPath = HttpContext.Current.Server.MapPath(path);
                if (!File.Exists(physicalPath))
                {
                    return EzCMSContants.NoWidget;
                }
                return path;
            }
        }

        public bool CanGenerate
        {
            get
            {
                return ManageType != null;
            }
        }

        public WidgetType WidgetType { get; set; }

        #endregion
    }

    public class WidgetParameter
    {
        public string Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [DefaultOrder]
        public int Order { get; set; }
    }
}
