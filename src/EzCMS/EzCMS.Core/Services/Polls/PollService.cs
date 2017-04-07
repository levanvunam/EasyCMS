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
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Excel;
using Ez.Framework.Utilities.Reflection;
using EzCMS.Core.Framework.Context;
using EzCMS.Core.Models.Polls;
using EzCMS.Core.Models.Polls.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Polls
{
    public class PollService : ServiceHelper, IPollService
    {
        private readonly IRepository<Poll> _pollRepository;

        public PollService(IRepository<Poll> pollRepository)
        {
            _pollRepository = pollRepository;
        }

        #region Widgets

        /// <summary>
        /// Get poll widget
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        public PollWidget GetPollsWidget(int pollId)
        {
            var poll = GetById(pollId);
            if (poll != null)
                return new PollWidget(poll);
            return null;
        }

        #endregion

        #region Base

        public IQueryable<Poll> GetAll()
        {
            return _pollRepository.GetAll();
        }

        public IQueryable<Poll> Fetch(Expression<Func<Poll, bool>> expression)
        {
            return _pollRepository.Fetch(expression);
        }

        public Poll FetchFirst(Expression<Func<Poll, bool>> expression)
        {
            return _pollRepository.FetchFirst(expression);
        }

        public Poll GetById(object id)
        {
            return _pollRepository.GetById(id);
        }

        internal ResponseModel Insert(Poll poll)
        {
            return _pollRepository.Insert(poll);
        }

        internal ResponseModel Update(Poll poll)
        {
            return _pollRepository.Update(poll);
        }

        internal ResponseModel Delete(Poll poll)
        {
            return _pollRepository.Delete(poll);
        }

        internal ResponseModel Delete(object id)
        {
            return _pollRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _pollRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the polls.
        /// </summary>
        /// <returns></returns>
        public JqGridSearchOut SearchPolls(JqSearchIn si)
        {
            var data = GetAll();

            var polls = Maps(data);

            return si.Search(polls);
        }

        /// <summary>
        /// Export polls
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode)
        {
            var data = GetAll();

            var polls = Maps(data);

            var exportData = si.Export(polls, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="polls"></param>
        /// <returns></returns>
        private IQueryable<PollModel> Maps(IQueryable<Poll> polls)
        {
            return polls.Select(m => new PollModel
            {
                Id = m.Id,
                PollQuestion = m.PollQuestion,
                PollSummary = m.PollSummary,
                IsMultiple = m.IsMultiple,
                ThankyouText = m.ThankyouText,
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
        /// Get list of polls
        /// </summary>
        /// <param name="pollIds"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetPolls(List<int> pollIds = null)
        {
            if (pollIds == null) pollIds = new List<int>();
            return GetAll().Select(g => new SelectListItem
            {
                Text = g.PollQuestion,
                Value = SqlFunctions.StringConvert((double) g.Id).Trim(),
                Selected = pollIds.Any(id => id == g.Id)
            });
        }

        /// <summary>
        /// Get poll of poll answers
        /// </summary>
        /// <param name="pollAnswerIdsString"></param>
        /// <returns></returns>
        public Poll GetPoll(string pollAnswerIdsString)
        {
            if (string.IsNullOrEmpty(pollAnswerIdsString))
            {
                return null;
            }
            var pollAnswerIds = pollAnswerIdsString.Split(',').Select(int.Parse).ToList();
            if (pollAnswerIds.Count == 0)
            {
                return null;
            }
            var polls = GetAll();
            foreach (var pollAnswerId in pollAnswerIds)
            {
                var id = pollAnswerId;
                polls = polls.Where(p => p.PollAnswers.Select(a => a.Id).Contains(id));
            }
            return polls.FirstOrDefault();
        }

        /// <summary>
        /// Get poll manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PollManageModel GetPollManageModel(int? id = null)
        {
            var poll = GetById(id);
            return poll != null ? new PollManageModel(poll) : new PollManageModel();
        }

        /// <summary>
        /// Save poll
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SavePoll(PollManageModel model)
        {
            ResponseModel response;
            var poll = GetById(model.Id);
            if (poll != null)
            {
                poll.PollQuestion = model.PollQuestion;
                poll.PollSummary = model.PollSummary;
                poll.IsMultiple = model.IsMultiple;
                poll.ThankyouText = model.ThankyouText;
                response = Update(poll);
                return response.SetMessage(response.Success
                    ? T("Poll_Message_UpdateSuccessfully")
                    : T("Poll_Message_UpdateFailure"));
            }
            Mapper.CreateMap<PollManageModel, Poll>();
            poll = Mapper.Map<PollManageModel, Poll>(model);
            response = Insert(poll);
            return response.SetMessage(response.Success
                ? T("Poll_Message_CreateSuccessfully")
                : T("Poll_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete poll
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeletePoll(int id)
        {
            var poll = GetById(id);
            if (poll != null)
            {
                if (poll.PollAnswers.Any())
                {
                    return new ResponseModel
                    {
                        Success = false,
                        Message = T("Poll_Message_DeleteFailureBasedOnRelatedPollAnswers")
                    };
                }
                var response = Delete(poll);
                return
                    response.SetMessage(response.Success
                        ? T("Poll_Message_DeleteSuccessfully")
                        : T("Poll_Message_DeleteFailure"));
            }

            return new ResponseModel
            {
                Success = true,
                Message = T("Poll_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get poll details
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        public PollDetailModel GetPollDetailModel(int pollId)
        {
            var poll = GetById(pollId);
            return poll != null ? new PollDetailModel(poll) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdatePollData(XEditableModel model)
        {
            var poll = GetById(model.Pk);
            if (poll != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (PollManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new PollManageModel(poll);
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

                    poll.SetProperty(model.Name, value);

                    var response = Update(poll);
                    return response.SetMessage(response.Success
                        ? T("Poll_Message_UpdatePollInfoSuccessfully")
                        : T("Poll_Message_UpdatePollInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("Poll_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("Poll_Message_ObjectNotFound")
            };
        }

        #endregion
    }
}