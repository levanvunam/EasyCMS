using System.Collections.Generic;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.EmailLogs;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EmailLogs
{
    [Register(Lifetime.PerInstance)]
    public interface IEmailLogService : IBaseService<EmailLog>
    {
        /// <summary>
        /// Get emails waiting to be sent
        /// </summary>
        /// <returns></returns>
        List<EmailLog> GetEmailSendingQueues();

        #region Grid

        /// <summary>
        /// Search the email queues
        /// </summary>
        /// <param name="si"></param>
        /// <param name="emailAccountId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchEmailLogs(JqSearchIn si, int? emailAccountId = null);

        /// <summary>
        /// Export email queues
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        /// <summary>
        /// Search logs
        /// </summary>
        /// <param name="si"></param>
        /// <param name="emailLogId"></param>
        /// <returns></returns>
        JqGridSearchOut SearchLogs(JqSearchIn si, int emailLogId);

        #endregion

        #region Manage

        /// <summary>
        /// Create email queue
        /// </summary>
        /// <param name="emailLog"></param>
        /// <param name="useDefaultAccount"></param>
        /// <returns></returns>
        ResponseModel CreateEmail(EmailLog emailLog, bool useDefaultAccount = false);

        /// <summary>
        /// Update email
        /// </summary>
        /// <param name="emailLog"></param>
        /// <returns></returns>
        ResponseModel UpdateMail(EmailLog emailLog);

        /// <summary>
        /// Get email queue detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EmailLogDetailModel GetEmailLogDetailModel(int id);

        /// <summary>
        /// Delete email by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteEmail(int id);

        /// <summary>
        /// Resend the email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel ResendEmail(ResendEmailModel model);

        /// <summary>
        /// Get resend email model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResendEmailModel GetResendEmailModel(int id);

        #endregion
    }
}