using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.NewsCategories;
using EzCMS.Entity.Entities.Models;
using Ez.Framework.IoC;

namespace EzCMS.Core.Models.NewsCategories
{
    public class NewsCategoryManageModel : IValidatableObject
    {
        #region Constructors

        private readonly INewsCategoryService _categoryService;

        public NewsCategoryManageModel()
        {
            _categoryService = HostContainer.GetInstance<INewsCategoryService>();

            PossibleParents = _categoryService.GetPossibleParents();
        }

        public NewsCategoryManageModel(NewsCategory newsCategory)
            : this()
        {
            Id = newsCategory.Id;
            Name = newsCategory.Name;
            Abstract = newsCategory.Abstract;

            ParentId = newsCategory.ParentId;
            PossibleParents = _categoryService.GetPossibleParents(Id);

            RecordOrder = newsCategory.RecordOrder;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("NewsCategory_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("NewsCategory_Field_Abstract")]
        public string Abstract { get; set; }

        [LocalizedDisplayName("NewsCategory_Field_ParentId")]
        public int? ParentId { get; set; }

        public IEnumerable<SelectListItem> PossibleParents { get; set; }

        [LocalizedDisplayName("NewsCategory_Field_RecordOrder")]
        public int RecordOrder { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var categoryService = HostContainer.GetInstance<INewsCategoryService>();

            if (categoryService.IsNameExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("NewsCategory_Message_ExistingName"), new[] { "Name" });
            }
            if (categoryService.IsLevelOrderExisted(Id, ParentId, RecordOrder))
            {
                yield return new ValidationResult(localizedResourceService.T("NewsCategory_Message_ExistingOrder"), new[] { "RecordOrder" });
            }
        }
    }
}
