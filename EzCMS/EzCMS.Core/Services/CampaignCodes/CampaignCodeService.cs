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
using EzCMS.Core.Models.CampaignCodes;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.CampaignCodes
{
    public class CampaignCodeService : ServiceHelper, ICampaignCodeService
    {
        private readonly IRepository<CampaignCode> _campaignCodeRepository;

        public CampaignCodeService(IRepository<CampaignCode> campaignCodeRepository)
        {
            _campaignCodeRepository = campaignCodeRepository;
        }

        /// <summary>
        /// Get campaign code select list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetCampaignCodes()
        {
            return GetAll().Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = SqlFunctions.StringConvert((double) t.Id).Trim()
            });
        }

        #region Base

        public IQueryable<CampaignCode> GetAll()
        {
            return _campaignCodeRepository.GetAll();
        }

        public IQueryable<CampaignCode> Fetch(Expression<Func<CampaignCode, bool>> expression)
        {
            return _campaignCodeRepository.Fetch(expression);
        }

        public CampaignCode FetchFirst(Expression<Func<CampaignCode, bool>> expression)
        {
            return _campaignCodeRepository.FetchFirst(expression);
        }

        public CampaignCode GetById(object id)
        {
            return _campaignCodeRepository.GetById(id);
        }

        internal ResponseModel Insert(CampaignCode campaignCode)
        {
            return _campaignCodeRepository.Insert(campaignCode);
        }

        internal ResponseModel Update(CampaignCode campaignCode)
        {
            return _campaignCodeRepository.Update(campaignCode);
        }

        internal ResponseModel Delete(CampaignCode campaignCode)
        {
            return _campaignCodeRepository.Delete(campaignCode);
        }

        internal ResponseModel Delete(object id)
        {
            return _campaignCodeRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _campaignCodeRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the campaign codes
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchCampaignCodes(JqSearchIn si)
        {
            var data = GetAll();

            var campaignCodes = Maps(data);

            return si.Search(campaignCodes);
        }

        /// <summary>
        /// Export campaign codes
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var campaignCodes = Maps(data);

            var exportData = si.Export(campaignCodes, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="campaignCodes"></param>
        /// <returns></returns>
        private IQueryable<CampaignCodeModel> Maps(IQueryable<CampaignCode> campaignCodes)
        {
            return campaignCodes.Select(m => new CampaignCodeModel
            {
                Id = m.Id,
                Name = m.Name,
                Description = m.Description,
                TargetCount = m.TargetCount,
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
        /// Get CampaignCode manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CampaignCodeManageModel GetCampaignCodeManageModel(int? id = null)
        {
            var campaignCode = GetById(id);
            if (campaignCode != null)
            {
                return new CampaignCodeManageModel(campaignCode);
            }
            return new CampaignCodeManageModel();
        }

        /// <summary>
        /// Save CampaignCode
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveCampaignCode(CampaignCodeManageModel model)
        {
            ResponseModel response;
            var campaignCode = GetById(model.Id);
            if (campaignCode != null)
            {
                campaignCode.Name = model.Name;
                campaignCode.TargetCount = model.TargetCount;
                campaignCode.RecordOrder = model.RecordOrder;
                campaignCode.Description = model.Description;
                response = Update(campaignCode);
                return response.SetMessage(response.Success
                    ? T("CampaignCode_Message_UpdateSuccessfully")
                    : T("CampaignCode_Message_UpdateFailure"));
            }
            Mapper.CreateMap<CampaignCodeManageModel, CampaignCode>();
            campaignCode = Mapper.Map<CampaignCodeManageModel, CampaignCode>(model);
            response = Insert(campaignCode);
            return response.SetMessage(response.Success
                ? T("CampaignCode_Message_CreateSuccessfully")
                : T("CampaignCode_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete compaingn code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteCampaignCode(int id)
        {
            var campaignCode = GetById(id);
            if (campaignCode != null)
            {
                var response = Delete(campaignCode);
                return response.SetMessage(response.Success
                    ? T("CampaignCode_Message_DeleteSuccessfully")
                    : T("CampaignCode_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("CampaignCode_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}