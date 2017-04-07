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
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.Countries;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Countries
{
    public class CountryService : ServiceHelper, ICountryService
    {
        private readonly IRepository<Country> _countryRepository;

        public CountryService(IRepository<Country> countryRepository)
        {
            _countryRepository = countryRepository;
        }

        #region Base

        public IQueryable<Country> GetAll()
        {
            return _countryRepository.GetAll();
        }

        public IQueryable<Country> Fetch(Expression<Func<Country, bool>> expression)
        {
            return _countryRepository.Fetch(expression);
        }

        public Country FetchFirst(Expression<Func<Country, bool>> expression)
        {
            return _countryRepository.FetchFirst(expression);
        }

        public Country GetById(object id)
        {
            return _countryRepository.GetById(id);
        }

        internal ResponseModel Insert(Country country)
        {
            return _countryRepository.Insert(country);
        }

        internal ResponseModel Update(Country country)
        {
            return _countryRepository.Update(country);
        }

        internal ResponseModel Delete(Country country)
        {
            return _countryRepository.Delete(country);
        }

        internal ResponseModel Delete(object id)
        {
            return _countryRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _countryRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the countries
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchCountries(JqSearchIn si)
        {
            var data = GetAll();

            var countries = Maps(data);

            return si.Search(countries);
        }

        /// <summary>
        /// Export Countries
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var countries = Maps(data);

            var exportData = si.Export(countries, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="countries"></param>
        /// <returns></returns>
        private IQueryable<CountryModel> Maps(IQueryable<Country> countries)
        {
            return countries.Select(m => new CountryModel
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
        /// Get Country manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CountryManageModel GetCountryManageModel(int? id = null)
        {
            var country = GetById(id);
            if (country != null)
            {
                return new CountryManageModel(country);
            }
            return new CountryManageModel();
        }

        /// <summary>
        /// Save Country
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveCountry(CountryManageModel model)
        {
            ResponseModel response;
            var country = GetById(model.Id);
            if (country != null)
            {
                country.Name = model.Name;
                country.RecordOrder = model.RecordOrder;
                response = Update(country);
                return response.SetMessage(response.Success
                    ? T("Country_Message_UpdateSuccessfully")
                    : T("Country_Message_UpdateFailure"));
            }
            Mapper.CreateMap<CountryManageModel, Country>();
            country = Mapper.Map<CountryManageModel, Country>(model);
            response = Insert(country);
            return response.SetMessage(response.Success
                ? T("Country_Message_CreateSuccessfully")
                : T("Country_Message_CreateFailure"));
        }

        /// <summary>
        /// Get countries
        /// </summary>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetCountries(string keyword = "")
        {
            return
                Fetch(c => string.IsNullOrEmpty(keyword) || c.Name.ToLower().Contains(keyword.ToLower()))
                    .Select(c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Name,
                        Selected = keyword.ToLower().Equals(c.Name.ToLower())
                    });
        }

        #endregion
    }
}