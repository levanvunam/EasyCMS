using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.RotatingImageGroups
{
    public class GroupSettingModel
    {
        public GroupSettingModel()
        {
            AutoPlay = 3000;
            StopOnHover = true;
            Navigation = true;
            PaginationSpeed = 1000;
            PaginationNumbers = true;
            LazyLoad = true;
            GoToFirstSpeed = 2000;
            SingleItem = true;
            AutoHeight = true;
            TransitionStyle = "fade";
        }

        #region Public Properties

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_AutoPlay")]
        public int AutoPlay { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_StopOnHover")]
        public bool StopOnHover { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_Navigation")]
        public bool Navigation { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_PaginationSpeed")]
        public int PaginationSpeed { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_PaginationNumbers")]
        public bool PaginationNumbers { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_LazyLoad")]
        public bool LazyLoad { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_GoToFirstSpeed")]
        public int GoToFirstSpeed { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_SingleItem")]
        public bool SingleItem { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_AutoHeight")]
        public bool AutoHeight { get; set; }

        [Required]
        [LocalizedDisplayName("RotatingImageGroup_Field_TransitionStyle")]
        public string TransitionStyle { get; set; }

        #endregion
    }
}
