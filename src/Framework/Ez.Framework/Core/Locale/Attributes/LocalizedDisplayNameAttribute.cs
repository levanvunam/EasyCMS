using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Services;
using System.ComponentModel;

namespace Ez.Framework.Core.Locale.Attributes
{
    public class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        public string Key { get; set; }
        private readonly string _defaultValue;

        public LocalizedDisplayNameAttribute(string key)
        {
            Key = key;
        }

        public LocalizedDisplayNameAttribute(string key, string defaultValue)
        {
            Key = key;
            _defaultValue = defaultValue;
        }

        public override string DisplayName
        {
            get
            {
                var localizedResourceService = HostContainer.GetInstance<ILocalizedResourceService>();
                return localizedResourceService.T(Key, _defaultValue);
            }
        }
    }
}
