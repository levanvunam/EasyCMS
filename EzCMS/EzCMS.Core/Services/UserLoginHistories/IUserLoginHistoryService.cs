using System.Web;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.UserLoginHistories;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.UserLoginHistories
{
    [Register(Lifetime.PerInstance)]
    public interface IUserLoginHistoryService : IBaseService<UserLoginHistory>
    {
        #region Details

        /// <summary>
        /// Get user login history details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        UserLoginHistoryDetailModel GetUserLoginHistoryDetailModel(int id);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search user login histories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchUserLoginHistories(JqSearchIn si, UserLoginHistorySearchModel model);

        /// <summary>
        /// Export user login histories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, UserLoginHistorySearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Log login history for user
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        ResponseModel InsertUserLoginHistory(int userId, HttpContext context);

        /// <summary>
        /// Insert user login history
        /// </summary>
        /// <param name="userLoginHistory"></param>
        /// <returns></returns>
        ResponseModel Insert(UserLoginHistory userLoginHistory);

        #endregion
    }
}