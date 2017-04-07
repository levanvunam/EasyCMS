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
using EzCMS.Core.Models.News;
using EzCMS.Core.Models.News.Widgets;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using EzCMS.Entity.Repositories.NewsNewsCategories;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.News
{
    public class NewsService : ServiceHelper, INewsService
    {
        private readonly INewsNewsCategoryRepository _newsNewsCategoryRepository;
        private readonly IRepository<NewsRead> _newsReadRepository;
        private readonly IRepository<Entity.Entities.Models.News> _newsRepository;

        public NewsService(IRepository<Entity.Entities.Models.News> newsRepository,
            INewsNewsCategoryRepository newsNewsCategoryRepository,
            IRepository<NewsRead> newsReadRepository)
        {
            _newsRepository = newsRepository;
            _newsNewsCategoryRepository = newsNewsCategoryRepository;
            _newsReadRepository = newsReadRepository;
        }

        #region Validation

        /// <summary>
        /// Check if title exists
        /// </summary>
        /// <param name="newsId">the news id</param>
        /// <param name="title">the news title</param>
        /// <returns></returns>
        public bool IsTitleExisted(int? newsId, string title)
        {
            return Fetch(u => u.Title.Equals(title) && u.Id != newsId).Any();
        }

        #endregion

        /// <summary>
        /// Remove relationship between News and News Category
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="newsCategoryId"></param>
        /// <returns></returns>
        public ResponseModel DeleteNewsNewsCategoryMapping(int newsId, int newsCategoryId)
        {
            var response = _newsNewsCategoryRepository.Delete(newsId, newsCategoryId);

            return response.SetMessage(response.Success
                ? T("NewsNewsCategory_Message_DeleteMappingSuccessfully")
                : T("NewsNewsCategory_Message_DeleteMappingFailure"));
        }

        /// <summary>
        /// Get viewable news
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Entity.Entities.Models.News GetViewableNews(int id)
        {
            return GetViewableNews().FirstOrDefault(n => n.Id == id);
        }

        /// <summary>
        /// Get news list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetNewsList()
        {
            return GetViewableNews().Select(news => new SelectListItem
            {
                Text = news.Title,
                Value = SqlFunctions.StringConvert((double) news.Id).Trim()
            });
        }

        /// <summary>
        /// Setup news tracking
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public ResponseModel SetupNewsTracking(int newsId)
        {
            #region Setup news read

            var anonymousContactId = WorkContext.CurrentContact.AnonymousContactId;
            if (anonymousContactId > 0)
            {
                var newsRead = new NewsRead
                {
                    NewsId = newsId,
                    AnonymousContactId = anonymousContactId
                };

                return _newsReadRepository.Insert(newsRead);
            }

            return new ResponseModel
            {
                Success = true
            };

            #endregion
        }

        /// <summary>
        /// Get viewable news
        /// </summary>
        /// <returns></returns>
        public IQueryable<Entity.Entities.Models.News> GetViewableNews()
        {
            var now = DateTime.UtcNow;
            return Fetch(n => n.Status != NewsEnums.NewsStatus.Inactive
                              && (!n.DateStart.HasValue || now >= n.DateStart)
                              && (!n.DateEnd.HasValue || now <= n.DateEnd));
        }

        #region Base

        public IQueryable<Entity.Entities.Models.News> GetAll()
        {
            return _newsRepository.GetAll();
        }

        public IQueryable<Entity.Entities.Models.News> Fetch(
            Expression<Func<Entity.Entities.Models.News, bool>> expression)
        {
            return _newsRepository.Fetch(expression);
        }

        public Entity.Entities.Models.News FetchFirst(Expression<Func<Entity.Entities.Models.News, bool>> expression)
        {
            return _newsRepository.FetchFirst(expression);
        }

        public Entity.Entities.Models.News GetById(object id)
        {
            return _newsRepository.GetById(id);
        }

        internal ResponseModel Insert(Entity.Entities.Models.News news)
        {
            return _newsRepository.Insert(news);
        }

        internal ResponseModel Update(Entity.Entities.Models.News news)
        {
            return _newsRepository.Update(news);
        }

        internal ResponseModel Delete(Entity.Entities.Models.News news)
        {
            return _newsRepository.Delete(news);
        }

        internal ResponseModel Delete(object id)
        {
            return _newsRepository.Delete(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the news
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchNews(JqSearchIn si, NewsSearchModel model)
        {
            var data = SearchNews(model);

            var news = Maps(data.ToList());

            return si.Search(news);
        }

        /// <summary>
        /// Export news
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, NewsSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchNews(model);

            var news = Maps(data.ToList());

            var exportData = si.Export(news, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search news
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<Entity.Entities.Models.News> SearchNews(NewsSearchModel model)
        {
            return Fetch(news => (string.IsNullOrEmpty(model.Keyword)
                                  || (!string.IsNullOrEmpty(news.Title) && news.Title.Contains(model.Keyword))
                                  || (!string.IsNullOrEmpty(news.Abstract) && news.Abstract.Contains(model.Keyword)))
                                 && (!model.NewsCategoryId.HasValue ||
                                     news.NewsNewsCategories.Any(
                                         newsNewsCategory => newsNewsCategory.NewsCategoryId == model.NewsCategoryId)));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        private IQueryable<NewsModel> Maps(IEnumerable<Entity.Entities.Models.News> news)
        {
            return news.Select(n => new NewsModel
            {
                Id = n.Id,
                Title = n.Title,
                Abstract = n.Abstract,
                ImageUrl = n.ImageUrl,
                DateStart = n.DateStart,
                DateEnd = n.DateEnd,
                Status = n.Status,
                IsHotNews = n.IsHotNews,
                Categories =
                    n.NewsNewsCategories.Any()
                        ? n.NewsNewsCategories.Select(x => x.NewsCategory.Name)
                            .Aggregate((current, next) => current + ", " + next)
                        : string.Empty,
                TotalReads = n.NewsReads.Count(),
                RecordOrder = n.RecordOrder,
                Created = n.Created,
                CreatedBy = n.CreatedBy,
                LastUpdate = n.LastUpdate,
                LastUpdateBy = n.LastUpdateBy
            }).AsQueryable();
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get news manage model by id
        /// </summary>
        /// <param name="id">the news id</param>
        /// <returns></returns>
        public NewsManageModel GetNewsManageModel(int? id = null)
        {
            var news = GetById(id);
            if (news != null)
            {
                return new NewsManageModel(news);
            }
            return new NewsManageModel();
        }

        /// <summary>
        /// Save news manage model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveNews(NewsManageModel model)
        {
            ResponseModel response;
            var news = GetById(model.Id);

            #region Edit News

            if (news != null)
            {
                news.Title = model.Title;
                news.Status = model.Status;
                news.Abstract = model.Abstract;
                news.Content = model.Content;
                news.ImageUrl = model.ImageUrl;
                news.IsHotNews = model.IsHotNews;
                news.DateStart = model.DateStart;
                news.DateEnd = model.DateEnd;

                #region Categories

                var currentCategories = news.NewsNewsCategories.Select(nc => nc.NewsCategoryId).ToList();

                if (model.NewsCategoryIds == null) model.NewsCategoryIds = new List<int>();

                // Remove reference to deleted categories
                var removedCategoryIds = currentCategories.Where(id => !model.NewsCategoryIds.Contains(id));
                _newsNewsCategoryRepository.Delete(news.Id, removedCategoryIds);

                // Add new reference to categories
                var addedCategoryIds = model.NewsCategoryIds.Where(id => !currentCategories.Contains(id));
                _newsNewsCategoryRepository.Insert(news.Id, addedCategoryIds);

                #endregion

                //Get page record order
                response = Update(news);
                return response.SetMessage(response.Success
                    ? T("News_Message_UpdateSuccessfully")
                    : T("News_Message_UpdateFailure"));
            }

            #endregion

            Mapper.CreateMap<NewsManageModel, Entity.Entities.Models.News>();
            news = Mapper.Map<NewsManageModel, Entity.Entities.Models.News>(model);

            response = Insert(news);

            #region Categories

            if (model.NewsCategoryIds == null) model.NewsCategoryIds = new List<int>();

            _newsNewsCategoryRepository.Insert(news.Id, model.NewsCategoryIds);

            #endregion

            return response.SetMessage(response.Success
                ? T("News_Message_CreateSuccessfully")
                : T("News_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete news by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteNews(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                //Delete mapping with category
                _newsNewsCategoryRepository.Delete(item.NewsNewsCategories);

                // Delete the news
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("News_Message_DeleteSuccessfully")
                    : T("News_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("News_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get news details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NewsDetailModel GetNewsDetailModel(int id)
        {
            var news = GetById(id);
            return news != null ? new NewsDetailModel(news) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateNewsData(XEditableModel model)
        {
            var news = GetById(model.Pk);
            if (news != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (NewsManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new NewsManageModel(news);
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

                    news.SetProperty(model.Name, value);
                    var response = Update(news);
                    return response.SetMessage(response.Success
                        ? T("News_Message_UpdateNewsInfoSuccessfully")
                        : T("News_Message_UpdateNewsInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("News_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("News_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Widgets

        /// <summary>
        /// Get news item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public NewsWidget GetNewsDetails(int id)
        {
            var news = GetViewableNews(id);
            if (news != null)
            {
                return new NewsWidget(news);
            }
            return new NewsWidget();
        }

        /// <summary>
        /// Get news listing
        /// </summary>
        /// <param name="index"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <param name="newsType"></param>
        /// <param name="categoryId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public NewsListingWidget GetNewsListing(int index, int pageSize, int total, NewsEnums.NewsType newsType,
            int? categoryId, NewsEnums.NewsStatus status = NewsEnums.NewsStatus.Active)
        {
            /*
             * If page size = 0, this mean we have no paging
             * If total = 0 then no limit
             * If category = 0 then no category
             */
            var now = DateTime.UtcNow;
            var data = Fetch(n => n.Status == status
                                  &&
                                  (!categoryId.HasValue || categoryId == 0 ||
                                   n.NewsNewsCategories.Any(nc => nc.NewsCategoryId == categoryId))
                                  && (!n.DateStart.HasValue || now >= n.DateStart)
                                  && (!n.DateEnd.HasValue || now <= n.DateEnd));

            switch (newsType)
            {
                case NewsEnums.NewsType.General:
                    data = data.Where(n => !n.IsHotNews);
                    break;
                case NewsEnums.NewsType.Hot:
                    data = data.Where(n => n.IsHotNews);
                    break;
            }

            if (total > 0)
            {
                data = data.OrderByDescending(n => n.DateStart ?? (n.LastUpdate ?? n.Created)).Take(total);
            }

            var totalPage = 0;
            if (pageSize != 0)
            {
                totalPage = data.Count()/pageSize;
                data = data.OrderByDescending(n => n.DateStart ?? (n.LastUpdate ?? n.Created)).Skip(index*pageSize)
                    .Take(pageSize);
            }

            var model = new NewsListingWidget
            {
                PageIndex = index,
                PageSize = pageSize,
                TotalPage = totalPage,
                NewsListing =
                    data.OrderByDescending(n => n.DateStart ?? (n.LastUpdate ?? n.Created))
                        .ToList()
                        .Select(n => new NewsWidget(n))
                        .ToList()
            };
            return model;
        }

        /// <summary>
        /// Get news of category
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public List<NewsWidget> GetNewsOfCategory(int categoryId)
        {
            return Fetch(n => n.NewsNewsCategories.Any(c => c.NewsCategoryId == categoryId)).ToList()
                .Select(n => new NewsWidget(n)).ToList();
        }

        #endregion
    }
}