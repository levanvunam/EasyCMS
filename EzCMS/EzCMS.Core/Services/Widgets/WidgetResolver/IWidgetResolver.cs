using Ez.Framework.Core.IoC.Attributes;
using EzCMS.Core.Models.Widgets;

namespace EzCMS.Core.Services.Widgets.WidgetResolver
{
    [Register(Lifetime.PerInstance)]
    public interface IWidgetResolver
    {
        /// <summary>
        /// Get widget information
        /// </summary>
        /// <returns></returns>
        WidgetSetupModel GetSetup();

        /// <summary>
        /// Generate full widget with data
        /// </summary>
        /// <returns></returns>
        string GenerateFullWidget();

        /// <summary>
        /// Render widget function
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        string Render(string[] parameters);
    }
}