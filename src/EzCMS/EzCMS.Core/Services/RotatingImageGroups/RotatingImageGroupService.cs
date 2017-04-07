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
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Models.RotatingImageGroups;
using EzCMS.Core.Models.RotatingImageGroups.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.RotatingImageGroups
{
    public class RotatingImageGroupService : ServiceHelper, IRotatingImageGroupService
    {
        private readonly IRepository<RotatingImageGroup> _rotatingImageGroupRepository;
        private readonly IRepository<RotatingImage> _rotatingImageRepository;

        public RotatingImageGroupService(IRepository<RotatingImageGroup> rotatingImageGroupRepository,
            IRepository<RotatingImage> rotatingImageRepository)
        {
            _rotatingImageGroupRepository = rotatingImageGroupRepository;
            _rotatingImageRepository = rotatingImageRepository;
        }

        /// <summary>
        /// Gets the rotating image groups.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetRotatingImageGroups(int? groupId = null)
        {
            return GetAll().Select(rotatingImageGroup => new SelectListItem
            {
                Text = rotatingImageGroup.Name,
                Value = SqlFunctions.StringConvert((double) rotatingImageGroup.Id).Trim(),
                Selected = groupId.HasValue && groupId == rotatingImageGroup.Id
            });
        }

        /// <summary>
        /// Get group rotating image manage model
        /// </summary>
        /// <param name="id">the group id</param>
        /// <returns></returns>
        public GroupManageSettingModel GetGroupManageSettingModel(int id)
        {
            var group = GetById(id);
            if (group != null)
            {
                var model = new GroupManageSettingModel
                {
                    Id = id,
                    GroupSettingModel = SerializeUtilities.Deserialize<GroupSettingModel>(@group.Settings)
                };
                return model;
            }
            return null;
        }

        /// <summary>
        /// Save group settings
        /// </summary>
        /// <param name="model">the group setting model</param>
        /// <returns></returns>
        public ResponseModel SaveGroupSettings(GroupManageSettingModel model)
        {
            var group = GetById(model.Id);
            if (group != null)
            {
                group.Settings = SerializeUtilities.Serialize(model.GroupSettingModel);
                var response = Update(group);

                return response.SetMessage(response.Success
                    ? T("RotatingImageGroup_Message_UpdateSuccessfully")
                    : T("RotatingImageGroup_Message_UpdateFailure"));
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("RotatingImageGroup_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Get all rotating image of gallery
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RotatingImagesWidget GetRotatingImageWidget(int id)
        {
            var group = GetById(id);
            if (group != null)
            {
                var images = GetById(id).RotatingImages.OrderBy(i => i.RecordOrder);
                var index = 0;
                return new RotatingImagesWidget
                {
                    Id = id,
                    GroupName = group.Name,
                    Images = images.Select(i => new RotatingImageWidget
                    {
                        Id = i.Id,
                        Index = index++,
                        ImageUrl = i.ImageUrl,
                        Title = i.Title,
                        Text = i.Text,
                        Url = i.Url,
                        UrlTarget = i.UrlTarget,
                        RecordOrder = i.RecordOrder
                    }).ToList()
                };
            }
            return null;
        }

        /// <summary>
        /// Sort images by group images sort model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SortImages(GroupImageSortingModel model)
        {
            var group = GetById(model.GroupId);
            if (group != null)
            {
                var images = group.RotatingImages.OrderBy(i => model.Ids.ToList().IndexOf(i.Id)).ToList();
                var dictionary = images.OrderBy(i => i.RecordOrder).Select(i => new {i.Id, i.RecordOrder}).ToList();
                var index = 0;
                foreach (var image in images)
                {
                    if (image.Id != dictionary[index].Id)
                    {
                        image.RecordOrder = dictionary[index].RecordOrder;
                        _rotatingImageRepository.Update(image);
                    }
                    index++;
                }
                return new ResponseModel
                {
                    Success = true,
                    Message = T("RotatingImage_Message_SortImageSuccessfully")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("RotatingImageGroup_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete rotating image group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteRotatingImageGroup(int id)
        {
            var rotatingImageGroup = GetById(id);
            if (rotatingImageGroup != null)
            {
                if (rotatingImageGroup.RotatingImages.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("RotatingImageGroup_Message_DeleteFailureBasedOnRelatedRotatingImages")
                    };
                }

                var response = Delete(rotatingImageGroup);

                return response.SetMessage(response.Success
                    ? T("RotatingImageGroup_Message_DeleteSuccessfully")
                    : T("RotatingImageGroup_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("RotatingImageGroup_Message_DeleteSuccessfully")
            };
        }

        #region Base

        public IQueryable<RotatingImageGroup> GetAll()
        {
            return _rotatingImageGroupRepository.GetAll();
        }

        public IQueryable<RotatingImageGroup> Fetch(Expression<Func<RotatingImageGroup, bool>> expression)
        {
            return _rotatingImageGroupRepository.Fetch(expression);
        }

        public RotatingImageGroup FetchFirst(Expression<Func<RotatingImageGroup, bool>> expression)
        {
            return _rotatingImageGroupRepository.FetchFirst(expression);
        }

        public RotatingImageGroup GetById(object id)
        {
            return _rotatingImageGroupRepository.GetById(id);
        }

        internal ResponseModel Insert(RotatingImageGroup rotatingImageGroup)
        {
            return _rotatingImageGroupRepository.Insert(rotatingImageGroup);
        }

        internal ResponseModel Update(RotatingImageGroup rotatingImageGroup)
        {
            return _rotatingImageGroupRepository.Update(rotatingImageGroup);
        }

        internal ResponseModel Delete(RotatingImageGroup rotatingImageGroup)
        {
            return _rotatingImageGroupRepository.Delete(rotatingImageGroup);
        }

        internal ResponseModel Delete(object id)
        {
            return _rotatingImageGroupRepository.Delete(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the rotating image groups
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchRotatingImageGroups(JqSearchIn si)
        {
            var data = GetAll();
            var rotatingImageGroups = Maps(data);

            return si.Search(rotatingImageGroups);
        }

        /// <summary>
        /// Export rotating image groups
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var rotatingImageGroups = Maps(data);

            var exportData = si.Export(rotatingImageGroups, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="rotatingImageGroups"></param>
        /// <returns></returns>
        private IQueryable<RotatingImageGroupModel> Maps(IQueryable<RotatingImageGroup> rotatingImageGroups)
        {
            return rotatingImageGroups.Select(u => new RotatingImageGroupModel
            {
                Id = u.Id,
                Name = u.Name,
                Settings = u.Settings,
                RecordOrder = u.RecordOrder,
                Created = u.Created,
                CreatedBy = u.CreatedBy,
                LastUpdate = u.LastUpdate,
                LastUpdateBy = u.LastUpdateBy
            });
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get Rotating Image Group manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RotatingImageGroupManageModel GetRotatingImageGroupManageModel(int? id = null)
        {
            var rotatingImageGroup = GetById(id);
            if (rotatingImageGroup != null)
            {
                return new RotatingImageGroupManageModel(rotatingImageGroup);
            }
            return new RotatingImageGroupManageModel();
        }

        /// <summary>
        /// Save RotatingImageGroup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveRotatingImageGroup(RotatingImageGroupManageModel model)
        {
            ResponseModel response;
            var rotatingImageGroup = GetById(model.Id);
            if (rotatingImageGroup != null)
            {
                rotatingImageGroup.Name = model.Name;
                response = Update(rotatingImageGroup);
                return response.SetMessage(response.Success
                    ? T("RotatingImageGroup_Message_UpdateSuccessfully")
                    : T("RotatingImageGroup_Message_UpdateFailure"));
            }
            Mapper.CreateMap<RotatingImageGroupManageModel, RotatingImageGroup>();
            rotatingImageGroup = Mapper.Map<RotatingImageGroupManageModel, RotatingImageGroup>(model);
            response = Insert(rotatingImageGroup);
            return response.SetMessage(response.Success
                ? T("RotatingImageGroup_Message_CreateSuccessfully")
                : T("RotatingImageGroup_Message_CreateFailure"));
        }

        #endregion
    }
}