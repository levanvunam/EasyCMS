using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.RotatingImages;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Services.RotatingImages
{
    [Register(Lifetime.PerInstance)]
    public interface IRotatingImageService : IBaseService<RotatingImage>
    {
        /// <summary>
        /// Delete rotating image
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel Delete(object id);

        /// <summary>
        /// Get rotating image manage model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RotatingImageManageModel GetRotatingImageManageModel(int? id = null);

        /// <summary>
        /// Get rotating image manage model for group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        RotatingImageManageModel GetRotatingImageManageModelForGroup(int? id = null);

        /// <summary>
        /// Save rotating image
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SaveRotatingImage(RotatingImageManageModel model);

        /// <summary>
        /// Update rotating image url
        /// </summary>
        /// <param name="id"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        ResponseModel UpdateRotatingImageUrl(int id, string url);
    }
}