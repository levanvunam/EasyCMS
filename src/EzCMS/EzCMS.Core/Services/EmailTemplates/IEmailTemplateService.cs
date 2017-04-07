using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.EmailTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.EmailTemplates
{
    [Register(Lifetime.PerInstance)]
    public interface IEmailTemplateService : IBaseService<EmailTemplate>
    {
        /// <summary>
        /// Parse email from type and model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="type"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        EmailTemplateResponseModel ParseEmail<TModel>(EmailEnums.EmailTemplateType type, TModel model);

        /// <summary>
        /// Parse the email from template and model
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="template"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        EmailTemplateResponseModel ParseEmail<TModel>(EmailTemplate template, TModel model);

        /// <summary>
        /// Get email template detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EmailTemplateDetailModel GetEmailTemplateDetailModel(int id);

        #region Grid

        /// <summary>
        /// Search the email templates
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchEmailTemplates(JqSearchIn si);

        /// <summary>
        /// Export the email templates
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get email template manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EmailTemplateManageModel GetEmailTemplateManageModel(int? id = null);

        /// <summary>
        /// Save email template
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveEmailTemplate(EmailTemplateManageModel model);

        /// <summary>
        /// Update email template data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateEmailTemplateData(XEditableModel model);

        /// <summary>
        /// Delete email template
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteEmailTemplate(int id);

        #endregion

        #region Send Test Email Template 

        /// <summary>
        /// Send test email template
        /// </summary>
        /// <param name="emailTemplateSendTestModel"></param>
        /// <returns></returns>
        ResponseModel SendTestEmailTemplate(EmailTemplateSendTestModel emailTemplateSendTestModel);

        /// <summary>
        /// Get model for sending test email
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        EmailTemplateSendTestModel GetEmailTemplateSendTestModel(int? id);

        #endregion
    }
}