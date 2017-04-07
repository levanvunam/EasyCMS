using Ez.Framework.Core.IoC.Attributes;
using EzCMS.Core.Models.WordConverters;
using System.Collections.Generic;

namespace EzCMS.Help.Plugins.Business.Services.WordConverter
{
    [Register(Lifetime.PerInstance)]
    public interface IWordConverterService
    {
        /// <summary>
        /// Convert word to html
        /// </summary>
        /// <param name="input"></param>
        /// <param name="mediaPath"></param>
        /// <returns></returns>
        HtmlResult ToHtml(byte[] input, string mediaPath);

        /// <summary>
        /// Convert html to word
        /// </summary>
        /// <param name="content"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        byte[] ToDocx(string content, Dictionary<string, byte[]> files);
    }
}
