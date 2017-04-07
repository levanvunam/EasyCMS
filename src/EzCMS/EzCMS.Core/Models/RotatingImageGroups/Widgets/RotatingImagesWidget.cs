using System.Collections.Generic;

namespace EzCMS.Core.Models.RotatingImageGroups.Widgets
{
    public class RotatingImagesWidget
    {
        public RotatingImagesWidget()
        {
            Images = new List<RotatingImageWidget>();
        }

        public int Id { get; set; }

        public string GroupName { get; set; }

        public List<RotatingImageWidget> Images { get; set; }
    }
}
