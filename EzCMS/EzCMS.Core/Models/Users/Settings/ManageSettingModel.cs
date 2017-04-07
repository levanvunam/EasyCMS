using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Attributes.Validation;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Time;
using Ez.Framework.IoC;
using EzCMS.Core.Services.Languages;
using EzCMS.Core.Services.Users;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Users.Settings
{
    public class ManageSettingModel
    {
        private readonly ILanguageService _languageService;
        public ManageSettingModel()
        {
            _languageService = HostContainer.GetInstance<ILanguageService>();
            Cultures = _languageService.GetLanguages();

            // Default page size
            AdminPageSize = 50;
            TimeZones = TimeZoneUtilities.GetTimeZones();
        }

        public ManageSettingModel(User user)
            : this()
        {
            Init(user);
        }

        public ManageSettingModel(int userId)
            : this()
        {
            var userService = HostContainer.GetInstance<IUserService>();
            var user = userService.GetById(userId);
            if (user != null)
            {
                Init(user);
            }
        }

        private void Init(User user)
        {
            UserId = user.Id;

            var setting = string.IsNullOrEmpty(user.Settings)
                ? new UserSettingModel()
                : SerializeUtilities.Deserialize<UserSettingModel>(user.Settings);
            AdminPageSize = setting.AdminPageSize;
            TimeZone = setting.TimeZone;

            Culture = setting.Culture;
            Cultures = _languageService.GetLanguages();

            TimeZones = TimeZoneUtilities.GetTimeZones(TimeZone);
        }

        #region Public Properties

        [ScriptIgnore]
        public int UserId { get; set; }

        [RequiredInteger("Account_Field_AdminPageSize", "Account_UserSetting_Message_AdminPageSizeValidateMessage")]
        [LocalizedDisplayName("Account_UserSetting_Field_AdminPageSize")]
        public int AdminPageSize { get; set; }

        [LocalizedDisplayName("Account_UserSetting_Field_TimeZone")]
        public string TimeZone { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> TimeZones { get; set; }

        [LocalizedDisplayName("Account_UserSetting_Field_Culture")]
        public string Culture { get; set; }

        [ScriptIgnore]
        public IEnumerable<SelectListItem> Cultures { get; set; }

        #endregion
    }
}
