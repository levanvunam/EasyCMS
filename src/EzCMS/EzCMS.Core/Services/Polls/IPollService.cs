using System.Collections.Generic;
using System.Web.Mvc;
using Ez.Framework.Core.Entity.RepositoryBase.Models;
using Ez.Framework.Core.IoC.Attributes;
using Ez.Framework.Core.JqGrid;
using Ez.Framework.Core.JqGrid.Enums;
using Ez.Framework.Core.Mvc.Models.Editable;
using Ez.Framework.Core.Services;
using EzCMS.Core.Models.Polls;
using EzCMS.Core.Models.Polls.Widgets;
using EzCMS.Entity.Entities.Models;
using NPOI.HSSF.UserModel;

namespace EzCMS.Core.Services.Polls
{
    [Register(Lifetime.PerInstance)]
    public interface IPollService : IBaseService<Poll>
    {
        #region Widgets

        PollWidget GetPollsWidget(int pollId);

        #endregion

        #region Grid Search

        /// <summary>
        /// Search the poll
        /// </summary>
        /// <param name="si"></param>
        /// <returns></returns>
        JqGridSearchOut SearchPolls(JqSearchIn si);

        /// <summary>
        /// Export the polls
        /// </summary>
        /// <param name="si"></param>
        /// <param name="gridExportMode"></param>
        /// <returns></returns>
        HSSFWorkbook Exports(JqSearchIn si, GridExportMode gridExportMode);

        #endregion

        #region Manage

        IEnumerable<SelectListItem> GetPolls(List<int> pollIds = null);

        Poll GetPoll(string pollAnswerIdsString);

        PollManageModel GetPollManageModel(int? id = null);

        ResponseModel SavePoll(PollManageModel model);

        ResponseModel DeletePoll(int id);

        #endregion

        #region Details

        PollDetailModel GetPollDetailModel(int pollId);

        ResponseModel UpdatePollData(XEditableModel model);

        #endregion
    }
}