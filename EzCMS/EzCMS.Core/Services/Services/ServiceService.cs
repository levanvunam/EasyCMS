using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Services;
using EzCMS.Core.Models.Services.Widgets;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Services
{
    public class ServiceService : ServiceHelper, IServiceService
    {
        private readonly IRepository<Service> _serviceRepository;

        public ServiceService(IRepository<Service> serviceRepository)
        {
            _serviceRepository = serviceRepository;
        }

        /// <summary>
        /// Get service status
        /// </summary>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetStatus()
        {
            return EnumUtilities.GenerateSelectListItems<ServiceEnums.ServiceStatus>();
        }

        /// <summary>
        /// Check if title exists
        /// </summary>
        /// <param name="serviceId">the Service id</param>
        /// <param name="title">the Service title</param>
        /// <returns></returns>
        public bool IsNameExisted(int? serviceId, string title)
        {
            return Fetch(u => u.Name.Equals(title) && u.Id != serviceId).Any();
        }

        public List<ServiceWidget> GetServices(int number)
        {
            return Fetch(s => s.Status == ServiceEnums.ServiceStatus.Active)
                .OrderBy(m => m.RecordOrder)
                .Take(number)
                .ToList().Select(s => new ServiceWidget
                {
                    Id = s.Id,
                    Name = s.Name,
                    Description = s.Description,
                    Content = s.Content,
                    ImageUrl = s.ImageUrl,
                    RecordOrder = s.RecordOrder,
                    Created = s.Created,
                    CreatedBy = s.CreatedBy,
                    LastUpdate = s.LastUpdate,
                    LastUpdateBy = s.LastUpdateBy
                }).ToList();
        }

        /// <summary>
        /// Get service detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ServiceDetailModel GetServiceDetailModel(int id)
        {
            var service = GetById(id);
            return service != null ? new ServiceDetailModel(service) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateServiceData(XEditableModel model)
        {
            var service = GetById(model.Pk);
            if (service != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (ServiceManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new ServiceManageModel(service);
                    manageModel.SetProperty(model.Name, value);

                    var validationResults = manageModel.ValidateModel();

                    if (validationResults.Any())
                    {
                        return new ResponseModel
                        {
                            Success = false,
                            Message = validationResults.BuildValidationMessages()
                        };
                    }

                    #endregion

                    service.SetProperty(model.Name, value);
                    var response = Update(service);
                    return response.SetMessage(response.Success
                        ? T("Service_Message_UpdateServiceInfoSuccessfully")
                        : T("Service_Message_UpdateServiceInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Service_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Service_Message_ObjectNotFound")
            };
        }

        #region Validation

        /// <summary>
        /// Check if service exists
        /// </summary>
        /// <param name="serviceId"></param>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public bool IsServiceExisted(int? serviceId, string serviceName)
        {
            return Fetch(u => u.Name.Equals(serviceName) && u.Id != serviceId).Any();
        }

        #endregion

        #region Base

        public IQueryable<Service> GetAll()
        {
            return _serviceRepository.GetAll();
        }

        public IQueryable<Service> Fetch(Expression<Func<Service, bool>> expression)
        {
            return _serviceRepository.Fetch(expression);
        }

        public Service FetchFirst(Expression<Func<Service, bool>> expression)
        {
            return _serviceRepository.FetchFirst(expression);
        }

        public Service GetById(object id)
        {
            return _serviceRepository.GetById(id);
        }

        internal ResponseModel Insert(Service service)
        {
            return _serviceRepository.Insert(service);
        }

        internal ResponseModel Update(Service service)
        {
            return _serviceRepository.Update(service);
        }

        internal ResponseModel Delete(Service service)
        {
            return _serviceRepository.Delete(service);
        }

        internal ResponseModel Delete(object id)
        {
            return _serviceRepository.Delete(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the services
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchService(JqSearchIn si)
        {
            var data = GetAll();

            var services = Maps(data);

            return si.Search(services);
        }

        /// <summary>
        /// Export services
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var services = Maps(data);

            var exportData = si.Export(services, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }


        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private IQueryable<ServiceModel> Maps(IQueryable<Service> services)
        {
            return services.Select(u => new ServiceModel
            {
                Id = u.Id,
                Name = u.Name,
                Description = u.Description,
                ImageUrl = u.ImageUrl,
                Content = u.Content,
                Status = u.Status,
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
        /// Get Service manage model by id
        /// </summary>
        /// <param name="id">the Service id</param>
        /// <returns></returns>
        public ServiceManageModel GetServiceManageModel(int? id = null)
        {
            var service = GetById(id);
            if (service != null)
            {
                return new ServiceManageModel
                {
                    Id = service.Id,
                    Description = service.Description,
                    Content = service.Content,
                    ImageUrl = service.ImageUrl,
                    Name = service.Name,
                    Status = service.Status,
                    StatusList = GetStatus()
                };
            }
            return new ServiceManageModel
            {
                StatusList = GetStatus()
            };
        }

        /// <summary>
        /// Save Service manage model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveServiceManageModel(ServiceManageModel model)
        {
            ResponseModel response;
            var service = GetById(model.Id);

            #region Edit Service

            if (service != null)
            {
                service.Name = model.Name;

                service.Status = model.Status;
                service.Description = model.Description;
                service.Content = model.Content;
                service.ImageUrl = model.ImageUrl;

                //Get page record order
                response = Update(service);
                return response.SetMessage(response.Success
                    ? T("Service_Message_UpdateSuccessfully")
                    : T("Service_Message_UpdateFailure"));
            }

            #endregion

            service = new Service
            {
                Name = model.Name,
                Status = model.Status,
                Description = model.Description,
                Content = model.Content,
                ImageUrl = model.ImageUrl
            };
            response = Insert(service);
            return response.SetMessage(response.Success
                ? T("Service_Message_CreateSuccessfully")
                : T("Service_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete service
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteService(int id)
        {
            var service = GetById(id);
            if (service != null)
            {
                var response = Delete(service);
                return response.SetMessage(response.Success
                    ? T("Service_Message_DeleteSuccessfully")
                    : T("Service_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("Service_Message_DeleteSuccessfully")
            };
        }

        #endregion
    }
}