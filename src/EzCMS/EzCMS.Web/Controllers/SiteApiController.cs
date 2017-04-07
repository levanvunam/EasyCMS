using Ez.Framework.Core.Exceptions.Common;
using Ez.Framework.Core.Mvc.Helpers;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Files;
using EzCMS.Core.Framework.Configuration;
using EzCMS.Core.Framework.Exceptions.Sites;
using EzCMS.Core.Framework.Mvc.Controllers;
using EzCMS.Core.Models.Polls;
using EzCMS.Core.Models.Subscriptions;
using EzCMS.Core.Services.Contacts;
using EzCMS.Core.Services.Forms;
using EzCMS.Core.Services.LinkTrackers;
using EzCMS.Core.Services.Locations;
using EzCMS.Core.Services.News;
using EzCMS.Core.Services.PollAnswers;
using EzCMS.Core.Services.Polls;
using EzCMS.Core.Services.ProtectedDocumentLogs;
using EzCMS.Core.Services.ProtectedDocuments;
using EzCMS.Core.Services.Subscriptions;
using EzCMS.Entity.Core.Enums;
using EzCMS.Entity.Entities.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;

namespace EzCMS.Web.Controllers
{
    public class SiteApiController : ClientController
    {
        private readonly INewsService _newsService;
        private readonly ILocationService _locationService;
        private readonly IPollAnswerService _pollAnswerService;
        private readonly IPollService _pollService;
        private readonly ILinkTrackerService _linkTrackerService;
        private readonly IFormService _formService;
        private readonly IContactService _contactService;
        private readonly IDocumentService _documentService;
        private readonly IProtectedDocumentLogService _protectedDocumentLogService;
        private readonly ISubscriptionService _subscriptionService;

        public SiteApiController(INewsService newsService, ILocationService locationService,
            IPollAnswerService pollAnswerService, IPollService pollService, ILinkTrackerService linkTrackerService,
            IFormService formService, IContactService contactService, IDocumentService documentService,
            IProtectedDocumentLogService protectedDocumentLogService, ISubscriptionService subscriptionService)
        {
            _newsService = newsService;
            _locationService = locationService;
            _pollAnswerService = pollAnswerService;
            _pollService = pollService;
            _linkTrackerService = linkTrackerService;
            _formService = formService;
            _contactService = contactService;
            _documentService = documentService;
            _protectedDocumentLogService = protectedDocumentLogService;
            _subscriptionService = subscriptionService;
        }

        #region Contact

        #region Methods

        [HttpPost]
        public ActionResult SaveContact(Contact contact, ContactCommunication communication)
        {
            var response = _contactService.SaveContactForm(contact, communication, HttpContext.Request.Form);
            return Json(response);
        }

        #endregion

        #endregion

        #region Form

        public ActionResult ScriptLoader(string f)
        {
            var mime = FileUtilities.GetMimeMapping(".js");

            var content = _formService.LoadLoaderScript(f);
            return Content(content, mime);
        }

        public ActionResult IframeLoader(string f, bool formSubmitted = false)
        {
            var model = _formService.LoadForm(f, formSubmitted);

            return View("Forms/IframeLoader", model);
        }

        #region Methods

        [HttpPost]
        public ActionResult SubmitForm(string f, string referredUrl, bool isAjaxRequest = false)
        {
            var contact = new Contact();
            var communication = new ContactCommunication();
            TryUpdateModel(contact);
            TryUpdateModel(communication);

            var response = _formService.SubmitForm(f, contact, communication, HttpContext.Request.Form);

            if (isAjaxRequest)
            {
                //Allow cross domain submit
                HttpContext.Response.AppendHeader("Access-Control-Allow-Origin", "*");
                return Json(response);
            }

            if (string.IsNullOrEmpty(referredUrl))
            {
                return RedirectToAction("IframeLoader", new { f, formSubmitted = true });
            }

            if (response.Success)
            {
                referredUrl = referredUrl.AddQueryParam("formSubmitted", "true");
            }
            return Redirect(referredUrl);
        }

        #endregion

        #endregion

        #region News

        #region Methods

        [HttpPost]
        public JsonResult SetupNewsTracking(int id)
        {
            return Json(_newsService.SetupNewsTracking(id));
        }

        #endregion

        #endregion

        #region Link Tracker

        public ActionResult LinkTracker(int id)
        {
            var response = _linkTrackerService.TriggerLinkTrackerAction(id);
            if (response.Success)
            {
                return Redirect((string)response.Data);
            }

            return View("LinkTrackers/Index", response);
        }

        #endregion

        #region Locations

        #region Methods

        [HttpPost]
        public JsonResult GetLocationsByTypes(List<int> types)
        {
            return Json(_locationService.GetLocationsByTypes(types));
        }

        #endregion

        #endregion

        #region Poll

        public ActionResult PollShowResult(int pollId)
        {
            var poll = _pollService.GetPollsWidget(pollId);
            var pollChart = new PollChartModel(poll);
            return View("Polls/ShowResult", pollChart);
        }

        #region Methods

        public int PollVote(List<int> voteIds, int pollId)
        {
            if (voteIds != null)
            {
                foreach (var voteId in voteIds)
                {
                    var answer = _pollAnswerService.GetById(voteId);
                    if (answer != null)
                    {
                        _pollAnswerService.IncreaseVote(answer);
                    }
                }

                var cookieName = string.Format(EzCMSContants.VotedUserCookie, pollId);

                StateManager.GetCookie(cookieName, true, true,
                    DateTime.Now.AddDays(EzCMSContants.PollCookieExpireDay));

                return voteIds.Count;
            }
            return 0;
        }

        #endregion

        #endregion

        #region Protected Document

        /// <summary>
        /// Render dynamic style sheet file
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult ProtectedDocument(string id)
        {
            try
            {
                var realPath = id.GetUniqueLinkInput();

                // Check if user can access path
                if (_documentService.CanCurrentUserAccessPath(realPath))
                {
                    // Save log
                    _protectedDocumentLogService.SaveProtectedDocumentLog(realPath);

                    // Return file
                    var extension = Path.GetExtension(realPath);
                    if (extension != null)
                    {
                        var mime = System.Web.MimeMapping.GetMimeMapping(extension);
                        return new FilePathResult(realPath, mime);
                    }
                }
            }
            catch (InvalidUniqueLinkException)
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var fileMime = System.Web.MimeMapping.GetMimeMapping(Path.GetExtension(id));
                    return new FilePathResult(string.Format("/Documents/{0}", id), fileMime);
                }
            }

            throw new EzCMSNotFoundException();
        }

        #endregion

        #region Subscription

        public ActionResult Subscription(SubscriptionEnums.SubscriptionAction subscriptionAction, int subscriptionId)
        {
            var response = _subscriptionService.TriggerSubscriptionAction(subscriptionAction, subscriptionId);

            if (response.Success)
            {
                switch (subscriptionAction)
                {
                    case SubscriptionEnums.SubscriptionAction.View:
                        return Redirect((string)response.Data);
                }
            }

            return View("Subscription/Index", response);
        }

        #region Methods

        [HttpPost]
        public ActionResult SubscriptionRegister(SubscriptionManageModel model)
        {
            return Json(_subscriptionService.Register(model));
        }

        [HttpPost]
        public ActionResult SubscriptionRemoveRegistration(SubscriptionManageModel model)
        {
            return Json(_subscriptionService.RemoveRegistration(model));
        }

        #endregion

        #endregion
    }
}
