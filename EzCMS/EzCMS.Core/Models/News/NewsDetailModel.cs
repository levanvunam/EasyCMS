using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.News
{
    public class NewsDetailModel
    {
        public NewsDetailModel()
        {
        }

        public NewsDetailModel(Entity.Entities.Models.News news)
            : this()
        {
            Id = news.Id;

            News = new NewsManageModel(news);

            Created = news.Created;
            CreatedBy = news.CreatedBy;
            LastUpdate = news.LastUpdate;
            LastUpdateBy = news.LastUpdateBy;

        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("News_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("News_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("News_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("News_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        public NewsManageModel News { get; set; }

        #endregion
    }
}
