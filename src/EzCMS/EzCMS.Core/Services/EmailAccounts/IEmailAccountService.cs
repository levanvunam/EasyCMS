using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using Ez.Framework.Utilities.Mails.Models;
using EzCMS.Core.Models.EmailAccounts;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EmailAccounts
{
    [Register(Lifetime.PerInstance)]
    public interface IEmailAccountService : IBaseService<EmailAccount>
    {
        EmailAccount GetDefaultAccount();

        /// <summary>
        /// Make an email to default
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel MarkAsDefault(int id);

        /// <summary>
        /// Send test email
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SendTestEmail(TestEmailModel model);

        IEnumerable<SelectListItem> GetEmailAccounts();

        ResponseModel SendEmailDirectly(EmailAccount emailAccount, EmailModel emailModel);

        EmailAccountDetailModel GetEmailAccountDetailModel(int id);

        #region Grid

        /// <summary>
        /// Search the email accounts
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchEmailAccounts(JqSearchIn si);

        /// <summary>
        /// Export the email accounts
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        EmailAccountManageModel GetEmailAccountManageModel(int? id = null);

        ResponseModel SaveEmailAccount(EmailAccountManageModel model);

        ResponseModel UpdateEmailAccountData(XEditableModel model);

        ResponseModel DeleteEmailAccount(int id);

        #endregion
    }
}