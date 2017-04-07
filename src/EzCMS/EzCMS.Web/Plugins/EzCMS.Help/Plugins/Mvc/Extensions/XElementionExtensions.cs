using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace EzCMS.Help.Plugins.Mvc.Extensions
{
    public static class XElementionExtensions
    {
        public static string ToStringNewLineOnAttributes(this XElement element)
        {
            var settings = new XmlWriterSettings {Indent = true, OmitXmlDeclaration = true, NewLineOnAttributes = true};
            var stringBuilder = new StringBuilder();
            using (var stringWriter = new StringWriter(stringBuilder))
            using (var xmlWriter = XmlWriter.Create(stringWriter, settings))
                element.WriteTo(xmlWriter);
            return stringBuilder.ToString();
        }
    }
}