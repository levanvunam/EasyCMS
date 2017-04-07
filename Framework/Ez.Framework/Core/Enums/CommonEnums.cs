using System.ComponentModel;

namespace Ez.Framework.Core.Enums
{
    public class CommonEnums
    {
        public enum EditableType
        {
            Text = 1,
            TextArea,
            Date,
            DateTime,
            Number,
            [Description("password")]
            Password,
            [Description("url")]
            Url,
            [Description("tel")]
            Phone,
            [Description("range")]
            Range,
            [Description("time")]
            Time,
            [Description("email")]
            Email,
            Boolean
        }

        public enum DateRange
        {
            Today = 1,
            Yesterday = 2,
            [Description("Current Week")]
            CurrentWeek = 3,
            [Description("Current Month")]
            CurrentMonth = 4,
            [Description("Current Year To Date")]
            CurrentYearToDate = 5,
            [Description("Previous Week")]
            PreviousWeek = 6,
            [Description("Previous Month")]
            PreviousMonth = 7,
            [Description("Previous Year")]
            PreviousYear = 8
        }

        public enum UrlTarget
        {
            [Description("_self")]
            Self,
            [Description("_blank")]
            Blank,
            [Description("_parent")]
            Parent,
            [Description("_top")]
            Top
        }

        public enum UrlType
        {
            Internal,
            External
        }

        public enum DateTimeFormat
        {
            Date = 1,
            Time,
            DateTime
        }

        public enum AustraliaState
        {
            ACT = 1,
            NSW = 2,
            NT = 3,
            QLD = 4,
            SA = 5,
            TAS = 6,
            VIC = 7,
            WA = 8
        }
    }
}
