using System.Collections.Generic;
using System.Linq;
using EzCMS.Entity.Entities.Models;

namespace EzCMS.Core.Models.Pages.Widgets.Member
{
    public class MembersWidget
    {
        public MembersWidget()
        {

        }

        public MembersWidget(Page page)
            : this()
        {
            var children = page.ChildrenPages;
            if (page.ParentId.HasValue && !children.Any())
            {
                CurrentMember = new MemberItem(page.Parent);
                Members = page.Parent.ChildrenPages.ToList().Select(p => new MemberItem(p)).ToList();
            }
            else
            {
                CurrentMember = new MemberItem(page);
                Members = children == null
                    ? new List<MemberItem>()
                    : children.ToList().Select(p => new MemberItem(p)).ToList();
            }
        }

        #region Public Properties

        public List<MemberItem> Members { get; set; }

        public MemberItem CurrentMember { get; set; }

        #endregion
    }
}
