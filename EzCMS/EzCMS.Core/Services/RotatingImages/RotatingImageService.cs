using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.Mvc.Helpers;
using EzCMS.Core.Models.RotatingImages;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.RotatingImages
{
    public class RotatingImageService : ServiceHelper, IRotatingImageService
    {
        private readonly IRepository<RotatingImage> _rotatingImageRepository;

        public RotatingImageService(IRepository<RotatingImage> rotatingImageRepository)
        {
            _rotatingImageRepository = rotatingImageRepository;
        }

        #region Base

        public IQueryable<RotatingImage> GetAll()
        {
            return _rotatingImageRepository.GetAll();
        }

        public IQueryable<RotatingImage> Fetch(Expression<Func<RotatingImage, bool>> expression)
        {
            return _rotatingImageRepository.Fetch(expression);
        }

        public RotatingImage FetchFirst(Expression<Func<RotatingImage, bool>> expression)
        {
            return _rotatingImageRepository.FetchFirst(expression);
        }

        public RotatingImage GetById(object id)
        {
            return _rotatingImageRepository.GetById(id);
        }

        internal ResponseModel Insert(RotatingImage rotatingImage)
        {
            return _rotatingImageRepository.Insert(rotatingImage);
        }

        internal ResponseModel Update(RotatingImage rotatingImage)
        {
            return _rotatingImageRepository.Update(rotatingImage);
        }

        internal ResponseModel Delete(RotatingImage rotatingImage)
        {
            return _rotatingImageRepository.Delete(rotatingImage);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _rotatingImageRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Manage

        /// <summary>
        /// Get rotating image manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RotatingImageManageModel GetRotatingImageManageModel(int? id = null)
        {
            var rotatingImage = GetById(id);
            if (rotatingImage != null)
            {
                return new RotatingImageManageModel(rotatingImage);
            }
            return new RotatingImageManageModel();
        }

        /// <summary>
        /// Get rotating image manage model for group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public RotatingImageManageModel GetRotatingImageManageModelForGroup(int? id = null)
        {
            if (id.HasValue)
            {
                return new RotatingImageManageModel(id.Value);
            }
            return new RotatingImageManageModel();
        }

        /// <summary>
        /// Save rotating image
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveRotatingImage(RotatingImageManageModel model)
        {
            ResponseModel response;
            var rotatingImage = GetById(model.Id);
            if (rotatingImage != null)
            {
                rotatingImage.Title = model.Title;
                rotatingImage.ImageUrl = model.ImageUrl;
                rotatingImage.Text = model.Text;
                rotatingImage.Url = model.Url;
                rotatingImage.UrlTarget = model.UrlTarget;
                rotatingImage.GroupId = model.GroupId;

                response = Update(rotatingImage);
                return response.SetMessage(response.Success
                    ? T("RotatingImage_Message_UpdateSuccessfully")
                    : T("RotatingImage_Message_UpdateFailure"));
            }

            Mapper.CreateMap<RotatingImageManageModel, RotatingImage>();
            rotatingImage = Mapper.Map<RotatingImageManageModel, RotatingImage>(model);
            rotatingImage.RecordOrder = Fetch(i => i.GroupId == model.GroupId).Any()
                ? Fetch(i => i.GroupId == model.GroupId).Max(i => i.RecordOrder) + 1
                : 0;
            response = Insert(rotatingImage);
            return response.SetMessage(response.Success
                ? T("RotatingImage_Message_CreateSuccessfully")
                : T("RotatingImage_Message_CreateFailure"));
        }

        /// <summary>
        /// Update url of rotating image
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public ResponseModel UpdateRotatingImageUrl(int id, string url)
        {
            var image = GetById(id);
            if (image != null)
            {
                image.Url = url;
                var response = Update(image);
                return response.SetMessage(response.Success
                    ? T("RotatingImage_Message_UpdateSuccessfully")
                    : T("RotatingImage_Message_UpdateFailure"));
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("RotatingImage_Message_ObjectNotFound")
            };
        }

        /// <summary>
        /// Delete rotating image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel Delete(object id)
        {
            var item = GetById(id);
            if (item != null)
            {
                var response = Delete(id);

                return response.SetMessage(response.Success
                    ? T("RotatingImage_Message_DeleteSuccessfully")
                    : T("RotatingImage_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("RotatingImage_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}