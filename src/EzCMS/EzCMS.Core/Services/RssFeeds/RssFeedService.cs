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
using EzCMS.Core.Models.RssFeeds;
using EzCMS.Core.Models.RssFeeds.Widgets;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.RssFeeds
{
    public class RssFeedService : ServiceHelper, IRssFeedService
    {
        private readonly IRepository<RssFeed> _rssFeedRepository;

        public RssFeedService(IRepository<RssFeed> rssFeedRepository)
        {
            _rssFeedRepository = rssFeedRepository;
        }

        #region Validation

        /// <summary>
        /// Check if RSS name exists in the system
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool IsNameExisted(int? id, string name)
        {
            return Fetch(u => u.Name.Equals(name) && u.Id != id).Any();
        }

        #endregion

        /// <summary>
        /// Get RSS feeds for select list
        /// </summary>
        /// <param name="rssType"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRssFeeds(RssFeedEnums.RssType? rssType = null)
        {
            var rssFeeds = Fetch(rssFeed => rssType.HasValue && rssFeed.RssType == rssType.Value);

            return rssFeeds.Select(rssFeed => new SelectListItem
            {
                Text = rssFeed.Name,
                Value = SqlFunctions.StringConvert((double) rssFeed.Id).Trim()
            });
        }

        #region Base

        public IQueryable<RssFeed> GetAll()
        {
            return _rssFeedRepository.GetAll();
        }

        public IQueryable<RssFeed> Fetch(Expression<Func<RssFeed, bool>> expression)
        {
            return _rssFeedRepository.Fetch(expression);
        }

        public RssFeed FetchFirst(Expression<Func<RssFeed, bool>> expression)
        {
            return _rssFeedRepository.FetchFirst(expression);
        }

        public RssFeed GetById(object id)
        {
            return _rssFeedRepository.GetById(id);
        }

        public ResponseModel Insert(RssFeed rssFeed)
        {
            return _rssFeedRepository.Insert(rssFeed);
        }

        public ResponseModel Update(RssFeed rssFeed)
        {
            return _rssFeedRepository.Update(rssFeed);
        }

        public ResponseModel Delete(RssFeed rssFeed)
        {
            return _rssFeedRepository.Delete(rssFeed);
        }

        public ResponseModel Delete(object id)
        {
            return _rssFeedRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _rssFeedRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search feeds
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchRssFeed(JqSearchIn si)
        {
            var data = GetAll();

            var feeds = Maps(data);

            return si.Search(feeds);
        }

        /// <summary>
        /// Export rss feeds
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var feeds = Maps(data);

            var exportData = si.Export(feeds, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="feeds"></param>
        /// <returns></returns>
        private IQueryable<RssFeedModel> Maps(IQueryable<RssFeed> feeds)
        {
            return feeds.Select(rssFeed => new RssFeedModel
            {
                Id = rssFeed.Id,
                RssType = rssFeed.RssType,
                Name = rssFeed.Name,
                Url = rssFeed.Url,
                RecordOrder = rssFeed.RecordOrder,
                Created = rssFeed.Created,
                CreatedBy = rssFeed.CreatedBy,
                LastUpdate = rssFeed.LastUpdate,
                LastUpdateBy = rssFeed.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get RSS feed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RssFeedManageModel GetRssFeedManageModel(int? id = null)
        {
            var rssFeed = GetById(id);
            return rssFeed != null ? new RssFeedManageModel(rssFeed) : new RssFeedManageModel();
        }

        /// <summary>
        /// Save RSS feed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveRssFeed(RssFeedManageModel model)
        {
            ResponseModel response;
            var feed = GetById(model.Id);

            // Edit existed RSS feed
            if (feed != null)
            {
                feed.Name = model.Name;
                feed.Url = model.Url;

                response = Update(feed);
                return
                    response.SetMessage(response.Success
                        ? T("RSSFeed_Message_UpdateSuccessfully")
                        : T("RSSFeed_Message_UpdateFailure"));
            }

            // Insert new RSS feed
            Mapper.CreateMap<RssFeedManageModel, RssFeed>();
            feed = Mapper.Map<RssFeedManageModel, RssFeed>(model);

            response = Insert(feed);

            return response.SetMessage(response.Success
                ? T("RSSFeed_Message_CreateSuccessfully")
                : T("RSSFeed_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete RSS feed
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteRssFeed(int id)
        {
            var rssFeed = GetById(id);

            if (rssFeed != null)
            {
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("RSSFeed_Message_DeleteSuccessfully")
                    : T("RSSFeed_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("RSSFeed_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Get RSS feed to display in widget
        /// </summary>
        /// <param name="rssFeedId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public RssFeedWidget GetRssFeedWidget(int rssFeedId, int? number)
        {
            var rssFeed = GetById(rssFeedId);
            if (rssFeed != null)
            {
                return new RssFeedWidget(rssFeed, number);
            }
            return new RssFeedWidget();
        }

        #endregion

        #region Details

        /// <summary>
        /// Get RSS details
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RssFeedDetailModel GetRssFeedDetailModel(int id)
        {
            var rssFeed = GetById(id);
            return rssFeed != null ? new RssFeedDetailModel(rssFeed) : null;
        }

        /// <summary>
        /// Update RSS feed
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateRssFeedData(XEditableModel model)
        {
            var rssFeed = GetById(model.Pk);
            if (rssFeed != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (RssFeedManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new RssFeedManageModel(rssFeed);
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

                    rssFeed.SetProperty(model.Name, value);

                    var response = Update(rssFeed);
                    return response.SetMessage(response.Success
                        ? T("RSSFeed_Message_UpdateRssFeedInfoSuccessfully")
                        : T("RSSFeed_Message_UpdateRssFeedInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("RSSFeed_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("RSSFeed_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Widget

        #endregion
    }
}