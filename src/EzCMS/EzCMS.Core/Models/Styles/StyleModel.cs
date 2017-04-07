using Ez.Framework.Models;

namespace EzCMS.Core.Models.Styles
{
    public class StyleModel : BaseGridModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public bool IncludeIntoEditor { get; set; }
    }
}
