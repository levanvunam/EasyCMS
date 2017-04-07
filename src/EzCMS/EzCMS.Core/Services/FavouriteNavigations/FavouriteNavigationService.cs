using Ez.Framework.Core.Context;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.FavouriteNavigations;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace EzCMS.Core.Services.FavouriteNavigations
{
    public class FavouriteNavigationService : ServiceHelper, IFavouriteNavigationService
    {
        private readonly IRepository<FavouriteNavigation> _favouriteNavigationRepository;

        public FavouriteNavigationService(IRepository<FavouriteNavigation> favouriteNavigationRepository)
        {
            _favouriteNavigationRepository = favouriteNavigationRepository;
        }

        #region Base

        public IQueryable<FavouriteNavigation> GetAll()
        {
            return _favouriteNavigationRepository.GetAll();
        }

        public IQueryable<FavouriteNavigation> Fetch(Expression<Func<FavouriteNavigation, bool>> expression)
        {
            return _favouriteNavigationRepository.Fetch(expression);
        }

        public FavouriteNavigation FetchFirst(Expression<Func<FavouriteNavigation, bool>> expression)
        {
            return _favouriteNavigationRepository.FetchFirst(expression);
        }

        public FavouriteNavigation GetById(object id)
        {
            return _favouriteNavigationRepository.GetById(id);
        }

        internal ResponseModel Insert(FavouriteNavigation favouriteNavigation)
        {
            return _favouriteNavigationRepository.Insert(favouriteNavigation);
        }

        internal ResponseModel Update(FavouriteNavigation favouriteNavigation)
        {
            return _favouriteNavigationRepository.Update(favouriteNavigation);
        }

        internal ResponseModel Delete(FavouriteNavigation favouriteNavigation)
        {
            return _favouriteNavigationRepository.Delete(favouriteNavigation);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _favouriteNavigationRepository.SetRecordDeleted(id);
        }

        /// <summary>
        /// Search for favourite Navigations of current user
        /// </summary>
        /// <returns></returns>
        public IQueryable<FavouriteNavigation> GetCurrentUserFavouriteNavigations()
        {
            if (EzWorkContext.CurrentUserId != null)
                return Fetch(m => m.UserId == EzWorkContext.CurrentUserId).OrderBy(f => f.RecordOrder);
            return new List<FavouriteNavigation>().AsQueryable();
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the favourite Navigations
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchFavouriteNavigations(JqSearchIn si)
        {
            var data = GetCurrentUserFavouriteNavigations();

            var favouriteNavigations = Maps(data);

            return si.Search(favouriteNavigations);
        }

        /// <summary>
        /// Export favourite Navigations
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var favouriteNavigations = Maps(data);

            var exportData = si.Export(favouriteNavigations, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="favouriteNavigations"></param>
        /// <returns></returns>
        private IQueryable<FavouriteNavigationModel> Maps(IEnumerable<FavouriteNavigation> favouriteNavigations)
        {
            return favouriteNavigations.ToList().Select(m => new FavouriteNavigationModel
            {
                Id = m.Id,
                UserId = m.UserId,
                NavigationId = m.NavigationId,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            }).AsQueryable();
        }

        #endregion

        #region Manage

        /// <summary>
        /// Delete favourite Navigation
        /// </summary>
        /// <param name="favouriteNavigations"></param>
        /// <returns></returns>
        public ResponseModel Delete(IEnumerable<FavouriteNavigation> favouriteNavigations)
        {
            return _favouriteNavigationRepository.Delete(favouriteNavigations);
        }

        /// <summary>
        /// Add Navigation to favourite
        /// </summary>
        /// <param name="navigationId"></param>
        /// <returns></returns>
        public ResponseModel AddToFavourites(string navigationId)
        {
            ResponseModel response;
            try
            {
                if (EzWorkContext.CurrentUserId.HasValue)
                {
                    var favouriteNavigation =
                        FetchFirst(f => f.NavigationId == navigationId && f.UserId == EzWorkContext.CurrentUserId);

                    if (favouriteNavigation == null)
                    {
                        var currentFavouriteNavigations = GetCurrentUserFavouriteNavigations();
                        var order = currentFavouriteNavigations.Any()
                            ? currentFavouriteNavigations.Max(f => f.RecordOrder) + 1
                            : 1;
                        favouriteNavigation = new FavouriteNavigation
                        {
                            NavigationId = navigationId,
                            UserId = EzWorkContext.CurrentUserId.Value,
                            RecordOrder = order
                        };
                    }
                    response = Insert(favouriteNavigation);

                    response.SetMessage(response.Success
                        ? T("FavouriteNavigation_Message_CreateSuccessfully")
                        : T("FavouriteNavigation_Message_CreateFailure"));
                }
                else
                {
                    response = new ResponseModel
                    {
                        Message = T("User_Message_UserDoesNotExist"),
                        Success = false
                    };
                }
            }
            catch (Exception exception)
            {
                response = new ResponseModel
                {
                    Success = false,
                    Message = exception.Message
                };
            }

            return response;
        }

        /// <summary>
        /// Remove Navigation from favourite list
        /// </summary>
        /// <param name="navigationId"></param>
        /// <returns></returns>
        public ResponseModel RemoveFromFavourites(string navigationId)
        {
            ResponseModel response;
            try
            {
                var favouriteNavigation =
                    FetchFirst(f => f.NavigationId == navigationId && f.UserId == EzWorkContext.CurrentUserId);

                if (favouriteNavigation != null)
                {
                    response = Delete(favouriteNavigation);
                }
                else
                {
                    response = new ResponseModel
                    {
                        Success = true
                    };
                }

                response.SetMessage(response.Success
                    ? T("FavouriteNavigation_Message_DeleteSuccessfully")
                    : T("FavouriteNavigation_Message_DeleteFailure"));
            }
            catch (Exception exception)
            {
                response = new ResponseModel
                {
                    Success = false,
                    Message = exception.Message
                };
            }

            return response;
        }

        /// <summary>
        /// Move current favourite Navigation up
        /// </summary>
        /// <param name="favouriteNavigationId"></param>
        /// <returns></returns>
        public ResponseModel MoveDown(int favouriteNavigationId)
        {
            var currentNavigation = GetById(favouriteNavigationId);
            if (currentNavigation == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("FavouriteNavigation_Message_ObjectNotFound")
                };
            }
            var greaterNavigation =
                GetCurrentUserFavouriteNavigations()
                    .Where(f => f.RecordOrder > currentNavigation.RecordOrder)
                    .OrderBy(f => f.RecordOrder)
                    .FirstOrDefault();
            if (greaterNavigation != null)
            {
                var order = currentNavigation.RecordOrder;
                currentNavigation.RecordOrder = greaterNavigation.RecordOrder;
                greaterNavigation.RecordOrder = order;

                Update(currentNavigation);
                Update(greaterNavigation);
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("FavouriteNavigation_Message_ChangePositionSuccessfully")
            };
        }

        /// <summary>
        /// Move current favourite Navigation down
        /// </summary>
        /// <param name="favouriteNavigationId"></param>
        /// <returns></returns>
        public ResponseModel MoveUp(int favouriteNavigationId)
        {
            var currentNavigation = GetById(favouriteNavigationId);
            if (currentNavigation == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("FavouriteNavigation_Message_ObjectNotFound")
                };
            }
            var lowerNavigation =
                GetCurrentUserFavouriteNavigations()
                    .Where(f => f.RecordOrder < currentNavigation.RecordOrder)
                    .OrderByDescending(f => f.RecordOrder)
                    .FirstOrDefault();
            if (lowerNavigation != null)
            {
                var order = currentNavigation.RecordOrder;
                currentNavigation.RecordOrder = lowerNavigation.RecordOrder;
                lowerNavigation.RecordOrder = order;

                Update(currentNavigation);
                Update(lowerNavigation);
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("FavouriteNavigation_Message_ChangePositionSuccessfully")
            };
        }

        /// <summary>
        /// Delete favourite Navigation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel Delete(int id)
        {
            var response = _favouriteNavigationRepository.Delete(id);

            return response.SetMessage(response.Success
                ? T("FavouriteNavigation_Message_DeleteSuccessfully")
                : T("FavouriteNavigation_Message_DeleteFailure"));
        }

        #endregion
    }
}