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
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.Banners;
using EzCMS.Core.Models.Banners.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Banners
{
    public class BannerService : ServiceHelper, IBannerService
    {
        private readonly IRepository<Banner> _bannerRepository;

        public BannerService(IRepository<Banner> bannerRepository)
        {
            _bannerRepository = bannerRepository;
        }

        /// <summary>
        /// Get banner select list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetBanners()
        {
            return GetAll().Select(banner => new SelectListItem
            {
                Text = banner.Text,
                Value = SqlFunctions.StringConvert((double) banner.Id).Trim()
            });
        }

        /// <summary>
        /// Get banner widget
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BannerWidget GetBannerWidget(int id)
        {
            var banner = GetById(id);

            if (banner != null)
            {
                return new BannerWidget(banner);
            }

            return null;
        }

        #region Base

        public IQueryable<Banner> GetAll()
        {
            return _bannerRepository.GetAll();
        }

        public IQueryable<Banner> Fetch(Expression<Func<Banner, bool>> expression)
        {
            return _bannerRepository.Fetch(expression);
        }

        public Banner FetchFirst(Expression<Func<Banner, bool>> expression)
        {
            return _bannerRepository.FetchFirst(expression);
        }

        public Banner GetById(object id)
        {
            return _bannerRepository.GetById(id);
        }

        internal ResponseModel Insert(Banner banner)
        {
            return _bannerRepository.Insert(banner);
        }

        internal ResponseModel Update(Banner banner)
        {
            return _bannerRepository.Update(banner);
        }

        internal ResponseModel Delete(Banner banner)
        {
            return _bannerRepository.Delete(banner);
        }

        internal ResponseModel Delete(object id)
        {
            return _bannerRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _bannerRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the banners
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchGrid(JqSearchIn si)
        {
            var data = GetAll();

            var banners = Maps(data);

            return si.Search(banners);
        }

        /// <summary>
        /// Export banners
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var banners = Maps(data);

            var exportData = si.Export(banners, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="banners"></param>
        /// <returns></returns>
        private IQueryable<BannerModel> Maps(IQueryable<Banner> banners)
        {
            return banners.Select(b => new BannerModel
            {
                Id = b.Id,
                Text = b.Text,
                ImageUrl = b.ImageUrl,
                Url = b.Url,
                GroupName = b.GroupName,
                RecordOrder = b.RecordOrder,
                Created = b.Created,
                CreatedBy = b.CreatedBy,
                LastUpdate = b.LastUpdate,
                LastUpdateBy = b.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get banner manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BannerManageModel GetBannerManageModel(int? id = null)
        {
            var banner = GetById(id);
            if (banner != null)
            {
                return new BannerManageModel(banner);
            }
            return new BannerManageModel();
        }

        /// <summary>
        /// Save banner
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveBanner(BannerManageModel model)
        {
            ResponseModel response;
            var banner = GetById(model.Id);
            if (banner != null)
            {
                banner.ImageUrl = model.ImageUrl;
                banner.Text = model.Text;
                banner.Url = model.Url;
                banner.GroupName = model.GroupName;

                response = Update(banner);
                return response.SetMessage(response.Success
                    ? T("Banner_Message_UpdateSuccessfully")
                    : T("Banner_Message_UpdateFailure"));
            }
            Mapper.CreateMap<BannerManageModel, Banner>();
            banner = Mapper.Map<BannerManageModel, Banner>(model);
            response = Insert(banner);
            return response.SetMessage(response.Success
                ? T("Banner_Message_CreateSuccessfully")
                : T("Banner_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete banner
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteBanner(int id)
        {
            var banner = GetById(id);

            if (banner != null)
            {
                var response = Delete(banner);
                return response.SetMessage(response.Success
                    ? T("Banner_Message_DeleteSuccessfully")
                    : T("Banner_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Banner_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}