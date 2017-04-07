using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.SiteSettings.Models;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.SiteSettings;
using System.ComponentModel.DataAnnotations;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Core.SiteSettings.ComplexSettings
{
    public class GoogleMapsSetting : ComplexSetting
    {
        #region Setup

        /// <summary>
        /// Get complex setting setup
        /// </summary>
        /// <returns></returns>
        public override ComplexSettingSetup GetSetup()
        {
            return new ComplexSettingSetup
            {
                Name = SettingNames.GoogleMapsSetting,
                SettingType = "system",
                DefaultValue = new GoogleMapsSetting
                {
                    Address = EzCMSContants.GoogleMapDefaultAddress,
                    Zoom = EzCMSContants.GoogleMapDefaultZoom,
                    Latitude = EzCMSContants.GoogleMapDefaultLatitude,
                    Longitude = EzCMSContants.GoogleMapDefaultLongitude
                }
            };
        }

        #endregion

        #region Public Properties

        [LocalizedDisplayName("SiteSetting_GoogleMapsSetting_Field_Address")]
        public string Address { get; set; }

        [LocalizedDisplayName("SiteSetting_GoogleMapsSetting_Field_Zoom")]
        [Range(5, 18)]
        public int Zoom { get; set; }

        [LocalizedDisplayName("SiteSetting_GoogleMapsSetting_Field_Latitude")]
        public double? Latitude { get; set; }

        [LocalizedDisplayName("SiteSetting_GoogleMapsSetting_Field_Longitude")]
        public double? Longitude { get; set; }

        #endregion
    }
}
