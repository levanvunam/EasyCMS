using RazorEngine.Text;
using System.Web.Mvc;

namespace EzCMS.Core.Framework.Mvc.ViewEngines.Razor.RazorEngine.Configs
{
    public class RazorEngineMvcHtmlStringFactory : IEncodedStringFactory
    {
        public IEncodedString CreateEncodedString(string rawString)
        {
            return new HtmlEncodedString(rawString);
        }

        public IEncodedString CreateEncodedString(object obj)
        {
            if (obj == null)
                return new HtmlEncodedString(string.Empty);

            var htmlString = obj as HtmlEncodedString;
            if (htmlString != null)
                return htmlString;

            var mvcHtmlString = obj as MvcHtmlString;
            if (mvcHtmlString != null)
                return new MvcHtmlStringWrapper(mvcHtmlString);

            return new HtmlEncodedString(obj.ToString());
        }

        public class MvcHtmlStringWrapper : IEncodedString
        {
            private readonly MvcHtmlString _value;

            public MvcHtmlStringWrapper(MvcHtmlString value)
            {
                _value = value;
            }

            public string ToEncodedString()
            {
                return _value.ToString();
            }

            public override string ToString()
            {
                return ToEncodedString();
            }
        }
    }
}
