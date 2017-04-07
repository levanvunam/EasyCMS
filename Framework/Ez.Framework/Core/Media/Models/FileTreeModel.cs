using System.Collections.Generic;

namespace Ez.Framework.Core.Media.Models
{
    public class FileTreeModel
    {
        public string data;
        public FileTreeAttribute attr;
        public string state;
        public List<FileTreeModel> children;
    }
}
