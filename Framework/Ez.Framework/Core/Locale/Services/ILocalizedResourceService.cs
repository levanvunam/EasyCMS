using Ez.Framework.Core.IoC.Attributes;

namespace Ez.Framework.Core.Locale.Services
{
    [Register(Lifetime.PerInstance)]
    public interface ILocalizedResourceService
    {
        /// <summary>
        /// Gets the localized string from a text key, if the value is not available then add the default value to the dictionary
        /// </summary>
        /// <param name="textKey">Key of the string to get localized</param>
        /// <param name="defaultValue">Default value of the string mapped to key</param>
        /// <param name="parameters">Parameters for passing to the default value string</param>
        /// <returns></returns>
        string GetLocalizedResource(string textKey, string defaultValue = null, params object[] parameters);

        /// <summary>
        /// Get text by key, if not found set default value
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        string T(string textKey, string defaultValue = null);

        /// <summary>
        /// Get text by key, and format result
        /// </summary>
        /// <param name="textKey"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string TFormat(string textKey, params object[] parameters);
    }
}
