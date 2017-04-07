using System;

namespace Ez.Framework.Utilities.Social.Models
{
    public class SocialFeedItemModel
    {
        #region Public Properties

        public string Id { get; set; }

        public string Title { get; set; }

        public string Message { get; set; }

        public string Link { get; set; }

        public string Caption { get; set; }

        public string Description { get; set; }

        public string Picture { get; set; }

        public DateTime? Created { get; set; }

        #endregion
    }
}
