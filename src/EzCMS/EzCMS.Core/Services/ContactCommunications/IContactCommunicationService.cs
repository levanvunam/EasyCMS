using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.ContactCommunications;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ContactCommunications
{
    [Register(Lifetime.PerInstance)]
    public interface IContactCommunicationService : IBaseService<ContactCommunication>
    {
        ResponseModel Insert(ContactCommunication contactCommunication);

        ResponseModel Update(ContactCommunication contactCommunication);

        #region Grid Search

        JqGridSearchOut SearchContactCommunications(JqSearchIn si, int contactId);

        HSSFWorkbook Exports(int contactId);

        #endregion

        #region Manage

        ContactCommunicationManageModel GetContactCommunicationManageModel(int? contactId, int? id = null);

        ResponseModel SaveContactCommunication(ContactCommunicationManageModel model);

        ResponseModel DeleteContactCommunication(int id);

        #endregion
    }
}