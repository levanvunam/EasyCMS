using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.SocialMedia;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.SocialMedia
{
    public class SocialMediaService : ServiceHelper, ISocialMediaService
    {
        private readonly IRepository<Entity.Entities.Models.SocialMedia> _socialMediaRepository;

        public SocialMediaService(IRepository<Entity.Entities.Models.SocialMedia> socialMediaRepository)
        {
            _socialMediaRepository = socialMediaRepository;
        }

        /// <summary>
        /// Get select list social media
        /// </summary>
        /// <param name="socialMediaId"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetSocialMediaList(int? socialMediaId = null)
        {
            return GetAll().Select(s => new SelectListItem
            {
                Text = s.Name,
                Value = SqlFunctions.StringConvert((double) s.Id).Trim(),
                Selected = socialMediaId == s.Id
            });
        }

        /// <summary>
        /// Get social media details model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SocialMediaDetailModel GetSocialMediaDetailModel(int id)
        {
            var socialMedia = GetById(id);

            if (socialMedia != null)
            {
                return new SocialMediaDetailModel(socialMedia);
            }

            return null;
        }

        #region Base

        public IQueryable<Entity.Entities.Models.SocialMedia> GetAll()
        {
            return _socialMediaRepository.GetAll();
        }

        public IQueryable<Entity.Entities.Models.SocialMedia> Fetch(
            Expression<Func<Entity.Entities.Models.SocialMedia, bool>> expression)
        {
            return _socialMediaRepository.Fetch(expression);
        }

        public Entity.Entities.Models.SocialMedia FetchFirst(
            Expression<Func<Entity.Entities.Models.SocialMedia, bool>> expression)
        {
            return _socialMediaRepository.FetchFirst(expression);
        }

        public Entity.Entities.Models.SocialMedia GetById(object id)
        {
            return _socialMediaRepository.GetById(id);
        }

        internal ResponseModel Insert(Entity.Entities.Models.SocialMedia socialMedia)
        {
            return _socialMediaRepository.Insert(socialMedia);
        }

        internal ResponseModel Update(Entity.Entities.Models.SocialMedia socialMedia)
        {
            return _socialMediaRepository.Update(socialMedia);
        }

        internal ResponseModel Delete(Entity.Entities.Models.SocialMedia socialMedia)
        {
            return _socialMediaRepository.Delete(socialMedia);
        }

        internal ResponseModel Delete(object id)
        {
            return _socialMediaRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _socialMediaRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the social media
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchSocialMedia(JqSearchIn si)
        {
            var data = GetAll();

            var models = Maps(data);

            return si.Search(models);
        }

        /// <summary>
        /// Export social media
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var socialMedia = Maps(data);

            var exportData = si.Export(socialMedia, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        private IQueryable<SocialMediaModel> Maps(IQueryable<Entity.Entities.Models.SocialMedia> entities)
        {
            return entities.Select(m => new SocialMediaModel
            {
                Id = m.Id,
                Name = m.Name,
                MaxCharacter = m.MaxCharacter,
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
        /// Get SocialMedia manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SocialMediaManageModel GetSocialMediaManageModel(int id)
        {
            var socialMedia = GetById(id);
            if (socialMedia != null)
            {
                return new SocialMediaManageModel(socialMedia);
            }

            return null;
        }

        /// <summary>
        /// Save Social Media
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveSocialMedia(SocialMediaManageModel model)
        {
            var socialMedia = GetById(model.Id);
            if (socialMedia != null)
            {
                socialMedia.MaxCharacter = model.MaxCharacter;
                socialMedia.RecordOrder = model.RecordOrder;

                var response = Update(socialMedia);
                return response.SetMessage(response.Success
                    ? T("SocialMedia_Message_UpdateSuccessfully")
                    : T("SocialMedia_Message_UpdateFailure"));
            }

            return new ResponseModel
            {
                Success = false,
                Message = T("SocialMedia_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}