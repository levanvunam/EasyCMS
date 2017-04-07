using Ez.Framework.Core.Attributes;
using Ez.Framework.Utilities;
using Ez.Framework.Utilities.Reflection;
using Ez.Framework.IoC;
using EzCMS.Core.Framework.Attributes;
using EzCMS.Core.Framework.Enums;
using EzCMS.Core.Services.Localizes;
using EzCMS.Core.Services.Toolbars;
using EzCMS.Entity.Entities.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Ez.Framework.Core.IoC;
using Ez.Framework.Core.Locale.Attributes;

namespace EzCMS.Core.Models.Toolbars
{
    public class ToolbarManageModel : IValidatableObject
    {
        #region Constructors

        public ToolbarManageModel()
        {
            Groups = EnumUtilities.GetAllItems<EditorToolGroup>().Select(g => new ToolbarGroupItem
            {
                Group = g,
                Tools = new List<EditorTool>()
            }).ToList();
            foreach (var group in Groups)
            {
                group.Tools = EnumUtilities.GetAllItems<EditorTool>().Where(t => typeof(EditorTool).GetField(t.ToString()).GetAttribute<EditorToolSettingAttribute>() != null && typeof(EditorTool).GetField(t.ToString()).GetAttribute<EditorToolSettingAttribute>().Group == group.Group).ToList();
            }

            Tools = EnumUtilities.GetAllItems<EditorTool>();
        }

        public ToolbarManageModel(Toolbar toolbar)
            : this()
        {
            Id = toolbar.Id;
            Name = toolbar.Name;
            BasicToolbar = toolbar.BasicToolbar;
            PageToolbar = toolbar.PageToolbar;
        }

        #endregion

        #region Public Properties
        public int? Id { get; set; }

        [Required]
        [LocalizedDisplayName("Toolbar_Field_Name")]
        public string Name { get; set; }

        [LocalizedDisplayName("Toolbar_Field_BasicTools")]
        public List<string> BasicTools { get; set; }

        public string BasicToolbar
        {
            get
            {
                if (BasicTools == null) BasicTools = new List<string>();
                return SerializeUtilities.Serialize(BasicTools);
            }
            set
            {
                BasicTools = SerializeUtilities.Deserialize<List<string>>(value) ?? new List<string>();
            }
        }

        [LocalizedDisplayName("Toolbar_Field_PageTools")]
        public List<string> PageTools { get; set; }

        public string PageToolbar
        {
            get
            {
                if (PageTools == null) PageTools = new List<string>();
                return SerializeUtilities.Serialize(PageTools);
            }
            set
            {
                PageTools = SerializeUtilities.Deserialize<List<string>>(value) ?? new List<string>();
            }
        }

        public IEnumerable<EditorTool> Tools { get; set; }

        public IEnumerable<ToolbarGroupItem> Groups { get; set; }

        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var localizedResourceService = HostContainer.GetInstance<IEzCMSLocalizedResourceService>();
            var toolbarService = HostContainer.GetInstance<IToolbarService>();

            if (toolbarService.IsToolbarExisted(Id, Name))
            {
                yield return new ValidationResult(localizedResourceService.T("Toolbar_Message_ExistingName"), new[] { "Name" });
            }
        }
    }

    public class ToolbarGroupItem
    {
        #region Public Properties

        public EditorToolGroup Group { get; set; }

        public List<EditorTool> Tools { get; set; }

        #endregion
    }
}
