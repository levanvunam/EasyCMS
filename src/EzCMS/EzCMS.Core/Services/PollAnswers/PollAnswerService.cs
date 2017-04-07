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
using EzCMS.Core.Models.PollAnswers;
using EzCMS.Core.Models.PollAnswers.Widgets;
using EzCMS.Core.Models.Polls.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.PollAnswers
{
    public class PollAnswerService : ServiceHelper, IPollAnswerService
    {
        private readonly IRepository<PollAnswer> _pollAnswerRepository;
        private readonly IRepository<Poll> _pollRepository;

        public PollAnswerService(IRepository<PollAnswer> pollAnswerRepository, IRepository<Poll> pollRepository)
        {
            _pollAnswerRepository = pollAnswerRepository;
            _pollRepository = pollRepository;
        }

        #region Vote

        /// <summary>
        /// Increase total votes of an answer
        /// </summary>
        /// <param name="answer"></param>
        public void IncreaseVote(PollAnswer answer)
        {
            answer.Total += 1;
            Update(answer);
        }

        #endregion

        /// <summary>
        /// Get poll answers
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        public IEnumerable<SelectListItem> GetPollAnswers(int pollId)
        {
            return Fetch(m => m.PollId == pollId).Select(pollAnswer => new SelectListItem
            {
                Text = pollAnswer.AnswerText,
                Value = SqlFunctions.StringConvert((double) pollAnswer.Id).Trim()
            });
        }

        #region Base

        public IQueryable<PollAnswer> GetAll()
        {
            return _pollAnswerRepository.GetAll();
        }

        public IQueryable<PollAnswer> Fetch(Expression<Func<PollAnswer, bool>> expression)
        {
            return _pollAnswerRepository.Fetch(expression);
        }

        public PollAnswer FetchFirst(Expression<Func<PollAnswer, bool>> expression)
        {
            return _pollAnswerRepository.FetchFirst(expression);
        }

        public PollAnswer GetById(object id)
        {
            return _pollAnswerRepository.GetById(id);
        }

        internal ResponseModel Insert(PollAnswer pollAnswer)
        {
            return _pollAnswerRepository.Insert(pollAnswer);
        }

        internal ResponseModel Update(PollAnswer pollAnswer)
        {
            return _pollAnswerRepository.Update(pollAnswer);
        }

        internal ResponseModel Delete(PollAnswer pollAnswer)
        {
            return _pollAnswerRepository.Delete(pollAnswer);
        }

        internal ResponseModel Delete(object id)
        {
            return _pollAnswerRepository.Delete(id);
        }

        internal ResponseModel InactiveRecord(int id)
        {
            return _pollAnswerRepository.SetRecordDeleted(id);
        }

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the poll answers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public JqGridSearchOut SearchPollAnswers(JqSearchIn si, PollAnswerSearchModel model)
        {
            var data = SearchPollAnswers(model);

            var pollAnswers = Maps(data);

            return si.Search(pollAnswers);
        }

        /// <summary>
        /// Export poll answers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, PollAnswerSearchModel model)
        {
            var data = gridExportMode == GridExportMode.All ? GetAll() : SearchPollAnswers(model);

            var pollAnswers = Maps(data);

            var exportData = si.Export(pollAnswers, gridExportMode);

            return ExcelUtilities.CreateWorkBook(exportData);
        }

        #region Private Methods

        /// <summary>
        /// Search poll answers
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private IQueryable<PollAnswer> SearchPollAnswers(PollAnswerSearchModel model)
        {
            return Fetch(pollAnswer => (string.IsNullOrEmpty(model.Keyword)
                                        ||
                                        (!string.IsNullOrEmpty(pollAnswer.AnswerText) &&
                                         pollAnswer.AnswerText.Contains(model.Keyword)))
                                       && (!model.PollId.HasValue || pollAnswer.PollId == model.PollId));
        }

        /// <summary>
        /// Map entities to models
        /// </summary>
        /// <param name="pollAnswers"></param>
        /// <returns></returns>
        private IQueryable<PollAnswerModel> Maps(IQueryable<PollAnswer> pollAnswers)
        {
            return pollAnswers.Select(m => new PollAnswerModel
            {
                Id = m.Id,
                AnswerText = m.AnswerText,
                Total = m.Total,
                PollId = m.PollId,
                PollQuestion = m.Poll.PollQuestion,
                RecordOrder = m.RecordOrder,
                Created = m.Created,
                CreatedBy = m.CreatedBy,
                LastUpdate = m.LastUpdate,
                LastUpdateBy = m.LastUpdateBy
            });
        }

        #endregion

        #endregion

        #region Manage

        /// <summary>
        /// Get poll answer manage model for creating / editing
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pollId"></param>
        /// <returns></returns>
        public PollAnswerManageModel GetPollAnswerManageModel(int? id = null, int? pollId = null)
        {
            var pollAnswer = GetById(id);
            if (pollAnswer != null)
            {
                return new PollAnswerManageModel(pollAnswer);
            }
            return new PollAnswerManageModel(pollId);
        }

        /// <summary>
        /// Save poll answer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel SavePollAnswer(PollAnswerManageModel model)
        {
            ResponseModel response;
            var pollAnswer = GetById(model.Id);
            if (pollAnswer != null)
            {
                pollAnswer = GetById(model.Id);
                pollAnswer.AnswerText = model.AnswerText;
                pollAnswer.Total = model.Total;
                pollAnswer.PollId = model.PollId;
                pollAnswer.RecordOrder = model.RecordOrder;
                response = Update(pollAnswer);
                return response.SetMessage(response.Success
                    ? T("PollAnswer_Message_UpdateSuccessfully")
                    : T("PollAnswer_Message_UpdateFailure"));
            }
            Mapper.CreateMap<PollAnswerManageModel, PollAnswer>();
            pollAnswer = Mapper.Map<PollAnswerManageModel, PollAnswer>(model);
            response = Insert(pollAnswer);
            return response.SetMessage(response.Success
                ? T("PollAnswer_Message_CreateSuccessfully")
                : T("PollAnswer_Message_CreateFailure"));
        }

        /// <summary>
        /// Delete poll answer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ResponseModel DeletePollAnswer(int id)
        {
            var pollAnswer = GetById(id);
            if (pollAnswer != null)
            {
                var response = Delete(pollAnswer);
                return response.SetMessage(response.Success
                    ? T("PollAnswer_Message_DeleteSuccessfully")
                    : T("PollAnswer_Message_DeleteFailure"));
            }
            return new ResponseModel
            {
                Success = true,
                Message = T("PollAnswer_Message_DeleteSuccessfully")
            };
        }

        #endregion

        #region Details

        /// <summary>
        /// Get poll answer details
        /// </summary>
        /// <param name="pollAnswerId"></param>
        /// <returns></returns>
        public PollAnswerDetailModel GetPollAnswerDetailModel(int pollAnswerId)
        {
            var pollAnswer = GetById(pollAnswerId);
            return pollAnswer != null ? new PollAnswerDetailModel(pollAnswer) : null;
        }

        /// <summary>
        /// Update value for property of model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ResponseModel UpdatePollAnswerData(XEditableModel model)
        {
            var pollAnswer = GetById(model.Pk);
            if (pollAnswer != null)
            {
                var property =
                    ReflectionUtilities.GetAllPropertiesOfType(typeof (PollAnswerManageModel))
                        .FirstOrDefault(p => p.Name.Equals(model.Name, StringComparison.CurrentCultureIgnoreCase));
                if (property != null)
                {
                    object value = model.Value.ToType(property, WorkContext.CurrentTimezone);

                    #region Validate

                    var manageModel = new PollAnswerManageModel(pollAnswer);
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

                    pollAnswer.SetProperty(model.Name, value);

                    var response = Update(pollAnswer);
                    return response.SetMessage(response.Success
                        ? T("PollAnswer_Message_UpdatePollAnswerInfoSuccessfully")
                        : T("PollAnswer_Message_UpdatePollAnswerInfoFailure"));
                }
                return new ResponseModel
                {
                    Success = false,
                    Message = T("PollAnswer_Message_PropertyNotFound")
                };
            }
            return new ResponseModel
            {
                Success = false,
                Message = T("PollAnswer_Message_ObjectNotFound")
            };
        }

        #endregion

        #region Widget

        /// <summary>
        /// Get poll results widget
        /// </summary>
        /// <param name="pollAnswerIdsString"></param>
        /// <returns></returns>
        public PollResultWidget GetPollResultsWidget(string pollAnswerIdsString)
        {
            if (string.IsNullOrEmpty(pollAnswerIdsString))
                return null;
            var pollAnswerIds = pollAnswerIdsString.Split(',').Select(int.Parse).ToList();

            var polls = _pollRepository.Fetch(p => p.PollAnswers.Any(pa => pollAnswerIds.Contains(pa.Id)));
            var isPollAnswersFromSamePoll = polls.Any() && polls.Count() == 1;

            //Check if poll answers are comming from same poll or not
            if (!isPollAnswersFromSamePoll)
            {
                return null;
            }

            var poll = polls.First();

            var pollResultWidget = new PollResultWidget
            {
                PollQuestion = poll.PollQuestion,
                PollResultId = pollAnswerIdsString.ToIdStringByHash(),
                TotalVotes = poll.PollAnswers.Sum(p => p.Total)
            };

            foreach (var id in pollAnswerIds)
            {
                var pollAnswer = GetById(id);
                if (pollAnswer == null)
                {
                    return null;
                }
                var pollAnswerWidget = new PollAnswerWidget(pollAnswer);
                pollResultWidget.PollAnswers.Add(pollAnswerWidget);
            }
            return pollResultWidget;
        }

        /// <summary>
        /// Get poll answers by poll Id
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        public List<PollAnswerWidget> GetPollAnswerWidgets(int pollId)
        {
            var pollAnswers = GetAll().Where(m => m.PollId == pollId).ToList();
            return pollAnswers.Select(pollAnswer => new PollAnswerWidget(pollAnswer)).ToList();
        }

        #endregion
    }
}