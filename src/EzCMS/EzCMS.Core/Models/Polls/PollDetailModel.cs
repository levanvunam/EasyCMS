using System;
using Ez.Framework.Core.Attributes;
using Ez.Framework.Core.Locale.Attributes;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Polls
{
    public class PollDetailModel
    {
        #region Constructors

        public PollDetailModel()
        {

        }

        public PollDetailModel(Poll poll)
            : this()
        {
            Id = poll.Id;

            Poll = new PollManageModel(poll);

            Created = poll.Created;
            CreatedBy = poll.CreatedBy;
            LastUpdate = poll.LastUpdate;
            LastUpdateBy = poll.LastUpdateBy;
        }

        #endregion

        #region Public Properties

        public int Id { get; set; }

        public PollManageModel Poll { get; set; }

        [LocalizedDisplayName("Poll_Field_Created")]
        public DateTime Created { get; set; }

        [LocalizedDisplayName("Poll_Field_CreatedBy")]
        public string CreatedBy { get; set; }

        [LocalizedDisplayName("Poll_Field_LastUpdate")]
        public DateTime? LastUpdate { get; set; }

        [LocalizedDisplayName("Poll_Field_LastUpdateBy")]
        public string LastUpdateBy { get; set; }

        #endregion
    }
}
