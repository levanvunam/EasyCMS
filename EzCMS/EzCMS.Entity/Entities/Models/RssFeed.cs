using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Entity.Models;
using EzCMS.Entity.Core.Enums;

namespace EzCMS.Entity.Entities.Models
{
    public class RssFeed : BaseModel
    {
        #region Public Properties

        [StringLength(512)]
        public string Name { get; set; }

        [StringLength(512)]
        public string Url { get; set; }

        public RssFeedEnums.RssType RssType { get; set; }

        #endregion
    }
}