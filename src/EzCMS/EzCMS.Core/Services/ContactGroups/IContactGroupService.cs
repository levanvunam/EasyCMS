using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.ContactGroups;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ContactGroups
{
    [Register(Lifetime.PerInstance)]
    public interface IContactGroupService : IBaseService<ContactGroup>
    {
        #region Base

        ContactGroup GetByName(string name);

        #endregion

        #region Validation

        /// <summary>
        /// Check if contact group exists
        /// </summary>
        /// <param name="id">the contact group id</param>
        /// <param name="name">the contact group name</param>
        /// <returns></returns>
        bool IsNameExisted(int? id, string name);

        #endregion

        /// <summary>
        /// Count contacts currently in the group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int CountContacts(int id);

        /// <summary>
        /// Calculate number of contacts in this group queries
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        int CalculateContacts(int id);

        #region Grid Search

        /// <summary>
        /// Search contact groups
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchContactGroups(JqSearchIn si);

        /// <summary>
        /// Export contact groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        /// <summary>
        /// Get contact group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ContactGroupManageModel GetContactGroupManageModel(int? id = null);

        /// <summary>
        /// Get all contacts that fit queries
        /// </summary>
        /// <param name="queries"></param>
        /// <returns></returns>
        IQueryable<Contact> GetContacts(string queries);

        /// <summary>
        /// Get active contact groups
        /// </summary>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetActiveContactGroups();

        /// <summary>
        /// Update contact group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveContactGroup(ContactGroupManageModel model);

        /// <summary>
        /// Refresh contact group static contacts
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel RefreshContactGroup(int id);

        /// <summary>
        /// Add contact search to group
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel AddToGroup(AddToGroupModel model);

        /// <summary>
        /// Delete contact group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeleteContactGroup(int id);

        /// <summary>
        /// Change contact group active state
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel ChangeActiveState(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get contact group details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ContactGroupDetailModel GetContactGroupDetailModel(int? id = null);

        /// <summary>
        /// Update contact group data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdateContactGroupData(XEditableModel model);

        #endregion
    }
}