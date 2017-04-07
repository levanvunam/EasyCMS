using System.Collections.Generic;
using Ez.Framework.Core.IoC;
using EzCMS.Core.Services.Styles;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.Shared
{
    public class ContentPreviewModel
    {
        public ContentPreviewModel()
        {
            var styleService = HostContainer.GetInstance<IStyleService>();

            IncludedStyles = styleService.GetIncludedStyles();
        }

        public ContentPreviewModel(string content)
            : this()
        {
            Content = content;
        }

        #region Public Properties

        public string Content { get; set; }

        public List<string> IncludedStyles { get; set; }

        #endregion
    }
}
