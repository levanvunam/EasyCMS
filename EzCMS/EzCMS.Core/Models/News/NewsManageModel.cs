using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.IoC;
using Ez.Framework.Utilities;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.News;
using EzCMS.Core.Services.NewsCategories;
using EzCMS.Entity.Core.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.News
{
    public class NewsManageModel : IValidatableObject
    {
        public NewsManageModel()
        {
            var newsCategoryService = HostContainer.GetInstance<INewsCategoryService>();
            StatusList = EnumUtilities.GenerateSelectListItems<NewsEnums.NewsStatus>();
            NewsCategories = newsCategoryService.GetNewsCategories(Id);
        }

        public NewsManageModel(Entity.Entities.Models.News news)
            : this()
        {
            Id = news.Id;
            Abstract = news.Abstract;
            Content = news.Content;
            DateStart = news.DateStart;
            DateEnd = news.DateEnd;
            ImageUrl = news.ImageUrl;
            Title = news.Title;
            IsHotNews = news.IsHotNews;
            Status = news.Status;

            var newsCategoryService = HostContainer.GetInstance<INewsCategoryService>();
            StatusList = EnumUtilities.GenerateSelectListItems<NewsEnums.NewsStatus>();
            NewsCategories = newsCategoryService.GetNewsCategories(Id);
        }

        #region Public Properties
        public int? Id { get; set; }

        [LocalizedDisplayName("News_Field_ImageUrl")]
        public string ImageUrl { get; set; }

        [Required]
        [LocalizedDisplayName("News_Field_Title")]
        public string Title { get; set; }

        [LocalizedDisplayName("News_Field_Abstract")]
        public string Abstract { get; set; }

        [LocalizedDisplayName("News_Field_DateStart")]
        public DateTime? DateStart { get; set; }

        [DateGreaterThan("DateStart")]
        [LocalizedDisplayName("News_Field_DateEnd")]
        public DateTime? DateEnd { get; set; }

        [Required]
        [LocalizedDisplayName("News_Field_Content")]
        public string Content { get; set; }

        [Required]
        [LocalizedDisplayName("News_Field_IsHotNews")]
        public bool IsHotNews { get; set; }

        [LocalizedDisplayName("News_Field_NewsCategoryIds")]
        public List<int> NewsCategoryIds { get; set; }

        public IEnumerable<SelectListItem> NewsCategories { get; set; }

        [Required]
        [LocalizedDisplayName("News_Field_Status")]
        public NewsEnums.NewsStatus Status { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }

        #endregion

        /// <summary>
        /// Validate the model
        /// </summary>
        /// <param name="context">the validation context</param>
        /// <returns></returns>
        public IEnumerable<ValidationResult> Validate(ValidationContext context)
        {
            var newsService = HostContainer.GetInstance<INewsService>();
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            if (newsService.IsTitleExisted(Id, Title))
            {
                yield return new ValidationResult(localizedResourceService.T("News_Message_ExistingTitle"), new[] { "Title" });
            }
        }
    }
}
