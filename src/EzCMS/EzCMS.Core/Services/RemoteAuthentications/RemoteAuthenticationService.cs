using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.RemoteAuthentications;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.RemoteAuthentications
{
    public class RemoteAuthenticationService : ServiceHelper, IRemoteAuthenticationService
    {
        private readonly IRepository<RemoteAuthentication> _remoteAuthenticationRepository;

        public RemoteAuthenticationService(
            IRepository<RemoteAuthentication> remoteAuthenticationRepository)
        {
            _remoteAuthenticationRepository = remoteAuthenticationRepository;
        }

        #region Base

        public IQueryable<RemoteAuthentication> GetAll()
        {
            return _remoteAuthenticationRepository.GetAll();
        }

        public IQueryable<RemoteAuthentication> Fetch(
            Expression<Func<RemoteAuthentication, bool>> expression)
        {
            return _remoteAuthenticationRepository.Fetch(expression);
        }

        public RemoteAuthentication FetchFirst(
            Expression<Func<RemoteAuthentication, bool>> expression)
        {
            return _remoteAuthenticationRepository.FetchFirst(expression);
        }

        public RemoteAuthentication GetById(object id)
        {
            return _remoteAuthenticationRepository.GetById(id);
        }

        internal ResponseModel Insert(RemoteAuthentication remoteAuthentication)
        {
            return _remoteAuthenticationRepository.Insert(remoteAuthentication);
        }

        internal ResponseModel Update(RemoteAuthentication remoteAuthentication)
        {
            return _remoteAuthenticationRepository.Update(remoteAuthentication);
        }

        internal ResponseModel Delete(RemoteAuthentication remoteAuthentication)
        {
            return _remoteAuthenticationRepository.Delete(remoteAuthentication);
        }

        internal ResponseModel Delete(object id)
        {
            return _remoteAuthenticationRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _remoteAuthenticationRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the remote authenticate configurations
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchRemoteAuthentications(JqSearchIn si)
        {
            var data = GetAll();

            var remoteAuthentications = Maps(data);

            return si.Search(remoteAuthentications);
        }

        /// <summary>
        /// Export remote authenticate configurations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var remoteAuthentications = Maps(data);

            var exportData = si.Export(remoteAuthentications, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="remoteAuthentications"></param>
        /// <returns></returns>
        private IQueryable<RemoteAuthenticationModel> Maps(
            IQueryable<RemoteAuthentication> remoteAuthentications)
        {
            return remoteAuthentications.Select(m => new RemoteAuthenticationModel
            {
                Id = m.Id,
                Name = m.Name,
                ServiceUrl = m.ServiceUrl,
                AuthorizeCode = m.AuthorizeCode,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get RemoteAuthentication manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RemoteAuthenticationManageModel GetRemoteAuthenticationManageModel(int? id = null)
        {
            var remoteAuthentication = GetById(id);
            if (remoteAuthentication != null)
            {
                return new RemoteAuthenticationManageModel(remoteAuthentication);
            }
            return new RemoteAuthenticationManageModel();
        }

        /// <summary>
        /// Save RemoteAuthentication
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveRemoteAuthentication(RemoteAuthenticationManageModel model)
        {
            ResponseModel response;
            var remoteAuthentication = GetById(model.Id);
            if (remoteAuthentication != null)
            {
                remoteAuthentication.Name = model.Name;
                remoteAuthentication.ServiceUrl = model.ServiceUrl;
                remoteAuthentication.AuthorizeCode = model.AuthorizeCode;
                remoteAuthentication.Active = model.Active;
                response = Update(remoteAuthentication);
                return response.SetMessage(response.Success
                    ? T("RemoteAuthentication_Message_UpdateSuccessfully")
                    : T("RemoteAuthentication_Message_UpdateFailure"));
            }
            Mapper.CreateMap<RemoteAuthenticationManageModel, RemoteAuthentication>();
            remoteAuthentication =
                Mapper.Map<RemoteAuthenticationManageModel, RemoteAuthentication>(model);
            response = Insert(remoteAuthentication);
            return response.SetMessage(response.Success
                ? T("RemoteAuthentication_Message_CreateSuccessfully")
                : T("RemoteAuthentication_Message_CreateFailure"));
        }

        /// <summary>
        /// Get active remote services
        /// </summary>
        /// <returns></returns>
        public List<RemoteAuthenticationModel> GetActiveRemoteServices()
        {
            var activeRemoteServices = Fetch(r => r.Active);

            return Maps(activeRemoteServices).ToList();
        }

        #endregion
    }
}