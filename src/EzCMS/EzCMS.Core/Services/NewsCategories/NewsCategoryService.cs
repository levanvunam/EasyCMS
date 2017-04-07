using System;
using System.Collections.Generic;
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
using EzCMS.Core.Models.NewsCategories;
using EzCMS.Core.Models.NewsCategories.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.NewsCategories
{
    public class NewsCategoryService : ServiceHelper, INewsCategoryService
    {
        private readonly IHierarchyRepository<NewsCategory> _newsCategoryRepository;

        public NewsCategoryService(IHierarchyRepository<NewsCategory> newsCategoryRepository)
        {
            _newsCategoryRepository = newsCategoryRepository;
        }

        /// <summary>
        /// Get possible parent Navigation
        /// </summary>
        /// <param name="id">the current category id</param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetPossibleParents(int? id = null)
        {
            var newsCategories = GetAll();
            int? parentId = null;

            var category = GetById(id);
            if (category != null)
            {
                parentId = category.ParentId;
                newsCategories = _newsCategoryRepository.GetPossibleParents(category);
            }

            var data = newsCategories.Select(m => new HierarchyDropdownModel
            {
                Id = m.Id,
                Name = m.Name,
                Hierarchy = m.Hierarchy,
                RecordOrder = m.RecordOrder,
                Selected = parentId.HasValue && parentId.Value == m.Id
            }).ToList();

            return _newsCategoryRepository.BuildSelectList(data);
        }

        /// <summary>
        /// Get NewsCategory by parent id
        /// </summary>
        /// <param name="newsId"> the new id </param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetNewsCategories(int? newsId)
        {
            var data = GetAll().Select(c => new HierarchyDropdownModel
            {
                Id = c.Id,
                Name = c.Name,
                Hierarchy = c.Hierarchy,
                RecordOrder = c.RecordOrder,
                Selected = newsId.HasValue && c.NewsNewsCategories.Any(nc => nc.NewsId == newsId)
            }).ToList();
            return _newsCategoryRepository.BuildSelectList(data);
        }

        #region Base

        public IQueryable<NewsCategory> GetAll()
        {
            return _newsCategoryRepository.GetAll();
        }

        public IQueryable<NewsCategory> Fetch(Expression<Func<NewsCategory, bool>> expression)
        {
            return _newsCategoryRepository.Fetch(expression);
        }

        public NewsCategory FetchFirst(Expression<Func<NewsCategory, bool>> expression)
        {
            return _newsCategoryRepository.FetchFirst(expression);
        }

        public NewsCategory GetById(object id)
        {
            return _newsCategoryRepository.GetById(id);
        }

        internal ResponseModel Insert(NewsCategory newsCategory)
        {
            return _newsCategoryRepository.Insert(newsCategory);
        }

        internal ResponseModel Update(NewsCategory newsCategory)
        {
            return _newsCategoryRepository.Update(newsCategory);
        }

        internal ResponseModel HierarchyUpdate(NewsCategory newsCategory)
        {
            return _newsCategoryRepository.HierarchyUpdate(newsCategory);
        }

        internal ResponseModel HierarchyInsert(NewsCategory newsCategory)
        {
            return _newsCategoryRepository.HierarchyInsert(newsCategory);
        }

        internal ResponseModel Delete(NewsCategory newsCategory)
        {
            return _newsCategoryRepository.Delete(newsCategory);
        }

        internal ResponseModel Delete(object id)
        {
            return _newsCategoryRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _newsCategoryRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search news categories
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchNewsCategories(JqSearchIn si)
        {
            var data = GetAll();

            var newsCategories = Maps(data);

            return si.Search(newsCategories);
        }

        /// <summary>
        /// Export news categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var newsCategories = Maps(data);

            var exportData = si.Export(newsCategories, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search news categories of news
        /// </summary>
        /// <param name="si"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchNewsCategoriesOfNews(JqSearchIn si, int newsId)
        {
            var data = SearchNewsCategoriesByNews(newsId);

            var newsCategories = Maps(data);

            return si.Search(newsCategories);
        }

        /// <summary>
        /// Exports news categories of news
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportsNewsCategoriesOfNews(JqSearchIn si, GridExportMode gridExportMode, int newsId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchNewsCategoriesByNews(newsId);

            var newsCategories = Maps(data);

            var exportData = si.Export(newsCategories, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Search children categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchChildrenNewsCategories(JqSearchIn si, int newsCategoryId)
        {
            var data = SearchChildrenNewsCategories(newsCategoryId);

            var newsCategories = Maps(data);

            return si.Search(newsCategories);
        }

        /// <summary>
        /// Exports children categories
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        public HSSFWorkbook ExportsChildrenNewsCategories(JqSearchIn si, GridExportMode gridExportMode,
            int newsCategoryId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchChildrenNewsCategories(newsCategoryId);

            var newsCategories = Maps(data);

            var exportData = si.Export(newsCategories, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private methods

        /// <summary>
        /// Search news categories of news
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        private IQueryable<NewsCategory> SearchNewsCategoriesByNews(int newsId)
        {
            return Fetch(newsCategory =>
                newsCategory.NewsNewsCategories.Any() &&
                newsCategory.NewsNewsCategories.Any(newsNewsCategory => newsNewsCategory.NewsId == newsId));
        }

        /// <summary>
        /// Search children categories
        /// </summary>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        private IQueryable<NewsCategory> SearchChildrenNewsCategories(int newsCategoryId)
        {
            return Fetch(newsCategory => newsCategory.ParentId == newsCategoryId);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="newsCategories"></param>
        /// <returns></returns>
        private IQueryable<NewsCategoryModel> Maps(IQueryable<NewsCategory> newsCategories)
        {
            return newsCategories.Select(u => new NewsCategoryModel
            {
                Id = u.Id,
                Name = u.Name,
                Abstract = u.Abstract,
                ParentId = u.ParentId,
                ParentName = u.Parent.Name,
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
        /// Get NewsCategory manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NewsCategoryManageModel GetNewsCategoryManageModel(int? id = null)
        {
            var newsCategory = GetById(id);
            if (newsCategory != null)
            {
                return new NewsCategoryManageModel(newsCategory);
            }
            return new NewsCategoryManageModel();
        }

        /// <summary>
        /// Save NewsCategory
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveNewsCategoryManageModel(NewsCategoryManageModel model)
        {
            ResponseModel response;
            var newsCategory = GetById(model.Id);
            if (newsCategory != null)
            {
                newsCategory.Name = model.Name;
                newsCategory.Abstract = model.Abstract;
                newsCategory.ParentId = model.ParentId;
                newsCategory.RecordOrder = model.RecordOrder;
                response = HierarchyUpdate(newsCategory);
                return response.SetMessage(response.Success
                    ? T("NewsCategory_Message_UpdateSuccessfully")
                    : T("NewsCategory_Message_UpdateFailure"));
            }
            Mapper.CreateMap<NewsCategoryManageModel, NewsCategory>();
            newsCategory = Mapper.Map<NewsCategoryManageModel, NewsCategory>(model);
            response = HierarchyInsert(newsCategory);
            return response.SetMessage(response.Success
                ? T("NewsCategory_Message_CreateSuccessfully")
                : T("NewsCategory_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete news category by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteNewsCategory(int id)
        {
            var item = GetById(id);

            if (item != null)
            {
                if (item.ChildCategories.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("NewsCategory_Message_DeleteFailureBasedOnRelatedChildren")
                    };
                }

                if (item.NewsNewsCategories.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("NewsCategory_Message_DeleteFailureBasedOnRelatedNews")
                    };
                }

                // Delete the news category
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("NewsCategory_Message_DeleteSuccessfully")
                    : T("NewsCategory_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("NewsCategory_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get news category detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NewsCategoryDetailModel GetNewsCategoryDetailModel(int id)
        {
            var newsCategory = GetById(id);
            return newsCategory != null ? new NewsCategoryDetailModel(newsCategory) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateNewsCategoryData(XEditableModel model)
        {
            var newsCategory = GetById(model.Pk);
            if (newsCategory != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (NewsCategoryManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new NewsCategoryManageModel(newsCategory);
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

                    newsCategory.SetProperty(model.Name, value);
                    var response = Update(newsCategory);
                    return response.SetMessage(response.Success
                        ? T("NewsCategory_Message_UpdateNewsCategoryInfoSuccessfully")
                        : T("NewsCategory_Message_UpdateFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("NewsCategory_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("NewsCategory_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Validation

        /// <summary>
        /// Check if category order for root exists or not
        /// </summary>
        /// <param name="id"></param>
        /// <param name="parentId"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public bool IsLevelOrderExisted(int? id, int? parentId, int order)
        {
            return _newsCategoryRepository.IsLevelOrderExisted(id, parentId, order);
        }

        /// <summary>
        /// Check if news category name exists.
        /// </summary>
        /// <param name="newsCategoryId">the category id</param>
        /// <param name="name">the category title</param>
        /// <returns></returns>
        public bool IsNameExisted(int? newsCategoryId, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != newsCategoryId).Any();
        }

        #endregion

        #region Widgets

        /// <summary>
        /// Get category model
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public NewsCategoryWidget GetCategoryModel(int categoryId)
        {
            var category = GetById(categoryId);
            if (category != null)
            {
                return new NewsCategoryWidget(category, true);
            }
            return new NewsCategoryWidget();
        }

        /// <summary>
        /// Get news category listing
        /// </summary>
        /// <returns></returns>
        public NewsCategoriesWidget GetCategoryListing()
        {
            return new NewsCategoriesWidget
            {
                Categories = GetAll().ToList().Select(c => new NewsCategoryWidget(c)).ToList()
            };
        }

        #endregion
    }
}