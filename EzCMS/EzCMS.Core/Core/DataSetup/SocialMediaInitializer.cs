using System.Data.Entity;
using Ez.Framework.Core.Entity.Extensions;
using Ez.Framework.Core.Entity.Intialize;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Social.Enums;
using EzCMS.Entity.Entities;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Core.DataSetup
{
    public class SocialMediaInitializer : IDataInitializer
    {
        /// <summary>
        /// Priority of initializer
        /// </summary>
        /// <returns></returns>
        public DataInitializerPriority Priority()
        {
            return DataInitializerPriority.High;
        }

        #region Initialize

        /// <summary>
        /// Initialize socials
        /// </summary>
        public void Initialize(DbContext ezContext)
        {
            var context = ezContext as EzCMSEntities;
            if (context != null)
            {
                var socialMedia = new[]
                {
                    new SocialMedia
                    {
                        Name = SocialMediaEnums.SocialNetwork.Facebook.GetEnumName(),
                        MaxCharacter = 0,
                        RecordOrder = 10
                    },
                    new SocialMedia
                    {
                        Name = SocialMediaEnums.SocialNetwork.Twitter.GetEnumName(),
                        MaxCharacter = 0,
                        RecordOrder = 20
                    },
                    new SocialMedia
                    {
                        Name = SocialMediaEnums.SocialNetwork.LinkedIn.GetEnumName(),
                        MaxCharacter = 0,
                        RecordOrder = 30
                    }
                };
                context.SocialMedia.AddIfNotExist(l => l.Name, socialMedia);

            }
        }

        #endregion
    }
}
