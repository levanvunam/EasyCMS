using System.ComponentModel;

namespace EzCMS.Entity.Core.Enums
{
    public class ForNavigationms
    {
        public enum FormSetupStep
        {
            [Description("Setup Form")]
            SetupForm = 1,

            [Description("Setup Configuration")]
            SetupConfiguration = 2,

            [Description("Finish")]
            Finish = 3,
        }

        public enum FormType
        {
            [Description("input")]
            Input = 1,

            [Description("checkbox")]
            Checkbox = 2,

            [Description("select")]
            Select = 3,

            [Description("textarea")]
            Textarea = 4,

            [Description("textarea-split")]
            TextareaSplit = 5,

            [Description("hidden")]
            Hidden = 6,
        }
    }
}
