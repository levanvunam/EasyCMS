using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace EzCMS.Core.Models.Forms.FormConfigurations
{
    public class FormTabModel
    {
        public string title { get; set; }

        public IEnumerable<FormData> data { get; set; } 
    }

    public class FormData
    {
        public string title { get; set; }

        public string template { get; set; }

        public Dictionary<string, object> fields { get; set; }
    }
}
