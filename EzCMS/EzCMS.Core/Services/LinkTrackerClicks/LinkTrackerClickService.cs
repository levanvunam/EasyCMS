using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.LinkTrackerClicks;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.LinkTrackerClicks
{
    public class LinkTrackerClickService : ServiceHelper, ILinkTrackerClickService
    {
        private readonly IRepository<LinkTrackerClick> _linkTrackerClickRepository;

        public LinkTrackerClickService(IRepository<LinkTrackerClick> linkTrackerClickRepository)
        {
            _linkTrackerClickRepository = linkTrackerClickRepository;
        }

        #region Details

        /// <summary>
        /// Get link tracker click detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LinkTrackerClickDetailModel GetLinkTrackerClickDetailModel(int id)
        {
            var linkTrackerClick = GetById(id);
            return linkTrackerClick != null ? new LinkTrackerClickDetailModel(linkTrackerClick) : null;
        }

        #endregion

        #region Monthly Click Through

        /// <summary>
        /// Get all click years
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetYears()
        {
            var years = new List<SelectListItem>();

            var data = GetAll();
            if (data.Any())
            {
                var minYear = data.Min(m => m.Created).Year;
                var maxYear = data.Max(m => m.Created).Year;

                for (var year = minYear; year <= maxYear; year++)
                {
                    years.Add(new SelectListItem
                    {
                        Text = year.ToString(CultureInfo.InvariantCulture),
                        Value = year.ToString(CultureInfo.InvariantCulture),
                        Selected = year == maxYear
                    });
                }
            }
            return years;
        }

        #endregion

        #region Base

        public IQueryable<LinkTrackerClick> GetAll()
        {
            return _linkTrackerClickRepository.GetAll();
        }

        public IQueryable<LinkTrackerClick> Fetch(Expression<Func<LinkTrackerClick, bool>> expression)
        {
            return _linkTrackerClickRepository.Fetch(expression);
        }

        public LinkTrackerClick FetchFirst(Expression<Func<LinkTrackerClick, bool>> expression)
        {
            return _linkTrackerClickRepository.FetchFirst(expression);
        }

        public LinkTrackerClick GetById(object id)
        {
            return _linkTrackerClickRepository.GetById(id);
        }

        internal ResponseModel Insert(LinkTrackerClick linkTrackerClick)
        {
            return _linkTrackerClickRepository.Insert(linkTrackerClick);
        }

        internal ResponseModel Update(LinkTrackerClick linkTrackerClick)
        {
            return _linkTrackerClickRepository.Update(linkTrackerClick);
        }

        internal ResponseModel Delete(LinkTrackerClick linkTrackerClick)
        {
            return _linkTrackerClickRepository.Delete(linkTrackerClick);
        }

        internal ResponseModel Delete(object id)
        {
            return _linkTrackerClickRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _linkTrackerClickRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the link tracker clicks
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchLinkTrackerClicks(JqSearchIn si, int? linkTrackerId)
        {
            var data = SearchLinkTrackerClicks(linkTrackerId);

            var linkTrackerClicks = Maps(data);

            return si.Search(linkTrackerClicks);
        }

        /// <summary>
        /// Export link tracker clicks
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="linkTrackerId"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, int? linkTrackerId)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchLinkTrackerClicks(linkTrackerId);

            var linkTrackerClicks = Maps(data);

            var exportData = si.Export(linkTrackerClicks, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search link tracker clicks
        /// </summary>
        /// <param name="linkTrackerId"></param>
        /// <returns></returns>
        private IQueryable<LinkTrackerClick> SearchLinkTrackerClicks(int? linkTrackerId)
        {
            return Fetch(linkTrackerClick => !linkTrackerId.HasValue || linkTrackerClick.LinkTrackerId == linkTrackerId);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="linkTrackerClicks"></param>
        /// <returns></returns>
        private IQueryable<LinkTrackerClickModel> Maps(IQueryable<LinkTrackerClick> linkTrackerClicks)
        {
            return linkTrackerClicks.Select(linkTrackerClick => new LinkTrackerClickModel
            {
                Id = linkTrackerClick.Id,
                LinkTrackerId = linkTrackerClick.LinkTrackerId,
                LinkTrackerName = linkTrackerClick.LinkTracker.Name,
                IpAddress = linkTrackerClick.IpAddress,
                Platform = linkTrackerClick.Platform,
                OsVersion = linkTrackerClick.OsVersion,
                BrowserType = linkTrackerClick.BrowserType,
                BrowserName = linkTrackerClick.BrowserName,
                BrowserVersion = linkTrackerClick.BrowserVersion,
                MajorVersion = linkTrackerClick.MajorVersion,
                MinorVersion = linkTrackerClick.MinorVersion,
                IsBeta = linkTrackerClick.IsBeta,
                IsCrawler = linkTrackerClick.IsCrawler,
                IsAOL = linkTrackerClick.IsAOL,
                IsWin16 = linkTrackerClick.IsWin16,
                IsWin32 = linkTrackerClick.IsWin32,
                SupportsFrames = linkTrackerClick.SupportsFrames,
                SupportsTables = linkTrackerClick.SupportsTables,
                SupportsCookies = linkTrackerClick.SupportsCookies,
                SupportsVBScript = linkTrackerClick.SupportsVBScript,
                SupportsJavaScript = linkTrackerClick.SupportsJavaScript,
                SupportsJavaApplets = linkTrackerClick.SupportsJavaApplets,
                SupportsActiveXControls = linkTrackerClick.SupportsActiveXControls,
                JavaScriptVersion = linkTrackerClick.JavaScriptVersion,
                RecordOrder = linkTrackerClick.RecordOrder,
                Created = linkTrackerClick.Created,
                CreatedBy = linkTrackerClick.CreatedBy,
                LastUpdate = linkTrackerClick.LastUpdate,
                LastUpdateBy = linkTrackerClick.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Add new link tracker click
        /// </summary>
        /// <param name="linkTrackerClick"></param>
        /// <returns></returns>
        public ResponseModel AddLinkTrackerClick(LinkTrackerClick linkTrackerClick)
        {
            var response = Insert(linkTrackerClick);
            return response.SetMessage(response.Success
                ? T("LinkTrackerClick_Message_CreateSuccessfully")
                : T("LinkTrackerClick_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete the link tracker click by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteLinkTrackerClick(int id)
        {
            var item = GetById(id);
            if (item != null)
            {
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("LinkTrackerClick_Message_DeleteSuccessfully")
                    : T("LinkTrackerClick_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("LinkTrackerClick_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}