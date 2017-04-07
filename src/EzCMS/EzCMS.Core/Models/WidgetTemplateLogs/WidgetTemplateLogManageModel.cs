using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.WidgetTemplateLogs
{
    public class WidgetTemplateLogManageModel
    {
        #region Constructors

        public WidgetTemplateLogManageModel()
        {

        }

        public WidgetTemplateLogManageModel(WidgetTemplate widgetTemplate)
        {
            TemplateId = widgetTemplate.Id;
            Name = widgetTemplate.Name;
            Widgets = widgetTemplate.Widgets;
            Content = widgetTemplate.Content;
            Script = widgetTemplate.Script;
            Style = widgetTemplate.Style;
            FullContent = widgetTemplate.FullContent;
            DataType = widgetTemplate.DataType;
        }
        #endregion

        #region Public Properties

        public int Id { get; set; }

        public int TemplateId { get; set; }

        public string Name { get; set; }

        public string Widgets { get; set; }

        public string Content { get; set; }

        public string Script { get; set; }

        public string Style { get; set; }

        public string FullContent { get; set; }

        public string DataType { get; set; }

        #endregion
    }
}
