using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
using Ez.Framework.Core.Entity.RepositoryBase;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Logging;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Contacts;
using EzCMS.Core.Models.Pages;
using EzCMS.Core.Models.Subscriptions;
using EzCMS.Core.Models.Subscriptions.Widgets;
using EzCMS.Core.Models.Subscriptions.Widgets.ContentUpdates;
using EzCMS.Core.Models.Subscriptions.ModuleParameters;
using EzCMS.Core.Models.Subscriptions.Subscribers;
using EzCMS.Core.Models.Subscriptions.SubscriptionTemplates;
using EzCMS.Core.Models.Subscriptions.SubscriptionTemplates.Base;
using EzCMS.Core.Services.Contacts;
using EzCMS.Core.Services.Pages;
using EzCMS.Core.Services.SubscriptionLogs;
using EzCMS.Core.Services.SubscriptionTemplates;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Subscriptions
{
    public class SubscriptionService : ServiceHelper, ISubscriptionService
    {
        private readonly IContactService _contactService;
        private readonly ILogger _logger;
        private readonly IPageService _pageService;
        private readonly ISubscriptionLogService _subscriptionLogService;
        private readonly IRepository<Subscription> _subscriptionRepository;

        public SubscriptionService(IRepository<Subscription> subscriptionRepository, IContactService contactService,
            IPageService pageService, ISubscriptionLogService subscriptionLogService, ILogger logger)
        {
            _subscriptionRepository = subscriptionRepository;
            _contactService = contactService;
            _pageService = pageService;
            _subscriptionLogService = subscriptionLogService;
            _logger = logger;
        }

        /// <summary>
        /// Register subscription
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel Register(SubscriptionManageModel model)
        {
            // Decode the parameter because maybe when sending to server it is in raw format
            model.Parameters = model.Parameters.Decode();

            var response = new ResponseModel();
            var subscriptions = new List<SubscriptionManageModel>();
            var contacts = _contactService.SearchContactsByEmail(model.Email).ToList();

            try
            {
                if (contacts.Any())
                {
                    foreach (var contact in contacts)
                    {
                        contact.SubscriptionType = model.SubscriptionType;

                        // Update contact
                        _contactService.SaveContact(new ContactManageModel(contact));

                        // Save contact to subscribers
                        subscriptions.Add(new SubscriptionManageModel
                        {
                            ContactId = contact.Id,
                            Email = model.Email,
                            Module = model.Module,
                            Parameters = model.Parameters
                        });
                    }
                }
                else
                {
                    var contact = new ContactManageModel
                    {
                        Email = model.Email,
                        SubscriptionType = model.SubscriptionType
                    };

                    response = _contactService.SaveContact(contact);

                    if (response.Success)
                    {
                        model.ContactId = response.Data.ToNullableInt();
                    }

                    subscriptions.Add(new SubscriptionManageModel
                    {
                        ContactId = model.ContactId,
                        Email = model.Email,
                        Module = model.Module,
                        Parameters = model.Parameters
                    });
                }

                foreach (var subscription in subscriptions)
                {
                    var existedSubscription = GetSubscription(subscription.Module, subscription.Email,
                        subscription.Parameters);
                    if (existedSubscription == null)
                    {
                        Mapper.CreateMap<SubscriptionManageModel, Subscription>();
                        var newSubscription =
                            Mapper.Map<SubscriptionManageModel, Subscription>(subscription);
                        Insert(newSubscription);
                    }
                }

                WorkContext.CurrentContact.Email = model.Email;
                return new ResponseModel
                {
                    Success = true,
                    Message = T("Subscription_Message_SubscribeSuccessfully")
                };
            }
            catch (Exception exception)
            {
                _logger.Error(exception);
                response.SetMessage(T("Subscription_Message_SubscribeFailure"));
            }

            return response;
        }

        /// <summary>
        /// Register subscription
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel RemoveRegistration(SubscriptionManageModel model)
        {
            ResponseModel response;

            // Decode the parameter because maybe when sending to server it is in raw format
            model.Parameters = model.Parameters.Decode();

            var subscriptionModel = GetSubscription(model.Module, WorkContext.CurrentContact.Email, model.Parameters);
            var subscription = subscriptionModel != null ? GetById(subscriptionModel.Id) : null;

            if (subscription != null)
            {
                subscription.Active = false;
                subscription.DeactivatedDate = DateTime.UtcNow;
                response = Update(subscription);
            }
            else
            {
                response = new ResponseModel
                {
                    Success = true
                };
            }

            return response.Success
                ? response.SetMessage(T("Subscription_Message_RemoveSubscriptionSuccessfully"))
                : response.SetMessage(T("Subscription_Message_RemoveSubscriptionFailure"));
        }

        #region Manage

        /// <summary>
        /// Get Subscription manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public SubscriptionManageModel GetSubscriptionManageModel(int? id = null)
        {
            var subscription = GetById(id);
            if (subscription != null)
            {
                return new SubscriptionManageModel(subscription);
            }
            return new SubscriptionManageModel();
        }

        #endregion

        /// <summary>
        /// Get subscription by module and parameters
        /// </summary>
        /// <param name="module"></param>
        /// <param name="email"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SubscriptionModel GetSubscription(SubscriptionEnums.SubscriptionModule module, string email,
            dynamic parameters)
        {
            if (parameters != null)
            {
                var data = Fetch(s => s.Active && s.Module == module && s.Email.Equals(email)).ToList();

                if (data.Any())
                {
                    var subscriptions = data.Select(s => new SubscriptionModel(s));

                    switch (module)
                    {
                        case SubscriptionEnums.SubscriptionModule.Page:
                            var param = (SubscriptionPageParameterModel) parameters;
                            var subscription =
                                subscriptions.FirstOrDefault(
                                    s => ((SubscriptionPageParameterModel) s.Parameters).Id == param.Id);
                            if (subscription != null)
                            {
                                return subscription;
                            }
                            break;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Get subscription by module and parameters
        /// </summary>
        /// <param name="module"></param>
        /// <param name="email"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SubscriptionModel GetSubscription(SubscriptionEnums.SubscriptionModule module, string email,
            string parameters)
        {
            if (parameters != null)
            {
                var data = Fetch(s => s.Active && s.Module == module && s.Email.Equals(email)).ToList();

                if (data.Any())
                {
                    var subscriptions = data.Select(s => new SubscriptionModel(s));

                    switch (module)
                    {
                        case SubscriptionEnums.SubscriptionModule.Page:
                            var param = SerializeUtilities.Deserialize<SubscriptionPageParameterModel>(parameters);
                            var subscription =
                                subscriptions.FirstOrDefault(
                                    s => ((SubscriptionPageParameterModel) s.Parameters).Id == param.Id);
                            if (subscription != null)
                            {
                                return subscription;
                            }
                            break;
                    }
                }
            }

            return null;
        }

        #region Base

        public IQueryable<Subscription> GetAll()
        {
            return _subscriptionRepository.GetAll();
        }

        public IQueryable<Subscription> Fetch(Expression<Func<Subscription, bool>> expression)
        {
            return _subscriptionRepository.Fetch(expression);
        }

        public Subscription FetchFirst(Expression<Func<Subscription, bool>> expression)
        {
            return _subscriptionRepository.FetchFirst(expression);
        }

        public Subscription GetById(object id)
        {
            return _subscriptionRepository.GetById(id);
        }

        internal ResponseModel Insert(Subscription subscription)
        {
            return _subscriptionRepository.Insert(subscription);
        }

        internal ResponseModel Update(Subscription subscription)
        {
            return _subscriptionRepository.Update(subscription);
        }

        internal ResponseModel Delete(Subscription subscription)
        {
            return _subscriptionRepository.Delete(subscription);
        }

        internal ResponseModel Delete(object id)
        {
            return _subscriptionRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _subscriptionRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the subscriptions
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchSubscriptions(JqSearchIn si)
        {
            var data = GetAll();

            var subscriptions = Maps(data);

            return si.Search(subscriptions);
        }

        /// <summary>
        /// Export subscriptions
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var subscriptions = Maps(data);

            var exportData = si.Export(subscriptions, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="subscriptions"></param>
        /// <returns></returns>
        private IQueryable<SubscriptionModel> Maps(IQueryable<Subscription> subscriptions)
        {
            return subscriptions.Select(m => new SubscriptionModel
            {
                Id = m.Id,
                Module = m.Module,
                Email = m.Email,
                Parameters = m.Parameters,
                DeactivatedDate = m.DeactivatedDate,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Subscription Action

        /// <summary>
        /// Handle subscription actions
        /// </summary>
        /// <param name="subscriptionAction"></param>
        /// <param name="subscriptionId"></param>
        /// <returns></returns>
        public ResponseModel TriggerSubscriptionAction(SubscriptionEnums.SubscriptionAction subscriptionAction,
            int subscriptionId)
        {
            ResponseModel response;

            var subscription = GetById(subscriptionId);

            // If wrong input parameters then returl wrong subscription url
            if (subscription == null)
            {
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Subscription_Message_WrongSubscription")
                };
            }

            WorkContext.CurrentContact.Email = subscription.Email;
            switch (subscription.Module)
            {
                case SubscriptionEnums.SubscriptionModule.Page:
                    response = GetPageSubscriptionResult(subscriptionAction, subscription);
                    break;

                default:
                    response = new ResponseModel
                    {
                        Success = false,
                        Message = T("Subscription_Message_WrongSubscription")
                    };
                    break;
            }

            return response;
        }

        #region Private Methods

        /// <summary>
        /// Get page subscription result from action
        /// </summary>
        /// <param name="subscriptionAction"></param>
        /// <param name="subscription"></param>
        /// <returns></returns>
        private ResponseModel GetPageSubscriptionResult(SubscriptionEnums.SubscriptionAction subscriptionAction,
            Subscription subscription)
        {
            ResponseModel response;

            var parameters = SerializeUtilities.Deserialize<SubscriptionPageParameterModel>(subscription.Parameters);
            var page = _pageService.GetById(parameters.Id);
            if (page != null)
            {
                switch (subscriptionAction)
                {
                    case SubscriptionEnums.SubscriptionAction.View:
                        response = new ResponseModel
                        {
                            Success = true,
                            Data = "/" + page.FriendlyUrl
                        };
                        break;
                    case SubscriptionEnums.SubscriptionAction.Remove:
                        var subscriptionManageModel = new SubscriptionManageModel(subscription);
                        response = RemoveRegistration(subscriptionManageModel);
                        if (response.Success)
                        {
                            response.Message = T("Subscription_Message_PageUnsubscribeSuccessfully");
                        }
                        break;
                    default:
                        response = new ResponseModel
                        {
                            Success = false,
                            Message = T("Subscription_Message_WrongSubscription")
                        };
                        break;
                }
            }
            else
            {
                response = new ResponseModel
                {
                    Success = false,
                    Message = T("Subscription_Message_WrongSubscription")
                };
            }
            return response;
        }

        #endregion

        #endregion

        #region Background Tasks

        /// <summary>
        /// Get final html from subscription info and template
        /// </summary>
        /// <param name="subscriberInfo"></param>
        /// <param name="subscriptionLogs"></param>
        /// <param name="template"></param>
        /// <param name="isAnythingChanged"></param>
        /// <returns></returns>
        public string GetSubscriptionEmailContent(IEnumerable<SubscribeModule> subscriberInfo,
            IEnumerable<SubscriptionLog> subscriptionLogs, SubscriptionTemplate template, ref bool isAnythingChanged)
        {
            // Map the subscription logs into the subscribe info of a subscriber
            var logs =
                subscriberInfo.Join(subscriptionLogs, subscribeModule => subscribeModule.Parameters,
                    subscriptionLog => subscriptionLog.Parameters,
                    (subscribeModule, subscriptionLog) => new
                    {
                        SubscribeModule = subscribeModule,
                        SubscriptionLog = subscriptionLog
                    })
                    .GroupBy(l => l.SubscribeModule.SubscriptionId)
                    .Select(l => new SubscriptionEmailModel
                    {
                        SubscriptionId = l.Key,
                        Parameters = l.First().SubscribeModule.Parameters,
                        Logs = l.Select(m => new SubscriptionLogItem(m.SubscriptionLog)).ToList()
                    }).ToList();

            // Build html for specific module
            switch (template.Module)
            {
                case SubscriptionEnums.SubscriptionModule.Page:
                    return GetPageSubscriptionEmailContent(logs, template, ref isAnythingChanged);
                default:
                    return string.Empty;
            }
        }

        #region Private Methods

        /// <summary>
        /// Get page subscription email content
        /// </summary>
        /// <param name="emailModels"></param>
        /// <param name="template"></param>
        /// <param name="isAnythingChanged"></param>
        /// <returns></returns>
        private string GetPageSubscriptionEmailContent(List<SubscriptionEmailModel> emailModels,
            SubscriptionTemplate template, ref bool isAnythingChanged)
        {
            var pageService = HostContainer.GetInstance<IPageService>();
            var subscriptionTemplateService = HostContainer.GetInstance<ISubscriptionTemplateService>();

            // Get a list of pageId in SubscriptionEmailModel
            var pageIds =
                emailModels.Select(l => SerializeUtilities.Deserialize<SubscriptionPageParameterModel>(l.Parameters).Id)
                    .ToList();

            // Map that list of pageId to list of PageModel
            var pages = pageService.Maps(pageService.Fetch(p => pageIds.Contains(p.Id))).ToList();

            // Select a list of PageSubscriptionEmailModel from SubscriptionEmailModel and PageModel
            var model = emailModels.Select(m => new PageSubscriptionEmailModel
            {
                SubscriptionId = m.SubscriptionId,
                Page =
                    pages.FirstOrDefault(
                        p => p.Id == SerializeUtilities.Deserialize<SubscriptionPageParameterModel>(m.Parameters).Id) ??
                    new PageModel(),
                Logs = m.Logs
            }).ToList();

            // Enable flag for sending email
            if (model.Any(e => e.Logs.Any()))
            {
                isAnythingChanged = true;
            }

            return subscriptionTemplateService.ParseSubscription(template.Module, model).Body;
        }

        #endregion

        #endregion

        #region Widget

        /// <summary>
        /// Generate subscription model
        /// </summary>
        /// <param name="module"></param>
        /// <param name="email"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SubscriptionWidget GenerateSubscription(SubscriptionEnums.SubscriptionModule module, string email,
            dynamic parameters)
        {
            var model = new SubscriptionWidget();
            if (parameters == null)
            {
                return model;
            }

            model.Parameters = SerializeUtilities.Serialize(parameters);
            model.Module = module;
            model.Email = email;

            // Check subscription existed or not
            var currentSubscription = GetSubscription(module, email, parameters);

            if (currentSubscription != null)
            {
                model.Id = currentSubscription.Id;
            }

            return model;
        }

        /// <summary>
        /// Get content update for current user from last login
        /// </summary>
        /// <returns></returns>
        public ContentUpdateWidget GetContentUpdates()
        {
            if (WorkContext.CurrentUser != null && WorkContext.CurrentUser.LastTimeGettingUpdate.HasValue)
            {
                var logs = _subscriptionLogService.GetLogs(null, WorkContext.CurrentUser.LastTimeGettingUpdate.Value,
                    SubscriptionEnums.SubscriptionModule.Page);

                var pageIds =
                    logs.Select(l => l.Parameters)
                        .ToList()
                        .Select(p => SerializeUtilities.Deserialize<SubscriptionPageParameterModel>(p).Id);

                var pages = _pageService.Maps(_pageService.Fetch(p => pageIds.Contains(p.Id))).ToList();

                var pageLogs = logs.GroupBy(l => l.Parameters).ToList().Select(l => new PageSubscriptionLogModel
                {
                    Page =
                        pages.FirstOrDefault(
                            p => p.Id == SerializeUtilities.Deserialize<SubscriptionPageParameterModel>(l.Key).Id) ??
                        new PageModel(),
                    Logs = l.Select(i => new SubscriptionLogItem(i)).ToList()
                }).ToList();


                return new ContentUpdateWidget
                {
                    TotalChanges = pageLogs.Count,
                    TotalLogs = pageLogs.Sum(l => l.Logs.Count),
                    PageLogs = pageLogs
                };
            }

            return new ContentUpdateWidget();
        }

        #endregion
    }
}