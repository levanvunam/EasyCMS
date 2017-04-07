using System;
using System.Linq;
using System.Linq.Expressions;
using AutoMapper;
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
using EzCMS.Core.Models.Links;
using EzCMS.Core.Models.Links.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Links
{
    public class LinkService : ServiceHelper, ILinkService
    {
        private readonly IRepository<Link> _linkRepository;

        public LinkService(IRepository<Link> linkRepository)
        {
            _linkRepository = linkRepository;
        }

        #region Widgets

        /// <summary>
        /// Get links widget
        /// </summary>
        /// <param name="linkTypeId"></param>
        /// <param name="number"></param>
        /// <returns></returns>
        public LinksWidget GetLinksWidget(int linkTypeId, int number)
        {
            var now = DateTime.UtcNow;

            var data = Fetch(m => (linkTypeId == 0 || m.LinkTypeId == linkTypeId)
                                  && (!m.DateEnd.HasValue || m.DateEnd >= now)
                                  && (!m.DateStart.HasValue || m.DateStart <= now));

            if (number > 0)
            {
                data = data.OrderByDescending(m => m.DateStart).Take(number);
            }
            else
            {
                data = data.OrderByDescending(m => m.DateStart);
            }

            return new LinksWidget
            {
                Links = data.Select(link => new LinkWidget
                {
                    Name = link.Name,
                    Url = link.Url,
                    UrlTarget = link.UrlTarget,
                    Description = link.Description,
                    LinkTypeId = link.LinkTypeId,
                    LinkType = link.LinkType.Name,
                    DateStart = link.DateStart,
                    DateEnd = link.DateEnd
                }).ToList()
            };
        }

        #endregion

        /// <summary>
        /// Get Link detail model
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public LinkDetailModel GetLinkDetailModel(int id)
        {
            var item = GetById(id);
            return item != null ? new LinkDetailModel(item) : null;
        }

        #region Base

        public IQueryable<Link> GetAll()
        {
            return _linkRepository.GetAll();
        }

        public IQueryable<Link> Fetch(Expression<Func<Link, bool>> expression)
        {
            return _linkRepository.Fetch(expression);
        }

        public Link FetchFirst(Expression<Func<Link, bool>> expression)
        {
            return _linkRepository.FetchFirst(expression);
        }

        public Link GetById(object id)
        {
            return _linkRepository.GetById(id);
        }

        internal ResponseModel Insert(Link link)
        {
            return _linkRepository.Insert(link);
        }

        internal ResponseModel Update(Link link)
        {
            return _linkRepository.Update(link);
        }

        internal ResponseModel Delete(Link link)
        {
            return _linkRepository.Delete(link);
        }

        internal ResponseModel Delete(object id)
        {
            return _linkRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _linkRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the links
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchLinks(JqSearchIn si, LinkSearchModel model)
        {
            var data = SearchLinkTypes(model);

            var links = Maps(data);

            return si.Search(links);
        }

        /// <summary>
        /// Export links
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, LinkSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchLinkTypes(model);

            var links = Maps(data);

            var exportData = si.Export(links, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search links
        /// </summary>
        /// <returns></returns>
        private IQueryable<Link> SearchLinkTypes(LinkSearchModel model)
        {
            return Fetch(link => (string.IsNullOrEmpty(model.Keyword)
                                  || (!string.IsNullOrEmpty(link.Name) && link.Name.Contains(model.Keyword))
                                  ||
                                  (!string.IsNullOrEmpty(link.Description) && link.Description.Contains(model.Keyword))
                                  || (!string.IsNullOrEmpty(link.Url) && link.Url.Contains(model.Keyword)))
                                 && (!model.LinkTypeId.HasValue || link.LinkTypeId == model.LinkTypeId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="links"></param>
        /// <returns></returns>
        private IQueryable<LinkModel> Maps(IQueryable<Link> links)
        {
            return links.Select(link => new LinkModel
            {
                Id = link.Id,
                Name = link.Name,
                Url = link.Url,
                UrlTarget = link.UrlTarget,
                Description = link.Description,
                LinkTypeId = link.LinkTypeId,
                LinkTypeName = link.LinkType.Name,
                DateStart = link.DateStart,
                DateEnd = link.DateEnd,
                RecordOrder = link.RecordOrder,
                Created = link.Created,
                CreatedBy = link.CreatedBy,
                LastUpdate = link.LastUpdate,
                LastUpdateBy = link.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get link manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="linkTypeId"></param>
        /// <returns></returns>
        public LinkManageModel GetLinkManageModel(int? id = null, int? linkTypeId = null)
        {
            var link = GetById(id);
            if (link != null)
            {
                return new LinkManageModel(link);
            }

            return new LinkManageModel(linkTypeId);
        }

        /// <summary>
        /// Save link
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SaveLink(LinkManageModel model)
        {
            ResponseModel response;
            var link = GetById(model.Id);
            if (link != null)
            {
                link.DateStart = model.DateStart;
                link.DateEnd = model.DateEnd;
                link.Name = model.Name;
                link.Description = model.Description;
                link.Url = model.Url;
                link.UrlTarget = model.UrlTarget;
                link.LinkTypeId = model.LinkTypeId;
                response = Update(link);
                return response.SetMessage(response.Success
                    ? T("Link_Message_UpdateSuccessfully")
                    : T("Link_Message_UpdateFailure"));
            }
            Mapper.CreateMap<LinkManageModel, Link>();
            link = Mapper.Map<LinkManageModel, Link>(model);
            response = Insert(link);
            return response.SetMessage(response.Success
                ? T("Link_Message_CreateSuccessfully")
                : T("Link_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete link
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeleteLink(int id)
        {
            var link = GetById(id);
            if (link != null)
            {
                var response = Delete(link);
                return response.SetMessage(response.Success
                    ? T("Link_Message_DeleteSuccessfully")
                    : T("Link_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Link_Message_DeleteSuccessfully")
            };
        }

        /// <summary>
        /// Update Link data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdateLinkData(XEditableModel model)
        {
            var link = GetById(model.Pk);
            if (link != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (LinkManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new LinkManageModel(link);
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

                    link.SetProperty(model.Name, value);

                    var response = Update(link);
                    return response.SetMessage(response.Success
                        ? T("Link_Message_UpdateLinkInfoSuccessfully")
                        : T("Link_Message_UpdateLinkInfoFailure"));
                }

                return new ResponseModel
                {
                    Success = false,
                    Message = T("Link_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Link_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}