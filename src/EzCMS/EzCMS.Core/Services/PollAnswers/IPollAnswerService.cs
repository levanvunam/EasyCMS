using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.PollAnswers;
using EzCMS.Core.Models.PollAnswers.Widgets;
using EzCMS.Core.Models.Polls.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.PollAnswers
{
    [Register(Lifetime.PerInstance)]
    public interface IPollAnswerService : IBaseService<PollAnswer>
    {
        /// <summary>
        /// Get poll answers
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        IEnumerable<SelectListItem> GetPollAnswers(int pollId);

        #region Vote

        /// <summary>
        /// Vote answer
        /// </summary>
        /// <param name="answer"></param>
        void IncreaseVote(PollAnswer answer);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search poll answers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        JqGridSearchOut SearchPollAnswers(JqSearchIn si, PollAnswerSearchModel model);

        /// <summary>
        /// Export poll answers
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode, PollAnswerSearchModel model);

        #endregion

        #region Manage

        /// <summary>
        /// Get poll answer manage model
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pollId"></param>
        /// <returns></returns>
        PollAnswerManageModel GetPollAnswerManageModel(int? id = null, int? pollId = null);

        /// <summary>
        /// Save poll answer
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel SavePollAnswer(PollAnswerManageModel model);

        /// <summary>
        /// Delete poll answer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        ResponseModel DeletePollAnswer(int id);

        #endregion

        #region Details

        /// <summary>
        /// Get poll answer detail model
        /// </summary>
        /// <param name="pollAnswerId"></param>
        /// <returns></returns>
        PollAnswerDetailModel GetPollAnswerDetailModel(int pollAnswerId);

        /// <summary>
        /// Update poll answer data
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        ResponseModel UpdatePollAnswerData(XEditableModel model);

        #endregion

        #region Widget

        /// <summary>
        /// Get poll result widget
        /// </summary>
        /// <param name="pollAnswerIdsString"></param>
        /// <returns></returns>
        PollResultWidget GetPollResultsWidget(string pollAnswerIdsString);

        /// <summary>
        /// Get list of poll answers
        /// </summary>
        /// <param name="pollId"></param>
        /// <returns></returns>
        List<PollAnswerWidget> GetPollAnswerWidgets(int pollId);

        #endregion
    }
}