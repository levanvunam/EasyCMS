using System.Collections.Generic;
using NotesFor.HtmlToOpenXml;

namespace EzCMS.Help.Plugins.Business.Services.WordConverter.Image
{
    public class MemoryImageProvider
    {
        private readonly Dictionary<string, byte[]> _files;

        public MemoryImageProvider(Dictionary<string, byte[]> files)
        {
            _files = files;
        }

        public void Get(object sender, ProvisionImageEventArgs e)
        {
            byte[] value;
            if (_files != null && _files.TryGetValue(e.ImageUrl.OriginalString, out value))
            {
                e.Data = value;
            }
        }
    }
}