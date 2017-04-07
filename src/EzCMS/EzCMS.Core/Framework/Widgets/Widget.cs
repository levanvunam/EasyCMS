using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.Entity.Intialize;
using EzCMS.Core.Framework.Mvc.Helpers;
using EzCMS.Core.Models.Widgets;
using EzCMS.Core.Services.Widgets.WidgetResolver;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;
using System.Data.Entity;

namespace EzCMS.Core.Framework.Widgets
{
    public abstract class Widget : IWidgetResolver, IDataInitializer
    {
        public DataInitializerPriority Priority()
        {
            return DataInitializerPriority.Low;
        }

        /// <summary>
        /// Initialize template and default data
        /// </summary>
        public virtual void Initialize(DbContext ezContext)
        {
            var context = ezContext as EzCMSEntities;
            if (context != null)
            {
                // If do not defined setup configuration or empty default template then do nothing
                if (GetSetup() == null || string.IsNullOrEmpty(GetSetup().DefaultTemplate))
                {
                    return;
                }

                var template = new WidgetTemplate
                {
                    Name = GetSetup().DefaultTemplate,
                    DataType = GetSetup().Type.AssemblyQualifiedName,
                    Widget = GetSetup().Widget,
                    Widgets = DataInitializeHelper.GetTemplateWidgets(GetSetup()),
                    Content = DataInitializeHelper.GetTemplateContent(GetSetup()),
                    Script = DataInitializeHelper.GetTemplateScript(GetSetup()),
                    Style = DataInitializeHelper.GetTemplateStyle(GetSetup()),
                    FullContent = DataInitializeHelper.GetTemplateFullContent(GetSetup()),
                    IsDefaultTemplate = true
                };
                context.WidgetTemplates.AddIfNotExist(i => i.Name, template);
            }
        }

        public virtual WidgetSetupModel GetSetup()
        {
            return null;
        }

        public virtual string GenerateFullWidget()
        {
            return string.Empty;
        }

        public virtual string Render(string[] parameters)
        {
            return string.Empty;
        }
    }
}