using EzCMS.Core.Framework.Enums;
using System;

namespace EzCMS.Core.Framework.Attributes
{
    public class EditorToolSettingAttribute : Attribute
    {
        public EditorToolGroup Group { get; set; }

        public EditorToolSettingAttribute()
        {

        }

        public EditorToolSettingAttribute(EditorToolGroup group)
            : this()
        {
            Group = group;
        }
    }
}
