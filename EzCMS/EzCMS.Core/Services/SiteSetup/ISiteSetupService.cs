using Ez.Framework.Core.Entity.RepositoryBase.Models;
using EzCMS.Core.Models.SiteSetup;

namespace EzCMS.Core.Services.SiteSetup
{
    public interface ISiteSetupService
    {
        /// <summary>
        /// Get database setup model
        /// </summary>
        /// <returns></returns>
        DatabaseSetupModel GetDatabaseSetupModel();

        /// <summary>
        /// Save database setup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveDatabaseSetupModel(DatabaseSetupModel model);

        /// <summary>
        /// Get user setup model
        /// </summary>
        /// <returns></returns>
        UserSetupModel GetUserSetupModel();

        /// <summary>
        /// Save admin user
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveUserSetupModel(UserSetupModel model);

        /// <summary>
        /// Get company setup model
        /// </summary>
        /// <returns></returns>
        CompanySetupModel GetCompanySetupModel();

        /// <summary>
        /// Save company setup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveCompanySetupModel(CompanySetupModel model);

        /// <summary>
        /// Get email setup model
        /// </summary>
        /// <returns></returns>
        EmailSetupModel GetEmailSetupModel();

        /// <summary>
        /// Save email setup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveEmailSetupModel(EmailSetupModel model);

        /// <summary>
        /// Get avail starter kits model
        /// </summary>
        /// <returns></returns>
        StarterKitsModel GetStarterKitsModel();

        /// <summary>
        /// Save starter kits
        /// </summary>
        /// <param name="chosenKit"></param>
        /// <returns></returns>
        ResponseModel SaveStarterKitsModel(string chosenKit);

        /// <summary>
        /// Finish setup and restart AppDomain
        /// </summary>
        /// <returns></returns>
        ResponseModel FinishSetup();
    }
}