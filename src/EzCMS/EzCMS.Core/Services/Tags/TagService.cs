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
using Ez.Framework.Core.Mvc.Models;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Tags;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.PageTags;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Tags
{
    public class TagService : ServiceHelper, ITagService
    {
        private readonly IPageTagRepository _pageTagRepository;
        private readonly IRepository<Tag> _tagRepository;

        public TagService(IRepository<Tag> tagRepository, IPageTagRepository pageTagRepository)
        {
            _tagRepository = tagRepository;
            _pageTagRepository = pageTagRepository;
        }

        /// <summary>
        /// Get tag select list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Select2Model> GetTags(int? pageId = null)
        {
            return GetAll().Select(t => new Select2Model
            {
                text = t.Name,
                slug = t.Name,
                id = t.Name
            });
        }

        /// <summary>
        /// Get tag select list
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetTagSelectList(int? pageId = null)
        {
            return GetAll().Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = SqlFunctions.StringConvert((double) t.Id).Trim(),
                Selected = pageId.HasValue && t.PageTags.Any(pageTag => pageTag.PageId == pageId)
            });
        }

        /// <summary>
        /// Get tag detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TagDetailModel GetTagDetailModel(int id)
        {
            var tag = GetById(id);
            return tag != null ? new TagDetailModel(tag) : null;
        }

        #region Validation

        /// <summary>
        /// Check if tag exists
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public bool IsTagExisted(int? tagId, string tagName)
        {
            return Fetch(u => u.Name.Equals(tagName) && u.Id != tagId).Any();
        }

        #endregion

        #region Base

        public IQueryable<Tag> GetAll()
        {
            return _tagRepository.GetAll();
        }

        public IQueryable<Tag> Fetch(Expression<Func<Tag, bool>> expression)
        {
            return _tagRepository.Fetch(expression);
        }

        public Tag FetchFirst(Expression<Func<Tag, bool>> expression)
        {
            return _tagRepository.FetchFirst(expression);
        }

        public Tag GetById(object id)
        {
            return _tagRepository.GetById(id);
        }

        internal ResponseModel Insert(Tag tag)
        {
            return _tagRepository.Insert(tag);
        }

        internal ResponseModel Update(Tag tag)
        {
            return _tagRepository.Update(tag);
        }

        internal ResponseModel Delete(Tag tag)
        {
            return _tagRepository.Delete(tag);
        }

        internal ResponseModel Delete(object id)
        {
            return _tagRepository.Delete(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the tags
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchTags(JqSearchIn si, int? pageId)
        {
            var data = SearchTags(pageId);

            var tags = Maps(data);

            return si.Search(tags);
        }

        /// <summary>
        /// Export tags
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? pageId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchTags(pageId);

            var tags = Maps(data);

            var exportData = si.Export(tags, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search tags
        /// </summary>
        /// <param name="pageId"></param>
        /// <returns></returns>
        private IQueryable<Tag> SearchTags(int? pageId)
        {
            return
                Fetch(
                    tag =>
                        !pageId.HasValue ||
                        (tag.PageTags.Any() && tag.PageTags.Select(y => y.PageId).Contains(pageId.Value)));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="tags"></param>
        /// <returns></returns>
        private IQueryable<TagModel> Maps(IQueryable<Tag> tags)
        {
            return tags.Select(u => new TagModel
            {
                Id = u.Id,
                Name = u.Name,
                RecordOrder = u.RecordOrder,
                Created = u.Created,
                CreatedBy = u.CreatedBy,
                LastUpdate = u.LastUpdate,
                LastUpdateBy = u.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get Tag manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public TagManageModel GetTagManageModel(int? id = null)
        {
            var tag = GetById(id);
            if (tag != null)
            {
                return new TagManageModel(tag);
            }
            return new TagManageModel();
        }

        /// <summary>
        /// Save Tag
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveTag(TagManageModel model)
        {
            ResponseModel response;
            var tag = GetById(model.Id);
            if (tag != null)
            {
                tag.Name = model.Name;
                response = Update(tag);
                return response.SetMessage(response.Success
                    ? T("Tag_Message_UpdateSuccessfully")
                    : T("Tag_Message_UpdateFailure"));
            }
            Mapper.CreateMap<TagManageModel, Tag>();
            tag = Mapper.Map<TagManageModel, Tag>(model);
            response = Insert(tag);
            return response.SetMessage(response.Success
                ? T("Tag_Message_CreateSuccessfully")
                : T("Tag_Message_CreateFailure"));
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateTagData(XEditableModel model)
        {
            var tag = GetById(model.Pk);
            if (tag != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (TagManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new TagManageModel(tag);
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

                    tag.SetProperty(model.Name, value);
                    var response = Update(tag);
                    return response.SetMessage(response.Success
                        ? T("Tag_Message_UpdateTagInfoSuccessfully")
                        : T("Tag_Message_UpdateTagInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Tag_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Tag_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete tag
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteTag(int id)
        {
            var tag = GetById(id);
            if (tag != null)
            {
                //delete page tag by tag
                _pageTagRepository.Delete(tag.PageTags);

                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("Tag_Message_DeleteSuccessfully")
                    : T("Tag_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Tag_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Remove Page and Tag reference
        /// </summary>
        /// <param name="tagId"></param>
        /// <param name="pageId"></param>
        /// <returns></returns>
        public ResponseModel DeletePageTagMapping(int tagId, int pageId)
        {
            var data = _pageTagRepository.FetchFirst(x => x.PageId == pageId && x.TagId == tagId);
            if (data != null)
            {
                var response = _pageTagRepository.Delete(data);
                return response.SetMessage(response.Success
                    ? T("PageTag_Message_DeleteMappingSuccessfully")
                    : T("PageTag_Message_DeleteMappingFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("PageTag_Message_DeleteMappingSuccessfully")
            };
        }

        #endregion
    }
}