using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.NewsCategories
{
    public class NewsCategoryDetailModel
    {
        public NewsCategoryDetailModel()
        {
        }

        public NewsCategoryDetailModel(NewsCategory newsCategory)
            : this()
        {
            Id = newsCategory.Id;

            NewsCategory = new NewsCategoryManageModel(newsCategory);
            ParentName = newsCategory.Parent != null ? newsCategory.Parent.Name : string.Empty;

            Created = newsCategory.Created;
            CreatedBy = newsCategory.CreatedBy;
            LastUpdate = newsCategory.LastUpdate;
            LastUpdateBy = newsCategory.LastUpdateBy;
        }

        #region Public Properties

        public int? Id { get; set; }

        [LocalizedDisplayName("NewsCategory_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("NewsCategory_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("NewsCategory_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("NewsCategory_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        [LocalizedDisplayName("NewsCategory_Field_ParentName")]
        public string ParentName { get; set; }

        public NewsCategoryManageModel NewsCategory { get; set; }

        #endregion
    }
}
