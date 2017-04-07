using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.LinkTypes;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.LinkTypes
{
    public class LinkTypeService : ServiceHelper, ILinkTypeService
    {
        private readonly IRepository<LinkType> _linkTypeRepository;

        public LinkTypeService(IRepository<LinkType> linkTypeRepository)
        {
            _linkTypeRepository = linkTypeRepository;
        }

        #region Validation

        /// <summary>
        /// Check if type exists
        /// </summary>
        /// <param name="linkTypeId"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public bool IsTypeExisted(int? linkTypeId, string typeName)
        {
            return Fetch(u => u.Name.Equals(typeName) && u.Id != linkTypeId).Any();
        }

        #endregion

        /// <summary>
        /// Get list of link type
        /// </summary>
        /// <param name="linkTypeIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetLinkTypes(List<int> linkTypeIds = null)
        {
            if (linkTypeIds == null) linkTypeIds = new List<int>();
            return GetAll().Select(g => new SelectListItem
            {
                Text = g.Name,
                Value = SqlFunctions.StringConvert((double) g.Id).Trim(),
                Selected = linkTypeIds.Any(id => id == g.Id)
            });
        }

        #region Base

        public IQueryable<LinkType> GetAll()
        {
            return _linkTypeRepository.GetAll();
        }

        public IQueryable<LinkType> Fetch(Expression<Func<LinkType, bool>> expression)
        {
            return _linkTypeRepository.Fetch(expression);
        }

        public LinkType FetchFirst(Expression<Func<LinkType, bool>> expression)
        {
            return _linkTypeRepository.FetchFirst(expression);
        }

        public LinkType GetById(object id)
        {
            return _linkTypeRepository.GetById(id);
        }

        internal ResponseModel Insert(LinkType linkType)
        {
            return _linkTypeRepository.Insert(linkType);
        }

        internal ResponseModel Update(LinkType linkType)
        {
            return _linkTypeRepository.Update(linkType);
        }

        internal ResponseModel Delete(LinkType linkType)
        {
            return _linkTypeRepository.Delete(linkType);
        }

        internal ResponseModel Delete(object id)
        {
            return _linkTypeRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _linkTypeRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the link types.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchLinkTypes(JqSearchIn si)
        {
            var data = GetAll();

            var linkType = Maps(data);

            return si.Search(linkType);
        }

        /// <summary>
        /// Export link types
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var linkTypes = Maps(data);

            var exportData = si.Export(linkTypes, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="linkType"></param>
        /// <returns></returns>
        private IQueryable<LinkTypeModel> Maps(IQueryable<LinkType> linkType)
        {
            return linkType.Select(m => new LinkTypeModel
            {
                Id = m.Id,
                Name = m.Name,
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
        /// Get link type manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LinkTypeManageModel GetLinkTypeManageModel(int? id = null)
        {
            var linkType = GetById(id);
            if (linkType != null)
            {
                return new LinkTypeManageModel(linkType);
            }
            return new LinkTypeManageModel();
        }

        /// <summary>
        /// Save link type
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveLinkType(LinkTypeManageModel model)
        {
            ResponseModel response;
            var linkType = GetById(model.Id);
            if (linkType != null)
            {
                linkType.Name = model.Name;
                response = Update(linkType);
                return response.SetMessage(response.Success
                    ? T("LinkType_Message_UpdateSuccessfully")
                    : T("LinkType_Message_UpdateFailure"));
            }
            Mapper.CreateMap<LinkTypeManageModel, LinkType>();
            linkType = Mapper.Map<LinkTypeManageModel, LinkType>(model);
            response = Insert(linkType);
            return response.SetMessage(response.Success
                ? T("LinkType_Message_CreateSuccessfully")
                : T("LinkType_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete link type
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteLinkType(int id)
        {
            var linkType = GetById(id);
            if (linkType != null)
            {
                if (linkType.Links.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("LinkType_Message_DeleteFailureBasedOnRelatedLinks")
                    };
                }
                var response = Delete(linkType);
                return response.SetMessage(response.Success
                    ? T("LinkType_Message_DeleteSuccessfully")
                    : T("LinkType_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("LinkType_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get link type detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LinkTypeDetailModel GetLinkTypeDetailModel(int? id = null)
        {
            var linkType = GetById(id);
            return linkType != null ? new LinkTypeDetailModel(linkType) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateLinkTypeData(XEditableModel model)
        {
            var linkType = GetById(model.Pk);
            if (linkType != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (LinkTypeManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new LinkTypeManageModel(linkType);
                    manageModel.SetProperty(model.Name, value);

                    var validationResults = manageModel.ValidateModel();

                    if (validationResults.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = validationResults.BuildValidationMessages()
                        };
                    }

                    #endregion

                    linkType.SetProperty(model.Name, value);
                    var response = Update(linkType);
                    return response.SetMessage(response.Success
                        ? T("LinkType_Message_UpdateLinkTypeInfoSuccessfully")
                        : T("LinkType_Message_UpdateLinkTypeInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("LinkType_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("LinkType_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}