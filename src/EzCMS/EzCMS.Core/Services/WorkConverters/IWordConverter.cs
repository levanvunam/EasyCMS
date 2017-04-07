using System.IO;
using Ez.Framework.Core.IoC.Attributes;

namespace EzCMS.Core.Services.WorkConverters
{
    [Register(Lifetime.PerInstance)]
    public interface IWordConverter
    {
        /// <summary>
        /// Convert to docx
        /// </summary>
        /// <param name="serverMediaFolder"></param>
        /// <param name="imageUrlPrefix"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        byte[] ToDocx(string serverMediaFolder, string imageUrlPrefix, string content);

        /// <summary>
        /// Convert to html
        /// </summary>
        /// <param name="serverMediaFolder"></param>
        /// <param name="imageUrlPrefix"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        string ToHtml(string serverMediaFolder, string imageUrlPrefix, Stream input);

        /// <summary>
        /// Convert to body
        /// </summary>
        /// <param name="serverMediaFolder"></param>
        /// <param name="imageUrlPrefix"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        string ToBody(string serverMediaFolder, string imageUrlPrefix, Stream input);


        /// <summary>
        /// Convert docx to html by service
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        string ConvertDocxToHtmlByService(int pageId, byte[] content);

        /// <summary>
        /// Convert html to docx by service
        /// </summary>
        /// <param name="pageId"></param>
        /// <param name="html"></param>
        /// <returns></returns>
        byte[] ConvertHtmlToDocxByService(int pageId, string html);
    }
}