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
using EzCMS.Core.Models.ProductOfInterests;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.ProductOfInterests
{
    public class ProductOfInterestService : ServiceHelper, IProductOfInterestService
    {
        private readonly IRepository<ProductOfInterest> _productOfInterestRepository;

        public ProductOfInterestService(IRepository<ProductOfInterest> productOfInterestRepository)
        {
            _productOfInterestRepository = productOfInterestRepository;
        }

        /// <summary>
        /// Get product of interest select list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetProductOfInterests()
        {
            return GetAll().Select(t => new SelectListItem
            {
                Text = t.Name,
                Value = SqlFunctions.StringConvert((double) t.Id).Trim()
            });
        }

        #region Base

        public IQueryable<ProductOfInterest> GetAll()
        {
            return _productOfInterestRepository.GetAll();
        }

        public IQueryable<ProductOfInterest> Fetch(Expression<Func<ProductOfInterest, bool>> expression)
        {
            return _productOfInterestRepository.Fetch(expression);
        }

        public ProductOfInterest FetchFirst(Expression<Func<ProductOfInterest, bool>> expression)
        {
            return _productOfInterestRepository.FetchFirst(expression);
        }

        public ProductOfInterest GetById(object id)
        {
            return _productOfInterestRepository.GetById(id);
        }

        internal ResponseModel Insert(ProductOfInterest productOfInterest)
        {
            return _productOfInterestRepository.Insert(productOfInterest);
        }

        internal ResponseModel Update(ProductOfInterest productOfInterest)
        {
            return _productOfInterestRepository.Update(productOfInterest);
        }

        internal ResponseModel Delete(ProductOfInterest productOfInterest)
        {
            return _productOfInterestRepository.Delete(productOfInterest);
        }

        internal ResponseModel Delete(object id)
        {
            return _productOfInterestRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _productOfInterestRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the product of interests
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchProductOfInterests(JqSearchIn si)
        {
            var data = GetAll();

            var productOfInterests = Maps(data);

            return si.Search(productOfInterests);
        }

        /// <summary>
        /// Export product of interests
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var productOfInterests = Maps(data);

            var exportData = si.Export(productOfInterests, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="productOfInterests"></param>
        /// <returns></returns>
        private IQueryable<ProductOfInterestModel> Maps(IQueryable<ProductOfInterest> productOfInterests)
        {
            return productOfInterests.Select(m => new ProductOfInterestModel
            {
                Id = m.Id,
                Name = m.Name,
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
        /// Get product of interest manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProductOfInterestManageModel GetProductOfInterestManageModel(int? id = null)
        {
            var productOfInterest = GetById(id);
            if (productOfInterest != null)
            {
                return new ProductOfInterestManageModel(productOfInterest);
            }
            return new ProductOfInterestManageModel();
        }

        /// <summary>
        /// Save product of interest
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveProductOfInterest(ProductOfInterestManageModel model)
        {
            ResponseModel response;
            var productOfInterest = GetById(model.Id);
            if (productOfInterest != null)
            {
                productOfInterest.Name = model.Name;
                productOfInterest.TargetCount = model.TargetCount;
                productOfInterest.RecordOrder = model.RecordOrder;
                response = Update(productOfInterest);
                return response.SetMessage(response.Success
                    ? T("ProductOfInterest_Message_UpdateSuccessfully")
                    : T("ProductOfInterest_Message_UpdateFailure"));
            }
            Mapper.CreateMap<ProductOfInterestManageModel, ProductOfInterest>();
            productOfInterest = Mapper.Map<ProductOfInterestManageModel, ProductOfInterest>(model);
            response = Insert(productOfInterest);

            return response.SetMessage(response.Success
                ? T("ProductOfInterest_Message_CreateSuccessfully")
                : T("ProductOfInterest_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete product of interest
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteProductOfInterest(int id)
        {
            var productOfInterest = GetById(id);
            if (productOfInterest != null)
            {
                var response = Delete(productOfInterest);
                return response.SetMessage(response.Success
                    ? T("ProductOfInterest_Message_DeleteSuccessfully")
                    : T("ProductOfInterest_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("ProductOfInterest_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}