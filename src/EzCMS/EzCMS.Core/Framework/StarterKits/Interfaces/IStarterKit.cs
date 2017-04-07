namespace EzCMS.Core.Framework.StarterKits.Interfaces
{
    public interface IStarterKit
    {
        /// <summary>
        /// Get the template folder
        /// </summary>
        /// <returns></returns>
        string GetTemplateFolder();

        /// <summary>
        /// Setup process
        /// </summary>
        void Setup();
    }
}
